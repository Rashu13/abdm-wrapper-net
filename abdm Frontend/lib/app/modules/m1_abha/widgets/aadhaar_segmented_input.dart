import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import '../controllers/abha_creation_controller.dart';
import '../../../../util/style.dart';

class AadhaarSegmentedInput extends StatefulWidget {
  final AbhaCreationController controller;

  const AadhaarSegmentedInput({Key? key, required this.controller})
      : super(key: key);

  @override
  State<AadhaarSegmentedInput> createState() => _AadhaarSegmentedInputState();
}

class _AadhaarSegmentedInputState extends State<AadhaarSegmentedInput> {
  late TextEditingController _part1Controller;
  late TextEditingController _part2Controller;
  late TextEditingController _part3Controller;

  late FocusNode _part1Focus;
  late FocusNode _part2Focus;
  late FocusNode _part3Focus;

  @override
  void initState() {
    super.initState();
    _part1Controller = TextEditingController();
    _part2Controller = TextEditingController();
    _part3Controller = TextEditingController();

    _part1Focus = FocusNode();
    _part2Focus = FocusNode();
    _part3Focus = FocusNode();

    // Populate initial text if controller has existing 12-digit input
    String currentVal = widget.controller.inputNumber.value;
    if (currentVal.length >= 12) {
      _part1Controller.text = currentVal.substring(0, 4);
      _part2Controller.text = currentVal.substring(4, 8);
      _part3Controller.text = currentVal.substring(8, 12);
    }
  }

  @override
  void dispose() {
    _part1Controller.dispose();
    _part2Controller.dispose();
    _part3Controller.dispose();

    _part1Focus.dispose();
    _part2Focus.dispose();
    _part3Focus.dispose();
    super.dispose();
  }

  void _onInputChanged() {
    String combined =
        _part1Controller.text + _part2Controller.text + _part3Controller.text;
    widget.controller.inputNumber.value = combined;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // 3 Segmented Inputs + Mask Toggle Button Row
        Row(
          children: [
            // Segment 1 (First 4 digits)
            Expanded(
              child: _buildSegmentField(
                controller: _part1Controller,
                focusNode: _part1Focus,
                hintText: 'XXXX',
                onChanged: (val) {
                  _onInputChanged();
                  if (val.length == 4) {
                    _part2Focus.requestFocus();
                  }
                },
              ),
            ),
            const SizedBox(width: 10),

            // Segment 2 (Middle 4 digits)
            Expanded(
              child: _buildSegmentField(
                controller: _part2Controller,
                focusNode: _part2Focus,
                hintText: 'XXXX',
                onChanged: (val) {
                  _onInputChanged();
                  if (val.length == 4) {
                    _part3Focus.requestFocus();
                  } else if (val.isEmpty) {
                    _part1Focus.requestFocus();
                  }
                },
              ),
            ),
            const SizedBox(width: 10),

            // Segment 3 (Last 4 digits)
            Expanded(
              child: _buildSegmentField(
                controller: _part3Controller,
                focusNode: _part3Focus,
                hintText: 'XXXX',
                onChanged: (val) {
                  _onInputChanged();
                  if (val.isEmpty) {
                    _part2Focus.requestFocus();
                  }
                },
              ),
            ),
            const SizedBox(width: 12),

            // Eye Icon Masking Toggle Button
            Obx(() => InkWell(
                  onTap: () {
                    widget.controller.isAadhaarObscured.value =
                        !widget.controller.isAadhaarObscured.value;
                  },
                  borderRadius: BorderRadius.circular(14),
                  child: Container(
                    width: 46,
                    height: 46,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(
                        color: widget.controller.isAadhaarObscured.value
                            ? const Color(0xFFCBD5E1)
                            : const Color(0xFF0F4C81),
                        width: 1.5,
                      ),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.03),
                          blurRadius: 6,
                          offset: const Offset(0, 2),
                        ),
                      ],
                    ),
                    child: Icon(
                      widget.controller.isAadhaarObscured.value
                          ? Icons.visibility_off_outlined
                          : Icons.visibility_outlined,
                      color: widget.controller.isAadhaarObscured.value
                          ? const Color(0xFF64748B)
                          : const Color(0xFF0F4C81),
                      size: 21,
                    ),
                  ),
                )),
          ],
        ),
      ],
    );
  }

  Widget _buildSegmentField({
    required TextEditingController controller,
    required FocusNode focusNode,
    required String hintText,
    required ValueChanged<String> onChanged,
  }) {
    return Obx(() {
      bool isObscured = widget.controller.isAadhaarObscured.value;

      return Container(
        height: 46,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(
            color: focusNode.hasFocus
                ? const Color(0xFF0F4C81)
                : const Color(0xFFCBD5E1),
            width: focusNode.hasFocus ? 2.0 : 1.5,
          ),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.02),
              blurRadius: 6,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Center(
          child: TextField(
            controller: controller,
            focusNode: focusNode,
            textAlign: TextAlign.center,
            textAlignVertical: TextAlignVertical.center,
            keyboardType: TextInputType.number,
            maxLength: 4,
            obscureText: isObscured,
            obscuringCharacter: '•',
            inputFormatters: [
              FilteringTextInputFormatter.digitsOnly,
              LengthLimitingTextInputFormatter(4),
            ],
            style: fontBold.copyWith(
              fontSize: 16,
              letterSpacing: 2.0,
              color: const Color(0xFF0F172A),
            ),
            decoration: InputDecoration(
              isDense: true,
              hintText: hintText,
              hintStyle: fontRegular.copyWith(
                fontSize: 14,
                letterSpacing: 2.0,
                color: const Color(0xFF94A3B8),
              ),
              counterText: '',
              border: InputBorder.none,
              enabledBorder: InputBorder.none,
              focusedBorder: InputBorder.none,
              contentPadding: const EdgeInsets.symmetric(vertical: 6),
            ),
            onChanged: onChanged,
          ),
        ),
      );
    });
  }
}
