import 'package:http/http.dart' as http;
import 'dart:convert';

Future<String?> checkAccesESP({int timeoutSecunde = 30}) async {
  final startTime = DateTime.now();
  final timeout = Duration(seconds: timeoutSecunde);

  while (DateTime.now().difference(startTime) < timeout) {
    try {
      final response = await http.get(Uri.parse('http://192.168.220.72/mobile'));

      if (response.statusCode == 200) {
        final jsonResponse = json.decode(response.body);
        final acces = jsonResponse['acces'];

        if (acces == "permis" || acces == "respins") {
          print("Stare acces: $acces");
          return acces;
        }
      }
    } catch (_) {
      // ignorăm erorile temporare de rețea
    }

    // Așteaptă 1 secundă înainte de următoarea verificare
    await Future.delayed(const Duration(seconds: 1));
  }

  print("Timeout: Nu s-a primit răspuns de la ESP în timpul așteptat.");
  return null;
}
