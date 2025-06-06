import 'package:acccess_guard/pages/home_page.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:acccess_guard/services/send_service_esp.dart';
import 'access_pending_page.dart';
import 'package:acccess_guard/services/send_service_web.dart';
import 'package:acccess_guard/services/get_service_esp.dart';
import 'package:acccess_guard/services/get_service_web.dart';
import 'package:acccess_guard/colors.dart';
import 'denied_page.dart';
import 'accepted_page.dart';

bool receivedResponse = false; // devinde true cand primes raspuns de la web/esp

class ConecctionSelectionPage extends StatelessWidget {
  const ConecctionSelectionPage({super.key});

  Future<void> _onFootPressed(BuildContext context) async {

    await sendDataToWeb(context);

    // Navighează spre pagina AccessPendingPage
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const AccessPendingPage()),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Mod selectat: Pe jos')),
    );

    // Așteaptă până când ESP-ul trimite răspunsul (maxim 10 secunde)
    final raspuns = await checkAccesESP();

    // Revine la pagina principala
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const HomePage(),
      ),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Mod selectat: Pe jos')),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          raspuns == "permis"
              ? 'Acces permis (pe jos)'
              : raspuns == "respins"
              ? 'Acces respins (pe jos)'
              : 'Fără răspuns de la Web',
        ),
      ),
    );

    if (raspuns == "permis") {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const AcceptedPage()),
      );
    } else if (raspuns == "respins") {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const DeniedPage()),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Răspuns necunoscut de la Web")),
      );
    }

    // Așteaptă 3 secunde
    await Future.delayed(const Duration(seconds: 3));

    // Revine la pagina principala
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const HomePage(),
      ),
    );
  }

  Future<void> _byCarPressed(BuildContext context) async {

    await sendDataToEsp(context); // trimite cererea

    // Navighează spre pagina AccessPendingPage
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => const AccessPendingPage()),
    );

    // Așteaptă până când ESP-ul trimite răspunsul (maxim 10 secunde)
    final raspuns = await checkAccesESP();

    // Revine la pagina principala
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const HomePage(),
      ),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Mod selectat: Cu mașina')),
    );

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          raspuns == "permis"
              ? 'Acces permis (cu mașina)'
              : raspuns == "respins"
              ? 'Acces respins (cu mașina)'
              : 'Fără răspuns de la ESP',
        ),
      ),
    );

    if (raspuns == "permis") {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const AcceptedPage()),
      );
    } else if (raspuns == "respins") {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const DeniedPage()),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Răspuns necunoscut de la ESP")),
      );
    }

    // Așteaptă 3 secunde
    await Future.delayed(const Duration(seconds: 3));

    // Revine la pagina principala
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const HomePage(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
        backgroundColor: appBar_light,
        centerTitle: true,
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
