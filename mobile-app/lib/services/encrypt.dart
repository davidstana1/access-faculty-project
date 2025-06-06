import 'package:encrypt/encrypt.dart' as encrypt;

String encryptText(String plainText, String keyString) {
  final key = encrypt.Key.fromUtf8(keyString.padRight(32, '0')); // AES-256: 32 bytes
  final iv = encrypt.IV.fromUtf8('1000000000000000'); // IV = Initialization Vector (16 bytes)

  final encrypter = encrypt.Encrypter(encrypt.AES(key));
  final encrypted = encrypter.encrypt(plainText, iv: iv);

  return encrypted.base64;
}
