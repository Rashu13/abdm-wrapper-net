import 'dart:typed_data';

class PickedFileData {
  final String fileName;
  final Uint8List bytes;
  final String base64Data;

  PickedFileData({
    required this.fileName,
    required this.bytes,
    required this.base64Data,
  });
}

Future<PickedFileData?> pickPdfFile() async {
  return null;
}
