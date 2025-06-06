import 'package:flutter/material.dart';
import 'package:acccess_guard/colors.dart';

class DeniedPage extends StatelessWidget {
  const DeniedPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
      ),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: const [
              Text(
                "Acces refuzat",
                style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
                textAlign: TextAlign.center,
              ),
              SizedBox(height: 10),
              Text(
                "Nu aveti acces in cladire.",
                textAlign: TextAlign.center,
              ),
              SizedBox(height: 20),
              Icon(Icons.cancel_outlined, size: 100, color: Colors.red),
            ],
          ),
        ),
      ),
    );
  }
}