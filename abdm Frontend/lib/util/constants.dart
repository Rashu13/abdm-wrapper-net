import 'package:flutter/material.dart';

class AppColor {
  // Rich Web Light Theme Colors
  static const Color background = Color(0xFFF8FAFC); // Clean Light Slate
  static const Color surface = Color(0xFFFFFFFF);    // Pure White Surface
  static const Color surfaceLight = Color(0xFFF1F5F9); // Light Grey
  
  static const Color primary = Color(0xFF0F4C81);    // ABDM Deep Blue
  static const Color primaryDark = Color(0xFF0A3359);
  static const Color accent = Color(0xFF0284C7);     // Vibrant Sky Blue
  
  static const Color textPrimary = Color(0xFF0F172A);  // Dark Slate Text
  static const Color textSecondary = Color(0xFF64748B); // Slate Secondary
  static const Color border = Color(0xFFE2E8F0);       // Light Border

  // Status Colors
  static const Color success = Color(0xFF10B981);    // Emerald
  static const Color warning = Color(0xFFF59E0B);    // Amber
  static const Color error = Color(0xFFEF4444);      // Rose Red
  static const Color info = Color(0xFF0EA5E9);       // Sky Blue

  // Gradients
  static const LinearGradient primaryGradient = LinearGradient(
    colors: [Color(0xFF0F4C81), Color(0xFF0284C7)],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
  );

  static const LinearGradient cardGradient = LinearGradient(
    colors: [Color(0xFFFFFFFF), Color(0xFFF8FAFC)],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
  );

  static const LinearGradient abhaCardGradient = LinearGradient(
    colors: [Color(0xFF0F4C81), Color(0xFF0D9488)],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
  );
}

class AppImages {
  static const String appLogo = 'assets/images/abdm_logo.png';
  static const String abhaCardBg = 'assets/images/abha_card_bg.png';
  static const String qrPlaceholder = 'assets/images/qr_placeholder.png';
}
