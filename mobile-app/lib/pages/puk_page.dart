import 'package:flutter/material.dart';
import 'request_access_page.dart';

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
    } else if (enteredPuk == _validPuk) {
      Navigator.push(
        context,
        MaterialPageRoute(builder: (context) => const RequestAccessPage()),
      );
    } else {
      _showMessage("Cod PUK incorect.");
    }
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
                'Introduceti codul PUK:',
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
                child: const Text('ok'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}