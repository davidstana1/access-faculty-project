import 'package:flutter/material.dart';
import 'profile_page.dart';
import 'connection_selection_page.dart';
import 'package:acccess_guard/colors.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        elevation: 10.0,
        backgroundColor: appBar_light,
        leading:
            IconButton(
              icon: const Icon(Icons.account_circle, size: 40),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const ProfilePage(),
                  ),
                );
              },
            ),
        title: const Text(
          'Access Guard',
          style: TextStyle(fontSize: 24),
        ),
        centerTitle: true,
      ),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 16.0),
        child: Column(
          children: [
            const SizedBox(height: 250), // Spațiu gol
            Center(
              child: ElevatedButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const ConecctionSelectionPage(),
                    ),
                  );
                },
                child: const Text(
                  "Solicită accesul",
                  style: TextStyle(
                      fontSize: 30,
                      color: Colors.black,
                  ),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: buttonBg_light,
                  minimumSize: const Size.fromHeight(100),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(8),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}