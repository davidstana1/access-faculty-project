import 'package:flutter/material.dart';
import 'package:acccess_guard/features/user_data/user_data_service.dart';

class AccessHistoryPage extends StatefulWidget {
  const AccessHistoryPage({super.key});

  @override
  State<AccessHistoryPage> createState() => _AccessHistoryPageState();
}

class _AccessHistoryPageState extends State<AccessHistoryPage> {
  final UserDataService _userDataService = UserDataService();
  List<String> _profile = [];
  List<String> _history = [];

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    final profile = await _userDataService.loadUserProfile();

    // Adaugă o nouă intrare la fiecare accesare
    final now = DateTime.now();
    final date = '${now.day.toString().padLeft(2, '0')}.' +
        '${now.month.toString().padLeft(2, '0')}.' +
        '${now.year}, ${now.hour.toString().padLeft(2, '0')}:${now.minute.toString().padLeft(2, '0')}';

    final name = profile.isNotEmpty ? profile[0] : 'Necunoscut';
    final surname = profile.length > 1 ? profile[1] : '-';

    final entry = '$date - Acces aplicație de către $name $surname';
    await _userDataService.addAccessHistoryEntry(entry);

    final history = await _userDataService.loadAccessHistory();

    print('Profil încărcat: $profile');
    print('Istoric încărcat: $history');

    setState(() {
      _profile = profile;
      _history = history;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Profil utilizator"),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.pop(context),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(_profile.isNotEmpty ? 'Nume: ${_profile[0]}' : "Nume", style: const TextStyle(fontSize: 18)),
            Text(_profile.length > 1 ? 'Prenume: ${_profile[1]}' : "Prenume", style: const TextStyle(fontSize: 18)),
            Text(_profile.length > 2 ? 'Email: ${_profile[2]}' : "Email", style: const TextStyle(fontSize: 18)),
            Text(_profile.length > 3 ? 'Rol: ${_profile[3]}' : "Rol", style: const TextStyle(fontSize: 18)),
            const SizedBox(height: 30),
            const Text(
              "Istoricul accesarilor",
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 20),
            ),
            const SizedBox(height: 10),
            Expanded(
              child: ListView.builder(
                  itemCount: _history.length,
                  itemBuilder: (context, index) {
                    return Container(
                      margin: const EdgeInsets.only(bottom: 10),
                      padding: const EdgeInsets.all(10),
                      color: Colors.grey[300],
                      child: Text(_history[index]),
                    );
                  }
              ),
            ),
          ],
        ),
      ),
    );
  }
}
