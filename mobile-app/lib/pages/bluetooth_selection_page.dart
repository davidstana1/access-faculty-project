import 'package:flutter/material.dart';
import 'access_pending_page.dart';


class BluetoothSelectionPage extends StatelessWidget {
  const BluetoothSelectionPage({super.key});

  @override
  Widget build(BuildContext context) {
    // Dispozitive Bluetooth fictive
    final List<String> devices = [
      "Dispozitivul 1 - 00:11:22:33:44:55",
      "Dispozitivul 2 - AA:BB:CC:DD:EE:FF",
      "Dispozitivul 3 - 66:77:88:99:00:11",
    ];

    return Scaffold(
      appBar: AppBar(
        title: const Text("Alege conexiunea"),
      ),
      body: ListView.builder(
        itemCount: devices.length,
        itemBuilder: (context, index) {
          return Card(
            margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: ListTile(
              title: Text(devices[index]),
              trailing: const Icon(Icons.bluetooth),
              onTap: () {
                // Navighează spre pagina următoare (pagina 5)
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const AccessPendingPage(),
                  ),
                );
              },
            ),
          );
        },
      ),
    );
  }
}

// Pagină temporară pentru pasul următor
class PlaceholderPage extends StatelessWidget {
  const PlaceholderPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Conectare...")),
      body: const Center(child: Text("Simulare pagina 5")),
    );
  }
}