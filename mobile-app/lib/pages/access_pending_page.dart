import 'package:flutter/material.dart';

class AccessPendingPage extends StatelessWidget {
  const AccessPendingPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Cerere trimisă"),
      ),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: const [
              Icon(Icons.hourglass_top, size: 80, color: Colors.orange),
              SizedBox(height: 20),
              Text(
                "Cererea ta a fost trimisă.",
                style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
                textAlign: TextAlign.center,
              ),
              SizedBox(height: 10),
              Text(
                "Așteptăm aprobarea administratorului pentru a-ți permite accesul.",
                textAlign: TextAlign.center,
              ),
            ],
          ),
        ),
      ),
    );
  }
}