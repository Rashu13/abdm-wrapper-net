import 'dart:convert';
import 'dart:html' as html;

Future<void> openPdfFromBase64(String base64Data, String fileName) async {
  try {
    String cleanBase64 = base64Data;
    if (base64Data.contains(',')) {
      cleanBase64 = base64Data.split(',').last;
    }
    cleanBase64 = cleanBase64.replaceAll(RegExp(r'\s+'), ''); // remove whitespace

    final bytes = base64Decode(cleanBase64);
    final blob = html.Blob([bytes], 'application/pdf');
    final url = html.Url.createObjectUrlFromBlob(blob);
    
    html.window.open(url, '_blank');
  } catch (e) {
    print("Error opening PDF: $e");
  }
}
