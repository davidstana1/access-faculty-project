import 'package:http/http.dart' as http;
import 'dart:convert';

Future<String?> checkAccesWeb({int timeoutSecunde = 30}) async {
  final startTime = DateTime.now();
  final timeout = Duration(seconds: timeoutSecunde);

  const webIp = "192.168.220.202"; // IP-ul aplicației web (backend)
  final url = Uri.parse('http://$webIp/get-data'); // Endpoint-ul care întoarce status

  while (DateTime.now().difference(startTime) < timeout) {
    try {
      final response = await http.get(url);

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);
        final acces = jsonResponse['acces'];

        if (acces == "permis" || acces == "respins") {
          print("Stare acces (web): $acces");
          return acces;
        }
      }
    } catch (_) {
      // Ignorăm erori temporare
    }

    await Future.delayed(const Duration(seconds: 1));
  }

  print("Timeout: Nu s-a primit răspuns de la Web în timpul așteptat.");
  return null;
}
