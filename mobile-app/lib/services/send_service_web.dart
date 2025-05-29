import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

Future<void> sendDataToWeb(BuildContext context) async {
  //const payload = 'Salut, laptop!';
  String Web_link = "http://192.168.220.202";
  String Web_port = "5000";
  String puk = "12345678";
  final url = Uri.parse('$Web_link:$Web_port/api/Esp/mobile');

 // final url = Uri.parse('http://<IP-API>.NET>:5000/api/message');
  try {
    final response = await http.post(
      url,
      body: {'message': puk},
    );
    if (response.statusCode == 200) {
      print('Mesaj trimis cu succes!');
    }
  } catch (e) {
    final error = 'Eroare la trimitere: $e';
    print(error);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(error), backgroundColor: Colors.red),
    );
  }

}

