import 'dart:io';
import 'dart:math';
import 'package:path_provider/path_provider.dart';

class UserDataService {
  Future<String> _getFilePath(String fileName) async {
    final directory = await getApplicationDocumentsDirectory();
    return '${directory.path}/$fileName';
  }

  List<String> _generateUserProfileFromRemote() {
    final random = Random();
    final fakeNames = ['Ion', 'Maria', 'Andrei', 'Elena', 'George'];
    final fakeSurnames = ['Popescu', 'Ionescu', 'Georgescu', 'Dumitrescu', 'Marin'];
    final fakeRoles = ['Admin', 'User', 'Moderator'];

    final name = fakeNames[random.nextInt(fakeNames.length)];
    final surname = fakeSurnames[random.nextInt(fakeSurnames.length)];
    final email = '${name.toLowerCase()}.${surname.toLowerCase()}@example.com';
    final role = fakeRoles[random.nextInt(fakeRoles.length)];

    return [name, surname, email, role];
  }

  Future<void> saveUserProfile(String name, String surname, String email, String role) async {
    final path = await _getFilePath('user_profile.txt');
    final file = File(path);
    final data = '$name\n$surname\n$email\n$role';
    await file.writeAsString(data);
  }

  Future<List<String>> loadUserProfile() async {
    final path = await _getFilePath('user_profile.txt');
    final file = File(path);

    if (!await file.exists()) {
      print('Fișierul profil NU există. Preiau date din "remote".');
      final generatedProfile = _generateUserProfileFromRemote();
      await saveUserProfile(
        generatedProfile[0],
        generatedProfile[1],
        generatedProfile[2],
        generatedProfile[3],
      );
      return generatedProfile;
    }

    return await file.readAsLines();
  }

  Future<void> addAccessHistoryEntry(String entry) async {
    final path = await _getFilePath('access_history.txt');
    final file = File(path);
    await file.writeAsString('$entry\n', mode: FileMode.append);
  }

  Future<List<String>> loadAccessHistory() async {
    final path = await _getFilePath('access_history.txt');
    final file = File(path);

    if (!await file.exists()) {
      print('Fișierul istoric NU există. Scriu prima intrare.');

      final profile = await loadUserProfile();

      final name = profile.isNotEmpty ? profile[0] : 'Necunoscut';
      final surname = profile.length > 1 ? profile[1] : '-';
      final email = profile.length > 2 ? profile[2] : '-';
      final role = profile.length > 3 ? profile[3] : '-';

      final now = DateTime.now();
      final date = '${now.day.toString().padLeft(2, '0')}.' +
          '${now.month.toString().padLeft(2, '0')}.' +
          '${now.year}';

      const gatekeeperName = 'Gheorghe Ionescu';

      final entry = '$date - Prima accesare a aplicației | '
          'Nume portar: $gatekeeperName | '
          'Nume: $name, Prenume: $surname, '
          'Email: $email, Rol: $role';

      await file.writeAsString('$entry\n');
      print('Scris în istoric: $entry');
    }

    return await file.readAsLines();
  }
}
