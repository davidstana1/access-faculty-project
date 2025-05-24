import 'package:flutter/material.dart';
import 'pages/puk_page.dart';

void main() {
  runApp(const AccessGuardApp());
}

class AccessGuardApp extends StatelessWidget {
  const AccessGuardApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Access Guard',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(primarySwatch: Colors.blue),
      home: const PukPage(),
    );
  }
}