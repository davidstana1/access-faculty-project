import 'package:acccess_guard/pages/request_access_page.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:acccess_guard/services/send_service_esp.dart';
import 'access_pending_page.dart';
import 'package:acccess_guard/services/send_service_web.dart';


class ConecctionSelectionPage extends StatelessWidget {
  const ConecctionSelectionPage({super.key});

  Future<void> _onFootPressed(BuildContext context) async {
    // TODO: Adaugă logica pentru „Pe jos”

    await sendDataToWeb(context);

    // Navighează spre pagina AccessPendingPage
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const AccessPendingPage()),
    );

    // Așteaptă 3 secunde
    await Future.delayed(const Duration(seconds: 3));

    // Revine la pagina principala
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const RequestAccessPage(),
      ),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Mod selectat: Pe jos')),
    );

    // Navighează spre o altă pagină dacă vrei:
    // Navigator.push(context, MaterialPageRoute(builder: (_) => const SomeOtherPage()));
  }

  Future<void> _byCarPressed(BuildContext context) async {
    // TODO: Adaugă logica pentru „Cu mașina”

    await sendDataToEsp(context); // trimite cererea

    // Navighează spre pagina AccessPendingPage
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const AccessPendingPage()),
    );

    // Așteaptă 3 secunde
    await Future.delayed(const Duration(seconds: 3));

    // Revine la pagina principala
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const RequestAccessPage(),
      ),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Mod selectat: Cu mașina')),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Alege modul de acces"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            ElevatedButton.icon(
              onPressed: () => _onFootPressed(context),
              icon: const Icon(
                  Icons.directions_walk,
                  size: 25,
              ),
              label: const Text(
                  "Pe jos",
                  style: TextStyle(
                    fontSize: 20,
                  ),
              ),
              style: ElevatedButton.styleFrom(
                minimumSize: const Size.fromHeight(100),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ),
            const SizedBox(height: 20),
            ElevatedButton.icon(
              onPressed: () => _byCarPressed(context),
              icon: const Icon(
                  Icons.directions_car,
                  size: 25,
              ),
              label: const Text(
                  "Cu mașina",
                  style: TextStyle(
                    fontSize: 20
                  ),
              ),
              style: ElevatedButton.styleFrom(
                minimumSize: const Size.fromHeight(100),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
