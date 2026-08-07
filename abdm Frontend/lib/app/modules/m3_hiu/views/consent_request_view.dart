import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:syncfusion_flutter_datagrid/datagrid.dart';
import 'package:syncfusion_flutter_core/theme.dart';
import 'package:abdm_frontend/app/routes/app_paths.dart' show Routes;
import '../controllers/health_record_controller.dart';
import '../../../data/repository/m3/hiu_health_record_repo.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';
import '../../m2_hip/controllers/patient_registry_controller.dart';

class ConsentRequestView extends GetView<HealthRecordController> {
  const ConsentRequestView({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header Banner
            Container(
              padding: const EdgeInsets.all(20),
              decoration: glassDecoration(),
              child: Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      gradient: AppColor.primaryGradient,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Icon(Icons.folder_shared_outlined,
                        color: Colors.white, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Milestone M3: HIU Consents & Encrypted FHIR Data",
                            style: fontBold.copyWith(fontSize: 18)),
                        const SizedBox(height: 4),
                        Text(
                          "Initiate patient consent requests, track status (Requested / Granted / Expired), and view FHIR records.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton.icon(
                    onPressed: () => _showInitiateConsentDialog(context),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColor.primary,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(
                          horizontal: 16, vertical: 12),
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10)),
                    ),
                    icon:
                        const Icon(Icons.add_circle_outline_rounded, size: 18),
                    label: const Text("New Consent Request",
                        style: TextStyle(fontWeight: FontWeight.bold)),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // Toolbar with Refresh
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Obx(() => Text(
                      'Consent Requests (${controller.consentRequests.length})',
                      style: fontBold.copyWith(fontSize: 16),
                    )),
                Obx(() => controller.isLoadingConsents.value
                    ? const SizedBox(
                        width: 24,
                        height: 24,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: AppColor.primary))
                    : IconButton(
                        icon: const Icon(Icons.refresh_rounded),
                        color: AppColor.textSecondary,
                        onPressed: () => controller.fetchConsentRequests(),
                      )),
              ],
            ),
            const SizedBox(height: 12),

            // Consents List Section
            Expanded(
              child: Obx(() {
                if (controller.isLoadingConsents.value &&
                    controller.consentRequests.isEmpty) {
                  return const Center(
                      child:
                          CircularProgressIndicator(color: AppColor.primary));
                }

                if (controller.consentRequests.isEmpty) {
                  return Center(
                    child: Container(
                      padding: const EdgeInsets.all(30),
                      decoration: glassDecoration(),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Icon(Icons.privacy_tip_outlined,
                              size: 60, color: AppColor.textSecondary),
                          const SizedBox(height: 16),
                          Text("No Consent Requests Found", style: fontBold),
                          const SizedBox(height: 6),
                          Text(
                            "Click 'New Consent Request' above to request health records from patient.",
                            style: fontSmall,
                            textAlign: TextAlign.center,
                          ),
                        ],
                      ),
                    ),
                  );
                }

                final dataSource = ConsentRequestDataSource(
                  consentRequests: controller.consentRequests,
                  patients: controller.patients,
                  onViewRecords: (item) {
                    controller.fetchAndDecryptRecords(item);
                    Get.toNamed(Routes.M3_HEALTH_RECORDS);
                  },
                );

                return Container(
                  decoration: BoxDecoration(
                    color: AppColor.surface,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: AppColor.border),
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(12),
                    child: SfDataGridTheme(
                      data: SfDataGridThemeData(
                        headerColor:
                            AppColor.primary, // Primary header color
                      ),
                      child: SfDataGrid(
                        source: dataSource,
                        columnWidthMode: ColumnWidthMode.fill,
                        gridLinesVisibility: GridLinesVisibility.horizontal,
                        headerGridLinesVisibility:
                            GridLinesVisibility.horizontal,
                        headerRowHeight: 52,
                        rowHeight: 90,
                        columns: <GridColumn>[
                          GridColumn(
                            columnName: 'srNo',
                            width: 60,
                            label: Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 12.0),
                              alignment: Alignment.center,
                              child: const Text('SR#',
                                  style: TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 13)),
                            ),
                          ),
                          GridColumn(
                            columnName: 'patient',
                            width: 220,
                            label: Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 12.0),
                              alignment: Alignment.centerLeft,
                              child: const Text('PATIENT',
                                  style: TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 13)),
                            ),
                          ),
                          GridColumn(
                            columnName: 'requestHiType',
                            width: 240,
                            label: Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 12.0),
                              alignment: Alignment.centerLeft,
                              child: const Text('REQUEST HI-TYPE',
                                  style: TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 13)),
                            ),
                          ),
                          GridColumn(
                            columnName: 'requestStatus',
                            width: 240,
                            label: Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 12.0),
                              alignment: Alignment.centerLeft,
                              child: const Text('REQUEST STATUS',
                                  style: TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 13)),
                            ),
                          ),
                          GridColumn(
                            columnName: 'grantedHiType',
                            width: 240,
                            label: Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 12.0),
                              alignment: Alignment.centerLeft,
                              child: const Text('GRANTED HI-TYPE',
                                  style: TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 13)),
                            ),
                          ),
                          GridColumn(
                            columnName: 'requested',
                            width: 240,
                            label: Container(
                              padding:
                                  const EdgeInsets.symmetric(horizontal: 12.0),
                              alignment: Alignment.centerLeft,
                              child: const Text('REQUESTED',
                                  style: TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                      fontSize: 13)),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                );
              }),
            ),
          ],
        ),
      ),
    );
  }

  void _showInitiateConsentDialog(BuildContext context) {
    final abhaCtrl =
        TextEditingController(text: controller.abhaAddressInput.value);

    DateTime selectedFrom = DateTime.now().subtract(const Duration(days: 30));
    DateTime selectedTo = DateTime.now();

    final fromDateCtrl = TextEditingController(
        text:
            "${selectedFrom.day.toString().padLeft(2, '0')}/${selectedFrom.month.toString().padLeft(2, '0')}/${selectedFrom.year}");
    final toDateCtrl = TextEditingController(
        text:
            "${selectedTo.day.toString().padLeft(2, '0')}/${selectedTo.month.toString().padLeft(2, '0')}/${selectedTo.year}");

    Get.dialog(
      Dialog(
        backgroundColor: AppColor.surface,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Container(
          width: 520,
          padding: const EdgeInsets.all(24),
          child: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: AppColor.primary.withOpacity(0.15),
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: const Icon(Icons.add_circle_outline_rounded,
                          color: AppColor.primary, size: 20),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('Initiate Consent Request (M3 HIU)',
                              style: TextStyle(
                                  color: AppColor.textPrimary,
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold)),
                          Text(
                              'Request health records consent from patient via ABDM Gateway',
                              style: TextStyle(
                                  color: AppColor.textSecondary, fontSize: 12)),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),

                // Select from Registered Patients dropdown helper
                if (controller.patients.isNotEmpty) ...[
                  Text('Select from Registered Patients',
                      style: TextStyle(
                          color: AppColor.textPrimary,
                          fontSize: 13,
                          fontWeight: FontWeight.w600)),
                  const SizedBox(height: 6),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 14),
                    decoration: BoxDecoration(
                      color: AppColor.background,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppColor.border),
                    ),
                    child: DropdownButtonHideUnderline(
                      child: DropdownButton<String>(
                        hint: const Text('Choose a patient...'),
                        isExpanded: true,
                        dropdownColor: AppColor.surface,
                        style: TextStyle(
                            color: AppColor.textPrimary, fontSize: 13),
                        items: controller.patients.map((p) {
                          return DropdownMenuItem<String>(
                            value: p.abhaAddress,
                            child: Text('${p.name} (${p.abhaAddress})'),
                          );
                        }).toList(),
                        onChanged: (val) {
                          if (val != null) {
                            abhaCtrl.text = val;
                          }
                        },
                      ),
                    ),
                  ),
                  const SizedBox(height: 16),
                ],

                // ABHA Address Input
                Text('Patient ABHA Address',
                    style: TextStyle(
                        color: AppColor.textPrimary,
                        fontSize: 13,
                        fontWeight: FontWeight.w600)),
                const SizedBox(height: 6),
                TextField(
                  controller: abhaCtrl,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                  decoration: InputDecoration(
                    hintText: 'e.g. user_40893@sbx',
                    hintStyle: TextStyle(color: AppColor.textSecondary),
                    filled: true,
                    fillColor: AppColor.background,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(10),
                      borderSide: BorderSide(color: AppColor.border),
                    ),
                    contentPadding: const EdgeInsets.symmetric(
                        horizontal: 14, vertical: 12),
                  ),
                ),
                const SizedBox(height: 16),

                // Date Range Picker Section
                Row(
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('From Date',
                              style: TextStyle(
                                  color: AppColor.textPrimary,
                                  fontSize: 13,
                                  fontWeight: FontWeight.w600)),
                          const SizedBox(height: 6),
                          TextField(
                            controller: fromDateCtrl,
                            readOnly: true,
                            style: TextStyle(
                                color: AppColor.textPrimary, fontSize: 13),
                            onTap: () async {
                              final picked = await showDatePicker(
                                context: context,
                                initialDate: selectedFrom,
                                firstDate: DateTime(2000),
                                lastDate: DateTime.now(),
                                builder: (context, child) {
                                  return Theme(
                                    data: Theme.of(context).copyWith(
                                      colorScheme: const ColorScheme.dark(
                                        primary: AppColor.primary,
                                        onPrimary: Colors.white,
                                        surface: Color(0xFF1F2937),
                                        onSurface: Colors.white,
                                      ),
                                    ),
                                    child: child!,
                                  );
                                },
                              );
                              if (picked != null) {
                                selectedFrom = picked;
                                fromDateCtrl.text =
                                    "${picked.day.toString().padLeft(2, '0')}/${picked.month.toString().padLeft(2, '0')}/${picked.year}";
                              }
                            },
                            decoration: InputDecoration(
                              prefixIcon: const Icon(
                                  Icons.calendar_today_rounded,
                                  size: 16,
                                  color: AppColor.primary),
                              filled: true,
                              fillColor: AppColor.background,
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                                borderSide: BorderSide(color: AppColor.border),
                              ),
                              contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 14, vertical: 12),
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('To Date',
                              style: TextStyle(
                                  color: AppColor.textPrimary,
                                  fontSize: 13,
                                  fontWeight: FontWeight.w600)),
                          const SizedBox(height: 6),
                          TextField(
                            controller: toDateCtrl,
                            readOnly: true,
                            style: TextStyle(
                                color: AppColor.textPrimary, fontSize: 13),
                            onTap: () async {
                              final picked = await showDatePicker(
                                context: context,
                                initialDate: selectedTo,
                                firstDate: DateTime(2000),
                                lastDate:
                                    DateTime.now().add(const Duration(days: 1)),
                                builder: (context, child) {
                                  return Theme(
                                    data: Theme.of(context).copyWith(
                                      colorScheme: const ColorScheme.dark(
                                        primary: AppColor.primary,
                                        onPrimary: Colors.white,
                                        surface: Color(0xFF1F2937),
                                        onSurface: Colors.white,
                                      ),
                                    ),
                                    child: child!,
                                  );
                                },
                              );
                              if (picked != null) {
                                selectedTo = picked;
                                toDateCtrl.text =
                                    "${picked.day.toString().padLeft(2, '0')}/${picked.month.toString().padLeft(2, '0')}/${picked.year}";
                              }
                            },
                            decoration: InputDecoration(
                              prefixIcon: const Icon(
                                  Icons.calendar_today_rounded,
                                  size: 16,
                                  color: AppColor.primary),
                              filled: true,
                              fillColor: AppColor.background,
                              border: OutlineInputBorder(
                                borderRadius: BorderRadius.circular(10),
                                borderSide: BorderSide(color: AppColor.border),
                              ),
                              contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 14, vertical: 12),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 16),

                // Purpose Code Selection
                Text('Purpose of Request',
                    style: TextStyle(
                        color: AppColor.textPrimary,
                        fontSize: 13,
                        fontWeight: FontWeight.w600)),
                const SizedBox(height: 6),
                Obx(() => Container(
                      padding: const EdgeInsets.symmetric(horizontal: 14),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColor.border),
                      ),
                      child: DropdownButtonHideUnderline(
                        child: DropdownButton<String>(
                          value: controller.selectedPurposeCode.value,
                          isExpanded: true,
                          dropdownColor: AppColor.surface,
                          style: TextStyle(
                              color: AppColor.textPrimary, fontSize: 13),
                          items: controller.purposeCodeList.map((p) {
                            return DropdownMenuItem<String>(
                              value: p['code'],
                              child: Text('${p['label']} (${p['code']})'),
                            );
                          }).toList(),
                          onChanged: (val) {
                            if (val != null)
                              controller.selectedPurposeCode.value = val;
                          },
                        ),
                      ),
                    )),
                const SizedBox(height: 16),

                // HI Types Multi-Select Checkboxes
                Text('Health Information (HI) Types Requested',
                    style: TextStyle(
                        color: AppColor.textPrimary,
                        fontSize: 13,
                        fontWeight: FontWeight.w600)),
                const SizedBox(height: 8),
                Obx(() => Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: controller.availableHiTypes.map((hi) {
                        bool isSelected =
                            controller.selectedHiTypes.contains(hi);
                        return FilterChip(
                          label: Text(hi),
                          selected: isSelected,
                          onSelected: (_) => controller.toggleHiType(hi),
                          selectedColor:
                              AppColor.primary.withOpacity(0.2),
                          checkmarkColor: AppColor.primary,
                          labelStyle: TextStyle(
                            color: isSelected
                                ? AppColor.primary
                                : AppColor.textPrimary,
                            fontWeight: isSelected
                                ? FontWeight.bold
                                : FontWeight.normal,
                            fontSize: 12,
                          ),
                          backgroundColor: AppColor.background,
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                            side: BorderSide(
                              color: isSelected
                                  ? AppColor.primary
                                  : AppColor.border,
                            ),
                          ),
                        );
                      }).toList(),
                    )),
                const SizedBox(height: 24),

                // Submit Buttons
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    TextButton(
                      onPressed: () => Get.back(),
                      child: Text('Cancel',
                          style: TextStyle(color: AppColor.textSecondary)),
                    ),
                    const SizedBox(width: 12),
                    Obx(() => controller.isSubmittingConsent.value
                        ? const CircularProgressIndicator(
                            color: AppColor.primary)
                        : ElevatedButton.icon(
                            onPressed: () {
                              final abha = abhaCtrl.text.trim();
                              if (abha.isEmpty) {
                                Get.snackbar(
                                    'Error', 'ABHA Address cannot be empty.');
                                return;
                              }

                              final fromIso = DateTime(
                                      selectedFrom.year,
                                      selectedFrom.month,
                                      selectedFrom.day,
                                      0,
                                      0,
                                      0)
                                  .toUtc()
                                  .toIso8601String();

                              DateTime toDateVal = DateTime(selectedTo.year,
                                  selectedTo.month, selectedTo.day, 23, 59, 59);
                              if (toDateVal.isAfter(DateTime.now())) {
                                toDateVal = DateTime.now();
                              }
                              final toIso = toDateVal.toUtc().toIso8601String();
                              final eraseIso = toDateVal
                                  .add(const Duration(days: 365))
                                  .toUtc()
                                  .toIso8601String();

                              Get.back();
                              controller.submitConsentRequest(
                                abhaAddress: abha,
                                fromDate: fromIso,
                                toDate: toIso,
                                eraseAt: eraseIso,
                              );
                            },
                            style: ElevatedButton.styleFrom(
                              backgroundColor: AppColor.primary,
                              foregroundColor: Colors.white,
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 18, vertical: 12),
                              shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(10)),
                            ),
                            icon: const Icon(Icons.send_rounded, size: 16),
                            label: const Text('Send Consent Request',
                                style: TextStyle(fontWeight: FontWeight.bold)),
                          )),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class ConsentRequestDataSource extends DataGridSource {
  ConsentRequestDataSource({
    required List<HiuConsentRequestModel> consentRequests,
    required List<PatientRegistryModel> patients,
    required Function(HiuConsentRequestModel) onViewRecords,
  }) {
    _dataGridRows = consentRequests.map<DataGridRow>((e) {
      String patientName = "Saurav Kumar";
      try {
        final p = patients
            .firstWhere((element) => element.abhaAddress == e.abhaAddress);
        patientName = p.name;
      } catch (_) {
        patientName =
            e.abhaAddress.split('@').first.replaceAll(RegExp(r'[._-]'), ' ');
        patientName = patientName
            .split(' ')
            .map((word) => word.isEmpty
                ? ''
                : '${word[0].toUpperCase()}${word.substring(1)}')
            .join(' ');
      }

      return DataGridRow(cells: [
        DataGridCell<HiuConsentRequestModel>(columnName: 'item', value: e),
        DataGridCell<String>(columnName: 'patientName', value: patientName),
      ]);
    }).toList();

    _onViewRecords = onViewRecords;
  }

  List<DataGridRow> _dataGridRows = [];
  late Function(HiuConsentRequestModel) _onViewRecords;

  @override
  List<DataGridRow> get rows => _dataGridRows;

  @override
  DataGridRowAdapter buildRow(DataGridRow row) {
    final HiuConsentRequestModel item = row.getCells()[0].value;
    final String patientName = row.getCells()[1].value;
    final int index = _dataGridRows.indexOf(row);

    final statusUpper = item.status.toUpperCase();
    bool isGranted = (statusUpper.contains('GRANT') ||
            statusUpper.contains('NOTIFY') ||
            statusUpper.contains('FETCH') ||
            statusUpper.contains('HEALTH INFORMATION') ||
            statusUpper.contains('ACCEPTED BY GATEWAY')) &&
        !statusUpper.contains('ERROR');
    bool isRequested =
        (statusUpper.contains('REQUEST') || statusUpper.contains('INIT')) &&
            !isGranted;
    bool isExpired = statusUpper.contains('EXPIR') || statusUpper.contains('REVOK');

    final statusColor = isGranted
        ? const Color(0xFF10B981)
        : isRequested
            ? const Color(0xFFF59E0B)
            : isExpired
                ? const Color(0xFF6B7280)
                : const Color(0xFFEF4444);

    String formatDateTimeStr(String isoStr) {
      if (isoStr.isEmpty) return '-';
      try {
        final dt = DateTime.parse(isoStr).toLocal();
        final months = [
          'Jan',
          'Feb',
          'Mar',
          'Apr',
          'May',
          'Jun',
          'Jul',
          'Aug',
          'Sep',
          'Oct',
          'Nov',
          'Dec'
        ];
        final day = dt.day.toString().padLeft(2, '0');
        final month = months[dt.month - 1];
        final year = dt.year;
        final hourVal =
            dt.hour == 0 ? 12 : (dt.hour > 12 ? dt.hour - 12 : dt.hour);
        final hour = hourVal.toString().padLeft(2, '0');
        final minute = dt.minute.toString().padLeft(2, '0');
        final ampm = dt.hour >= 12 ? 'pm' : 'am';
        return "$day $month $year, $hour:$minute $ampm";
      } catch (_) {
        return isoStr;
      }
    }

    final fromFormatted = formatDateTimeStr(item.fromDate);
    final toFormatted = formatDateTimeStr(item.toDate);

    return DataGridRowAdapter(
      cells: [
        // SR#
        Container(
          alignment: Alignment.center,
          child: Text(
            '${index + 1}.',
            style: TextStyle(
                color: AppColor.textSecondary,
                fontWeight: FontWeight.bold,
                fontSize: 13),
          ),
        ),

        // PATIENT
        Container(
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.symmetric(vertical: 8.0, horizontal: 12.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                patientName,
                style: fontBold.copyWith(
                  color: AppColor.textPrimary,
                  fontSize: 13,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 3),
              Text(
                item.abhaAddress,
                style: fontRegular.copyWith(
                    color: AppColor.textSecondary, fontSize: 10.5),
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 2),
              Text(
                'Req Ref: ${item.clientRequestId}',
                style: fontSmall.copyWith(color: AppColor.accent, fontSize: 9.5),
                overflow: TextOverflow.ellipsis,
              ),
              if (item.consentId.isNotEmpty && item.consentId != 'CONSENT_ON_NOTIFY_RESPONSE') ...[
                const SizedBox(height: 2),
                Text(
                  'Consent ID: ${item.consentId}',
                  style: fontSmall.copyWith(color: const Color(0xFF0F766E), fontSize: 9.5),
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ],
          ),
        ),

        // REQUEST HI-TYPE
        Container(
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.all(8.0),
          child: Wrap(
            spacing: 4,
            runSpacing: 4,
            children: item.hiTypes.map((hi) {
              return Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: AppColor.background,
                  borderRadius: BorderRadius.circular(4),
                  border: Border.all(color: AppColor.border),
                ),
                child: Text(
                  hi,
                  style: fontMedium.copyWith(
                      color: AppColor.textPrimary,
                      fontSize: 9.5,
                      fontWeight: FontWeight.w500),
                ),
              );
            }).toList(),
          ),
        ),

        // REQUEST STATUS
        Container(
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.all(8.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: statusColor.withOpacity(0.12),
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Text(
                  item.status,
                  style: fontBold.copyWith(
                    color: statusColor,
                    fontWeight: FontWeight.bold,
                    fontSize: 9.5,
                  ),
                ),
              ),
              if (isGranted) ...[
                const SizedBox(height: 6),
                ElevatedButton.icon(
                  onPressed: () => _onViewRecords(item),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF10B981),
                    foregroundColor: Colors.white,
                    elevation: 0,
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(4)),
                  ),
                  icon: const Icon(Icons.medical_information_rounded, size: 10),
                  label: const Text('View Records',
                      style: TextStyle(
                          fontSize: 9.5, fontWeight: FontWeight.bold)),
                ),
              ],
            ],
          ),
        ),

        // GRANTED HI-TYPE
        Container(
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.all(8.0),
          child: isGranted
              ? Wrap(
                  spacing: 4,
                  runSpacing: 4,
                  children: (item.grantedHiTypes.isNotEmpty
                          ? item.grantedHiTypes
                          : item.hiTypes)
                      .map((hi) {
                    return Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 3),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(4),
                        border: Border.all(color: AppColor.border),
                      ),
                      child: Text(
                        hi,
                        style: fontMedium.copyWith(
                            color: AppColor.textPrimary,
                            fontSize: 9.5,
                            fontWeight: FontWeight.w500),
                      ),
                    );
                  }).toList(),
                )
              : Text(
                  isExpired ? 'Expired' : 'Pending',
                  style: fontItalic.copyWith(
                      color: AppColor.textSecondary, fontSize: 11),
                ),
        ),

        // REQUESTED
        Container(
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.all(8.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              RichText(
                text: TextSpan(
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 10.5),
                  children: [
                    const TextSpan(
                        text: 'From: ',
                        style: TextStyle(fontWeight: FontWeight.bold)),
                    TextSpan(text: fromFormatted),
                  ],
                ),
              ),
              const SizedBox(height: 4),
              RichText(
                text: TextSpan(
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 10.5),
                  children: [
                    const TextSpan(
                        text: 'To: ',
                        style: TextStyle(fontWeight: FontWeight.bold)),
                    TextSpan(text: toFormatted),
                  ],
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
