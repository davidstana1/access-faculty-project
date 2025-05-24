import 'package:flutter/material.dart';

class AccessHistoryPage extends StatelessWidget {
  const AccessHistoryPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Profil utilizator"),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text("Nume", style: TextStyle(fontSize: 18)),
            const Text("Prenume", style: TextStyle(fontSize: 18)),
            const Text("E-mail", style: TextStyle(fontSize: 18)),
            const Text("rol", style: TextStyle(fontSize: 18)),
            const SizedBox(height: 30),
            const Text(
              "Istoricul accesarilor",
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 20),
            ),
            const SizedBox(height: 10),
            Container(
              color: Colors.grey[300],
              padding: const EdgeInsets.all(10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: const [
                  Text("22/05/2025", style: TextStyle(color: Colors.red)),
                  SizedBox(height: 5),
                  Text("Nume Prenume"),
                  Text("10-18"),
                  Text("P: Nume Prenume"),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}