import 'package:flutter/material.dart';
import 'pages/puk_page.dart';

// --- debug ---
import 'pages/error_page.dart';                // ErrorPage
import 'pages/accepted_page.dart';             // AcceptedPage
import 'pages/denied_page.dart';               // DeniedPage
import 'pages/home_page.dart';                 // HomePage
import 'pages/puk_page.dart';                  // PukPage
import 'pages/profile_page.dart';              // ProfilePage
import 'pages/access_pending_page.dart';       // AccessPendingPage
import 'pages/connection_selection_page.dart'; // ConecctionSelectionPage

void main() {
  //runApp(const AccessGuardApp());

  // --- debug ---
  runApp(const AccessGuardApp());
}

// --- debug ---
class TestPage extends StatelessWidget {
  const TestPage({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      home: ProfilePage(),    // modifica
      debugShowCheckedModeBanner: false,
    );
  }
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