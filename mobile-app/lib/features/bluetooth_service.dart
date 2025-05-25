import 'package:flutter_blue_plus/flutter_blue_plus.dart';
import 'package:permission_handler/permission_handler.dart';

class BluetoothService {
  static final BluetoothService _instance = BluetoothService._internal();
  factory BluetoothService() => _instance;
  BluetoothService._internal();

  // Stream de rezultate
  Stream<List<ScanResult>> get scanResults => FlutterBluePlus.onScanResults;

  Future<void> requestPermissions() async {
    await [
      Permission.bluetooth,
      Permission.bluetoothScan,
      Permission.bluetoothConnect,
      Permission.locationWhenInUse,
    ].request();
  }

  Future<void> startScan({Duration timeout = const Duration(seconds: 4)}) async {
    await requestPermissions();

    // Oprește scanarea dacă rulează deja
    if (FlutterBluePlus.isScanningNow) {
      FlutterBluePlus.stopScan();
    }

    // Pornește scanarea (fără await!)
    FlutterBluePlus.startScan(
      timeout: timeout,
      androidUsesFineLocation: true,
    );
  }

  void stopScan() {
    if (FlutterBluePlus.isScanningNow) {
      FlutterBluePlus.stopScan();
    }
  }
}
