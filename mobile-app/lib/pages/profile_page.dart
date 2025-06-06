import 'package:flutter/material.dart';
import 'package:acccess_guard/colors.dart';

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.blue,
        centerTitle: true,
        title: const Text(
            "Profil utilizator",
            style: TextStyle(fontSize: 24),
        ),
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
            const Text("Stana", style: TextStyle(fontSize: 18)),
            const Text("David", style: TextStyle(fontSize: 18)),
            const Text("CNP: 5040222020012", style: TextStyle(fontSize: 18)),
            const Text("Badge number: 9125", style: TextStyle(fontSize: 18)),
            const Text("IT Department", style: TextStyle(fontSize: 18)),
            const Text("Vehicle number: AR20ABC", style: TextStyle(fontSize: 18)),
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