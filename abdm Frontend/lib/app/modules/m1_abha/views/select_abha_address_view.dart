import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/abha_creation_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class SelectAbhaAddressView extends GetView<AbhaCreationController> {
  SelectAbhaAddressView({Key? key}) : super(key: key);

  final TextEditingController _customHandleController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      appBar: AppBar(
        title: const Text('Choose ABHA Address (M1)'),
        backgroundColor: AppColor.surface,
        elevation: 0,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Container(
          padding: const EdgeInsets.all(28),
          decoration: glassDecoration(),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Step 3: Select or Customize ABHA Address', style: fontBold.copyWith(fontSize: 18)),
              const SizedBox(height: 8),
              Text(
                'Your ABHA Address is your unique, lifelong digital health identifier username (e.g., username@abdm).',
                style: fontSmall,
              ),
              const SizedBox(height: 28),

              Text('Suggested Available Handles:', style: fontMedium.copyWith(color: AppColor.accent)),
              const SizedBox(height: 14),
              Obx(() {
                if (controller.isFetchingSuggestions.value) {
                  return const Center(child: CircularProgressIndicator(color: AppColor.accent));
                }
                final list = controller.suggestionsList;
                if (list.isEmpty) {
                  return Text('No suggestions available. Enter custom handle below.', style: fontSmall);
                }
                return Wrap(
                  spacing: 12,
                  runSpacing: 12,
                  children: list.map((handle) {
                    bool isSelected = controller.selectedAbhaHandle.value == handle;
                    return ChoiceChip(
                      label: Text(handle, style: fontMedium.copyWith(color: isSelected ? Colors.white : AppColor.textSecondary)),
                      selected: isSelected,
                      selectedColor: AppColor.primary,
                      backgroundColor: AppColor.background,
                      onSelected: (selected) {
                        if (selected) {
                          controller.selectedAbhaHandle.value = handle;
                          _customHandleController.text = handle;
                        }
                      },
                    );
                  }).toList(),
                );
              }),
              const SizedBox(height: 32),

              TextField(
                controller: _customHandleController,
                style: fontMedium,
                decoration: InputDecoration(
                  labelText: 'Custom ABHA Address',
                  hintText: 'e.g., username@abdm',
                  filled: true,
                  fillColor: AppColor.background,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide(color: AppColor.border),
                  ),
                  prefixIcon: const Icon(Icons.alternate_email, color: AppColor.accent),
                ),
                onChanged: (val) {
                  controller.selectedAbhaHandle.value = val;
                },
              ),
              const SizedBox(height: 36),

              Obx(() => SizedBox(
                    width: double.infinity,
                    height: 52,
                    child: Container(
                      decoration: BoxDecoration(
                        gradient: AppColor.primaryGradient,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: ElevatedButton(
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.transparent,
                          shadowColor: Colors.transparent,
                        ),
                        onPressed: controller.isLoading.value
                            ? null
                            : () => controller.finalizeAbhaCreation(_customHandleController.text),
                        child: controller.isLoading.value
                            ? const CircularProgressIndicator(color: Colors.white)
                            : Text('Confirm ABHA Address & Generate Card', style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold)),
                      ),
                    ),
                  )),
            ],
          ),
        ),
      ),
    );
  }
}
