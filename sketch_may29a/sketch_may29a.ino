/*
   IR Breakbeam sensor demo for ESP32!
*/
#include <WiFi.h>
#include <ESP32Servo.h>
#include <WebServer.h>
#include <HTTPClient.h>

const char* ssid = "Gicu"; // Înlocuiește cu numele rețelei tale WiFi
const char* password = "12345678"; // Înlocuiește cu parola rețelei tale WiFi

#define LEDPIN 2     // Pin 2: ESP32 are un LED conectat pe pinul 2 (de obicei pe plăcile devkit)
#define SENSORPIN1 4 // Pinul pentru senzorul IR Breakbeam 1 (intrare)
#define SENSORPIN4 14 // Pinul pentru senzorul IR Breakbeam 4 (ieșire)
#define SERVO_PIN 15 // Pinul pentru servomotor

Servo myservo;

#define BARRIER_UP_ANGLE 90
#define BARRIER_DOWN_ANGLE 0

// Variabile care se vor schimba:
int sensorState1 = 0, lastState1 = 0; // Variabile pentru citirea stării senzorului 1 (intrare)
int sensorState4 = 0, lastState4 = 0; // Variabile pentru citirea stării senzorului 4 (ieșire)
bool intrareDetectata = false;          // Indica dacă senzorul de intrare a fost întrerupt primul
bool iesireDetectata = false;           // Indica dacă senzorul de ieșire a fost întrerupt primul

WebServer server(80);  // portul HTTP

// Adresa serverului web unde vei trimite datele
const char* webServerAddress = "http://192.168.220.202:5000/api/esp";

// Variabila pentru a stoca string-ul primit de la web
String can_enter = "";

void handleRoot() {
  server.send(200, "text/plain", "ESP32 este online.");
}

void handleData() {
  if (server.hasArg("value")) {
    String value = server.arg("value");
    Serial.println("Am primit: " + value);
    if (value) {
      sendDataToWebServer(value); // Trimite datele primite către serverul web
    } else {
      Serial.println("Nu e bine");
    }
    server.send(200, "text/plain", "Primit si trimis: " + value);
  } else {
    server.send(400, "Lipseste parametrul 'value'");
  }
}

void setup() {
  // Inițializează pinul LED ca ieșire:
  pinMode(LEDPIN, OUTPUT);
  // Inițializează pinii senzorilor ca intrări:
  pinMode(SENSORPIN1, INPUT_PULLUP); // ESP32 suportă INPUT_PULLUP nativ
  pinMode(SENSORPIN4, INPUT_PULLUP); // ESP32 suportă INPUT_PULLUP nativ

  // Initialize servo
  ESP32PWM::allocateTimer(0);
  myservo.setPeriodHertz(50);
  myservo.attach(SERVO_PIN, 500, 2400);
  myservo.write(BARRIER_DOWN_ANGLE);

  Serial.begin(115200); // Setează viteza serialului la 115200 pentru ESP32

  // Conectare la WiFi
  Serial.println("Conectare la WiFi...");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi conectat");
  Serial.print("Adresa IP: ");
  Serial.println(WiFi.localIP());

  server.on("/", handleRoot);
  server.on("/send", handleData); // Ex: http://ip/send?value=123
  server.on("/data", HTTP_POST, handlePostData);
   server.on("/mobile", HTTP_GET, []() {
    if (can_enter == "true") {
      Serial.println("Trimis la mobile (GET): acces permis");
      server.send(200, "application/json", "{\"acces\":\"permis\"}");
    } else if(can_enter=="false"){
      Serial.println("Trimis la mobile (GET): acces respins");
      server.send(200, "application/json", "{\"acces\":\"respins\"}");
    }
  });

  server.begin();
  Serial.println("Server pornit!");
}

void sendDataToWebServer(String data) {
  HTTPClient http;

  http.begin("http://192.168.220.202:5000/api/esp");
  http.addHeader("Content-Type", "text/plain");

  int httpCode = http.POST(data);

  if (httpCode > 0) {
    Serial.printf("[HTTP] POST... code: %d\n", httpCode);
    if (httpCode == HTTP_CODE_OK) {
      String payload = http.getString();
      Serial.println("Payload primit de la server: " + payload);
    }
  } else {
    Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
  }

  http.end();
}

void handlePostData() {
  if (server.method() == HTTP_POST) {
    String body = server.arg("plain");
    Serial.println("Date primite de la backend:");
    Serial.println(body);
    if (can_enter != body) { // Verifică dacă valoarea este diferită de cea anterioară
      can_enter = body;
      Serial.print("can_enter: ");
      Serial.println(can_enter);
    }
    server.send(200, "application/json", "{\"status\":\"received\"}");
  } else {
    server.send(405, "text/plain", "Method Not Allowed");
  }

}

void handleEntrance() {
  // A fost întrerupt primul senzorul de intrare (pin 4)
  if (can_enter == "true") {
    myservo.write(BARRIER_UP_ANGLE); // Ridică bariera
    Serial.println("Bariera ridicata pentru intrare.");
    intrareDetectata = true;
  } else if (can_enter == "false") {
    Serial.println("Acces intrare NEPERMIS!");
  }
}

void handleExit() {
  // A fost întrerupt primul senzorul de ieșire (pin 14)
  myservo.write(BARRIER_UP_ANGLE); // Ridică bariera
  Serial.println("Bariera ridicata pentru iesire.");
  iesireDetectata = true;
}


void loop() {
  server.handleClient();

  // Citește starea senzorilor:
  sensorState1 = digitalRead(SENSORPIN1); // Senzor intrare
  sensorState4 = digitalRead(SENSORPIN4); // Senzor ieșire

  // Detectează primul senzor întrerupt (flanc descendent)
  if (!intrareDetectata && !iesireDetectata) {
    if (sensorState1 == LOW && lastState1 == HIGH) {
      handleEntrance();
    } else if (sensorState4 == LOW && lastState4 == HIGH) {
      handleExit();
    }
  }

  // Dacă e în proces de intrare, coboară bariera când se întrerupe senzorul de ieșire (flanc descendent)
  if (intrareDetectata && sensorState4 == LOW && lastState4 == HIGH) {
    myservo.write(BARRIER_DOWN_ANGLE); // Coboară bariera
    Serial.println("Bariera coborata dupa intrare.");
    intrareDetectata = false;
    can_enter="";
  }

  // Dacă e în proces de ieșire, coboară bariera când se întrerupe senzorul de intrare (flanc descendent)
  if (iesireDetectata && sensorState1 == LOW && lastState1 == HIGH) {
    myservo.write(BARRIER_DOWN_ANGLE); // Coboară bariera
    Serial.println("Bariera coborata dupa iesire.");
    iesireDetectata = false;
    can_enter="";
  }

  // Afișează mesaje pe serial dacă starea s-a schimbat pentru senzorul 1:
  if (sensorState1 && !lastState1) {
    Serial.println("Senzor 1: Unbroken");
  }
  if (!sensorState1 && lastState1) {
    Serial.println("Senzor 1: Broken");
  }
  lastState1 = sensorState1;

  // Afișează mesaje pe serial dacă starea s-a schimbat pentru senzorul 4:
  if (sensorState4 && !lastState4) {
    Serial.println("Senzor 4: 1");
  }
  if (!sensorState4 && lastState4) {
    Serial.println("Senzor 4: 0");
  }
  lastState4 = sensorState4;
}