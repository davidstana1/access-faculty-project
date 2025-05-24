import 'package:flutter/material.dart';
import 'access_history_page.dart';
import 'bluetooth_selection_page.dart';

class RequestAccessPage extends StatelessWidget {
  const RequestAccessPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Titlu centrat
              Center(
                child: Text(
                  'Solicita accesul',
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
              ),
              const SizedBox(height: 16),

              // Iconiță profil
              IconButton(
                icon: const Icon(Icons.account_circle, size: 40),
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const AccessHistoryPage(),
                    ),
                  );
                },
              ),

              const SizedBox(height: 250), // Spațiu vertical între icon și buton

              // Buton pe mijlocul ecranului
              Center(
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => const BluetoothSelectionPage(),
                      ),
                    );
                  },
                  child: const Text("Solicita accesul"),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}