class AbhaSuggestionsModel {
  final bool success;
  final String? message;
  final List<String> suggestions;

  AbhaSuggestionsModel({
    required this.success,
    this.message,
    required this.suggestions,
  });

  factory AbhaSuggestionsModel.fromJson(Map<String, dynamic> json) {
    List<String> list = [];
    if (json['data'] != null && json['data'] is List) {
      list = (json['data'] as List).map((e) => e.toString()).toList();
    } else if (json['suggestions'] != null && json['suggestions'] is List) {
      list = (json['suggestions'] as List).map((e) => e.toString()).toList();
    }
    return AbhaSuggestionsModel(
      success: json['success'] ?? true,
      message: json['message'],
      suggestions: list,
    );
  }
}
