import 'dart:convert';
import 'dart:html' as html;
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
  final html.FileUploadInputElement uploadInput = html.FileUploadInputElement();
  uploadInput.accept = '.pdf';
  uploadInput.click();

  await uploadInput.onChange.first;

  if (uploadInput.files == null || uploadInput.files!.isEmpty) {
    return null;
  }

  final html.File file = uploadInput.files!.first;
  final reader = html.FileReader();

  reader.readAsArrayBuffer(file);
  await reader.onLoadEnd.first;

  final Uint8List bytes = reader.result as Uint8List;
  final String base64Data = base64Encode(bytes);

  return PickedFileData(
    fileName: file.name,
    bytes: bytes,
    base64Data: base64Data,
  );
}
