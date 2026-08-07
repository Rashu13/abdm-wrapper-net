import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'constants.dart';

class Dimensions {
  static double fontSizeExtraSmall = 11.0;
  static double fontSizeSmall = 13.0;
  static double fontSizeDefault = 15.0;
  static double fontSizeLarge = 17.0;
  static double fontSizeExtraLarge = 22.0;
  static double fontSizeOverLarge = 28.0;
}

final fontRegular = GoogleFonts.outfit(
  fontWeight: FontWeight.w400,
  fontSize: Dimensions.fontSizeDefault,
  color: AppColor.textPrimary,
);

final fontMedium = GoogleFonts.outfit(
  fontWeight: FontWeight.w500,
  fontSize: Dimensions.fontSizeLarge,
  color: AppColor.textPrimary,
);

final fontBold = GoogleFonts.outfit(
  fontWeight: FontWeight.w700,
  fontSize: Dimensions.fontSizeExtraLarge,
  color: AppColor.textPrimary,
);

final fontSmall = GoogleFonts.outfit(
  fontWeight: FontWeight.w400,
  fontSize: Dimensions.fontSizeSmall,
  color: AppColor.textSecondary,
);

final fontItalic = GoogleFonts.outfit(
  fontWeight: FontWeight.w400,
  fontSize: Dimensions.fontSizeSmall,
  color: AppColor.textSecondary,
  fontStyle: FontStyle.italic,
);
final fontTitle = GoogleFonts.outfit(
  fontWeight: FontWeight.w800,
  fontSize: Dimensions.fontSizeOverLarge,
  color: AppColor.textPrimary,
);

BoxDecoration glassDecoration({double radius = 16, Color? borderColor}) {
  return BoxDecoration(
    color: AppColor.surface.withOpacity(0.8),
    borderRadius: BorderRadius.circular(radius),
    border: Border.all(
      color: borderColor ?? AppColor.border.withOpacity(0.5),
      width: 1,
    ),
    boxShadow: [
      BoxShadow(
        color: Colors.black.withOpacity(0.25),
        blurRadius: 16,
        spreadRadius: -2,
        offset: const Offset(0, 8),
      ),
    ],
  );
}
