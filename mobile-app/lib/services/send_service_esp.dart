import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:acccess_guard/session.dart';

Future<void> sendDataToEsp(BuildContext context) async {

  String? puk = SessionData.pukCriptat;
  String ESP_link = "http://192.168.220.72";
  final url = Uri.parse('$ESP_link/send?value=$puk');

  try {
    final response = await http.get(url);

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
