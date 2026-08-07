import 'package:flutter/material.dart';
import 'package:get/get.dart';

Future<void> openPdfFromBase64(String base64Data, String fileName) async {
  Get.dialog(
    AlertDialog(
      title: Text('Attached Document: $fileName'),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'PDF viewer in a new tab is supported on Web mode. On mobile, here is the information of this decrypted attachment:',
              style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 10),
            Text(
              'File Name: $fileName\nBase64 size: ${(base64Data.length / 1024).toStringAsFixed(2)} KB',
              style: const TextStyle(fontSize: 12),
            ),
            const SizedBox(height: 15),
            const Text(
              'Under ABDM M3 standard, this decrypted binary was received successfully.',
              style: TextStyle(fontSize: 11, fontStyle: FontStyle.italic, color: Colors.grey),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Get.back(),
          child: const Text('Close'),
        ),
      ],
    ),
  );
}
