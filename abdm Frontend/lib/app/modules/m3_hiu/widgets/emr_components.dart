import 'package:flutter/material.dart';
import '../../../../util/constants.dart';

// ═══════════════════════════════════════════════════════════════════════════════
// REUSABLE EMR COMPONENT 1: EmrCompactTextField
// ═══════════════════════════════════════════════════════════════════════════════
class EmrCompactTextField extends StatelessWidget {
  final TextEditingController controller;
  final String labelText;
  final String? hintText;
  final TextInputType? keyboardType;
  final int maxLines;

  const EmrCompactTextField({
    Key? key,
    required this.controller,
    required this.labelText,
    this.hintText,
    this.keyboardType,
    this.maxLines = 1,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    Widget inputWidget = TextField(
      controller: controller,
      keyboardType: keyboardType,
      maxLines: maxLines,
      style: TextStyle(color: AppColor.textPrimary, fontSize: 12),
      decoration: InputDecoration(
        isDense: true,
        hintText: hintText ?? labelText,
        hintStyle: TextStyle(
            color: AppColor.textSecondary.withValues(alpha: 0.4),
            fontSize: 11.5),
        filled: true,
        fillColor: AppColor.background,
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(6)),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(6),
          borderSide: BorderSide(color: AppColor.border),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(6),
          borderSide:
              const BorderSide(color: Color(0xFF2563EB), width: 1.5),
        ),
        contentPadding: EdgeInsets.symmetric(
            horizontal: 10, vertical: maxLines > 1 ? 8 : 7.5),
      ),
    );

    if (maxLines == 1) {
      inputWidget = SizedBox(
        height: 34,
        child: inputWidget,
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (labelText.isNotEmpty) ...[
          Text(
            labelText,
            style: const TextStyle(
              color: Color(0xFF64748B),
              fontSize: 10.5,
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 3),
        ],
        inputWidget,
      ],
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// REUSABLE EMR COMPONENT 2: EmrAddButton
// ═══════════════════════════════════════════════════════════════════════════════
class EmrAddButton extends StatelessWidget {
  final String label;
  final VoidCallback onPressed;
  final IconData icon;
  final Color color;

  const EmrAddButton({
    Key? key,
    required this.label,
    required this.onPressed,
    this.icon = Icons.add_circle_outline_rounded,
    this.color = const Color(0xFF10B981),
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return TextButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, size: 18, color: color),
      label: Text(
        label,
        style:
            TextStyle(color: color, fontWeight: FontWeight.bold, fontSize: 13),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// REUSABLE EMR COMPONENT 3: EmrPrimaryButton
// ═══════════════════════════════════════════════════════════════════════════════
class EmrPrimaryButton extends StatelessWidget {
  final String label;
  final VoidCallback? onPressed;
  final IconData? icon;
  final Color backgroundColor;
  final Color foregroundColor;
  final bool isLoading;
  final double height;

  const EmrPrimaryButton({
    Key? key,
    required this.label,
    required this.onPressed,
    this.icon,
    this.backgroundColor = const Color(0xFF2563EB),
    this.foregroundColor = Colors.white,
    this.isLoading = false,
    this.height = 40,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: height,
      child: ElevatedButton(
        onPressed: isLoading ? null : onPressed,
        style: ElevatedButton.styleFrom(
          backgroundColor: backgroundColor,
          foregroundColor: foregroundColor,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
          elevation: 2,
          padding: const EdgeInsets.symmetric(horizontal: 16),
        ),
        child: isLoading
            ? const SizedBox(
                width: 18,
                height: 18,
                child: CircularProgressIndicator(
                    strokeWidth: 2, color: Colors.white),
              )
            : Row(
                mainAxisAlignment: MainAxisAlignment.center,
                mainAxisSize: MainAxisSize.min,
                children: [
                  if (icon != null) ...[
                    Icon(icon, size: 16),
                    const SizedBox(width: 6),
                  ],
                  Text(
                    label,
                    style: const TextStyle(
                        fontSize: 12, fontWeight: FontWeight.bold),
                  ),
                ],
              ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// REUSABLE EMR COMPONENT 4: EmrSelectBox (Custom Dropdown Component)
// ═══════════════════════════════════════════════════════════════════════════════
class EmrSelectBox<T> extends StatelessWidget {
  final String labelText;
  final T value;
  final List<T> items;
  final String Function(T item)? itemLabelBuilder;
  final ValueChanged<T?> onChanged;
  final double? width;
  final double height;

  const EmrSelectBox({
    Key? key,
    required this.labelText,
    required this.value,
    required this.items,
    required this.onChanged,
    this.itemLabelBuilder,
    this.width,
    this.height = 34,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisSize: MainAxisSize.min,
      children: [
        if (labelText.isNotEmpty) ...[
          Text(
            labelText,
            style: const TextStyle(
              color: Color(0xFF64748B),
              fontSize: 10.5,
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 3),
        ],
        Container(
          width: width ?? double.infinity,
          height: height,
          padding: const EdgeInsets.symmetric(horizontal: 10),
          decoration: BoxDecoration(
            color: AppColor.background,
            borderRadius: BorderRadius.circular(6),
            border: Border.all(color: AppColor.border),
          ),
          child: DropdownButtonHideUnderline(
            child: DropdownButton<T>(
              value: items.contains(value) ? value : items.first,
              isExpanded: true,
              isDense: true,
              icon: const Icon(Icons.keyboard_arrow_down_rounded,
                  color: Color(0xFF64748B), size: 16),
              style: TextStyle(
                color: AppColor.textPrimary,
                fontSize: 12,
                fontWeight: FontWeight.w500,
              ),
              dropdownColor: AppColor.surface,
              borderRadius: BorderRadius.circular(6),
              items: items.map((T item) {
                final text = itemLabelBuilder != null
                    ? itemLabelBuilder!(item)
                    : item.toString();
                return DropdownMenuItem<T>(
                  value: item,
                  child: Text(
                    text,
                    style: TextStyle(
                      color: AppColor.textPrimary,
                      fontSize: 12,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                );
              }).toList(),
              onChanged: onChanged,
            ),
          ),
        ),
      ],
    );
  }
}
