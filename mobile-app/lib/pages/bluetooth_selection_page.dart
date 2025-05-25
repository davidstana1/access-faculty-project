import 'package:flutter/material.dart';
import 'package:flutter_blue_plus/flutter_blue_plus.dart';
import 'access_pending_page.dart';
import 'package:acccess_guard/features/bluetooth_service.dart' as my_bt;

class BluetoothSelectionPage extends StatefulWidget {
  const BluetoothSelectionPage({super.key});

  @override
  State<BluetoothSelectionPage> createState() => _BluetoothSelectionPageState();
}

class _BluetoothSelectionPageState extends State<BluetoothSelectionPage> {
  final bluetoothService = my_bt.BluetoothService();

  @override
  void initState() {
    super.initState();
    bluetoothService.startScan();
  }

  @override
  void dispose() {
    bluetoothService.stopScan();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Alege conexiunea")),
      body: StreamBuilder<List<ScanResult>>(
        stream: bluetoothService.scanResults,
        builder: (context, snapshot) {
          if (!snapshot.hasData) {
            return const Center(child: CircularProgressIndicator());
          }

          final filteredDevices = snapshot.data!
              .map((r) => r.device)
              .where((d) =>
          d.name.isNotEmpty &&
              d.name.toLowerCase().contains("desktop")) // înlocuiește "laptop" cu ce vrei
              .toList();

          if (filteredDevices.isEmpty) {
            return const Center(child: Text("Niciun dispozitiv relevant găsit."));
          }

          return ListView.builder(
            itemCount: filteredDevices.length,
            itemBuilder: (context, index) {
              final device = filteredDevices[index];
              return Card(
                child: ListTile(
                  title: Text(device.name),
                  subtitle: Text(device.id.id),
                  trailing: const Icon(Icons.bluetooth),
                  onTap: () {
                    // acțiune la tap
                  },
                ),
              );
            },
          );
        },
      ),
    );
  }
}
