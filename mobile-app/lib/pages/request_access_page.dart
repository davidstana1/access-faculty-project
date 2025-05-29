import 'package:flutter/material.dart';
import 'access_history_page.dart';
import 'connection_selection_page.dart';

class RequestAccessPage extends StatelessWidget {
  const RequestAccessPage({super.key});

  @override
  // Widget build(BuildContext context) {
  //   return Scaffold(
  //     body: SafeArea(
  //       child: Padding(
  //         padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 16.0),
  //         child: Column(
  //           crossAxisAlignment: CrossAxisAlignment.start,
  //           children: [
  //             // Titlu centrat
  //             Center(
  //               child: Text(
  //                 'Access Guard',
  //                 style: Theme.of(context).textTheme.headlineSmall,
  //               ),
  //             ),
  //             const SizedBox(height: 16),
  //
  //             // Iconiță profil
  //             IconButton(
  //               icon: const Icon(Icons.account_circle, size: 40),
  //               onPressed: () {
  //                 Navigator.push(
  //                   context,
  //                   MaterialPageRoute(
  //                     builder: (context) => const AccessHistoryPage(),
  //                   ),
  //                 );
  //               },
  //             ),
  //
  //             const SizedBox(height: 250), // Spațiu vertical între icon și buton
  //
  //             // Buton pe mijlocul ecranului
  //             Center(
  //               child: ElevatedButton(
  //                 onPressed: () {
  //                   Navigator.push(
  //                     context,
  //                     MaterialPageRoute(
  //                       builder: (context) => const ConecctionSelectionPage(),
  //                     ),
  //                   );
  //                 },
  //                 child: const Text(
  //                     "Solicita accesul",
  //                     style: TextStyle(
  //                       fontSize: 30
  //                     ),
  //                 ),
  //                 style: ElevatedButton.styleFrom(
  //                     minimumSize: const Size.fromHeight(100),
  //                     shape: RoundedRectangleBorder(
  //                       borderRadius: BorderRadius.circular(8),
  //                     ),
  //                 ),
  //               ),
  //             ),
  //           ],
  //         ),
  //       ),
  //     ),
  //   );
  // }

  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        elevation: 10.0,
        backgroundColor: Colors.blueAccent,
        leading: IconButton(
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
                  backgroundColor: Colors.blueAccent,
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