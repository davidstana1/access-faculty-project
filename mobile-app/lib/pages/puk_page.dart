import 'package:flutter/material.dart';
import 'package:encrypt/encrypt.dart' as encrypt;
import 'home_page.dart';
import 'package:acccess_guard/session.dart';
import 'package:acccess_guard/services/encrypt.dart';

class PukPage extends StatefulWidget {
  const PukPage({super.key});

  @override
  State<PukPage> createState() => _PukPageState();
}

class _PukPageState extends State<PukPage> {
  final TextEditingController _controller = TextEditingController();
  final String _validPuk = "12345678";

  void _validatePuk() {
    final enteredPuk = _controller.text.trim();


    if (enteredPuk.isEmpty) {
      _showMessage("Introdu un cod PUK.");
      return;
    }

    final encryptedPuk = encryptText(enteredPuk, "shelby");

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('PUK introdus: $enteredPuk\nPUK criptat: $encryptedPuk'),
        duration: const Duration(seconds: 6),
      ),
    );

    if (enteredPuk == _validPuk) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const HomePage()),
      );
    } else {
      _showMessage("Cod PUK incorect.");
    }

    SessionData.pukCriptat = enteredPuk;
  }

  void _showMessage(String message) {
    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: const Text("Avertisment"),
        content: Text(message),
        actions: [
          TextButton(
            child: const Text("OK"),
            onPressed: () => Navigator.pop(context),
          )
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 30.0),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Text(
                'Introduceți codul PUK:',
                style: TextStyle(fontSize: 18),
              ),
              const SizedBox(height: 20),
              TextField(
                controller: _controller,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  border: OutlineInputBorder(),
                  hintText: 'Cod PUK',
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _validatePuk,
                child: const Text('OK'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
