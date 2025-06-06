import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:acccess_guard/session.dart';

Future<void> sendDataToWeb(BuildContext context) async {
  String? puk = SessionData.pukCriptat;
  //String puk = "12300";
  String WEB_link = "http://192.168.220.202:5000";
  String url = "$WEB_link/api/esp/mobile";

  try {
    final response = await http.post(
        Uri.parse(url),
        headers: {'Content-Type': 'text/plain'},
        body: puk,
    );

    if (response.statusCode == 200) {
      final message = 'Trimis GET către: $url\nRăspuns: ${response.body}';
      print(message);
      // ScaffoldMessenger.of(context).showSnackBar(
      //   SnackBar(content: Text(message)),
      // );
    } else {
      final error = 'Eroare server: ${response.statusCode}';
      print(error);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(error), backgroundColor: Colors.red),
      );
    }
  } catch (e) {
    final error = 'Eroare la trimitere: $e';
    print(error);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(error), backgroundColor: Colors.red),
    );
  }
  }



