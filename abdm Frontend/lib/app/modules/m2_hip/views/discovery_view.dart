import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:syncfusion_flutter_datagrid/datagrid.dart';
import '../controllers/care_context_controller.dart';
import '../../dashboard/controllers/dashboard_controller.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';
import '../../../data/model/response/m2/care_context_model.dart';

class DiscoveryView extends GetView<CareContextController> {
  const DiscoveryView({Key? key}) : super(key: key);

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
                    child: const Icon(Icons.local_hospital_outlined, color: Colors.white, size: 28),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Milestone M2: HIP Care Contexts & Discovery", style: fontBold.copyWith(fontSize: 18)),
                        const SizedBox(height: 4),
                        Text(
                          "Discover patient by ABHA ID and link OPD visits, diagnostic reports, and IPD records.",
                          style: fontSmall,
                        ),
                      ],
                    ),
                  ),
                  ElevatedButton.icon(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColor.primary,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                    ),
                    onPressed: () {
                      Get.find<DashboardController>().changeTab(6);
                    },
                    icon: const Icon(Icons.add_link, color: Colors.white),
                    label: Text('Link New Context', style: fontMedium.copyWith(color: Colors.white, fontWeight: FontWeight.bold)),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // Content List Section
            Expanded(
              child: Obx(() {
                if (controller.isLoading.value) {
                  return const Center(child: CircularProgressIndicator(color: AppColor.accent));
                }
                if (controller.careContexts.isEmpty) {
                  return Center(
                    child: Container(
                      padding: const EdgeInsets.all(30),
                      decoration: glassDecoration(),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          const Icon(Icons.folder_open, size: 60, color: AppColor.textSecondary),
                          const SizedBox(height: 16),
                          Text("No Linked Care Contexts Found", style: fontBold),
                          const SizedBox(height: 6),
                          Text("Click 'Link New Context' to connect OPD/IPD records to patient ABHA.", style: fontSmall),
                        ],
                      ),
                    ),
                  );
                }

                final dataSource = CareContextDataSource(careContexts: controller.careContexts);
                
                return Container(
                  decoration: BoxDecoration(
                    color: AppColor.surface,
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: AppColor.border),
                  ),
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(12),
                    child: SfDataGrid(
                      source: dataSource,
                      columnWidthMode: ColumnWidthMode.fill,
                      gridLinesVisibility: GridLinesVisibility.horizontal,
                      headerGridLinesVisibility: GridLinesVisibility.horizontal,
                      headerRowHeight: 52,
                      rowHeight: 58,
                      columns: <GridColumn>[
                        GridColumn(
                          columnName: 'patientName',
                          width: 160,
                          label: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            alignment: Alignment.centerLeft,
                            child: Text('Patient Name',
                                style: fontBold.copyWith(fontSize: 13, color: AppColor.textPrimary)),
                          ),
                        ),
                        GridColumn(
                          columnName: 'abhaAddress',
                          width: 180,
                          label: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            alignment: Alignment.centerLeft,
                            child: Text('ABHA Address',
                                style: fontBold.copyWith(fontSize: 13, color: AppColor.textPrimary)),
                          ),
                        ),
                        GridColumn(
                          columnName: 'description',
                          width: 320,
                          label: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            alignment: Alignment.centerLeft,
                            child: Text('Record / Consent Info',
                                style: fontBold.copyWith(fontSize: 13, color: AppColor.textPrimary)),
                          ),
                        ),
                        GridColumn(
                          columnName: 'referenceNumber',
                          width: 140,
                          label: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            alignment: Alignment.centerLeft,
                            child: Text('Reference No.',
                                style: fontBold.copyWith(fontSize: 13, color: AppColor.textPrimary)),
                          ),
                        ),
                        GridColumn(
                          columnName: 'date',
                          width: 110,
                          label: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            alignment: Alignment.centerLeft,
                            child: Text('Date',
                                style: fontBold.copyWith(fontSize: 13, color: AppColor.textPrimary)),
                          ),
                        ),
                        GridColumn(
                          columnName: 'type',
                          width: 190,
                          label: Container(
                            padding: const EdgeInsets.symmetric(horizontal: 16.0),
                            alignment: Alignment.centerLeft,
                            child: Text('Type',
                                style: fontBold.copyWith(fontSize: 13, color: AppColor.textPrimary)),
                          ),
                        ),
                      ],
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
}

class ParsedDisplay {
  final String patientName;
  final String abhaAddress;
  final String description;

  ParsedDisplay({
    required this.patientName,
    required this.abhaAddress,
    required this.description,
  });

  factory ParsedDisplay.parse(String display) {
    try {
      if (display.contains(' - ')) {
        final parts = display.split(' - ');
        if (parts.length >= 3) {
          final description = "${parts[0]} (${parts[1]})";
          final patientPart = parts[2];
          String name = patientPart;
          String abha = '';
          if (patientPart.contains('(') && patientPart.contains(')')) {
            final start = patientPart.indexOf('(');
            final end = patientPart.indexOf(')');
            name = patientPart.substring(0, start).trim();
            abha = patientPart.substring(start + 1, end).trim();
          }
          return ParsedDisplay(patientName: name, abhaAddress: abha, description: description);
        } else if (parts.length == 2) {
          final description = parts[0];
          final patientPart = parts[1];
          String name = patientPart;
          String abha = '';
          if (patientPart.contains('(') && patientPart.contains(')')) {
            final start = patientPart.indexOf('(');
            final end = patientPart.indexOf(')');
            name = patientPart.substring(0, start).trim();
            abha = patientPart.substring(start + 1, end).trim();
          }
          return ParsedDisplay(patientName: name, abhaAddress: abha, description: description);
        }
      }
      
      // Fallback
      String name = 'N/A';
      String abha = '';
      if (display.contains('(') && display.contains(')')) {
        final start = display.indexOf('(');
        final end = display.indexOf(')');
        abha = display.substring(start + 1, end).trim();
        name = display.substring(0, start).trim();
        if (name.contains('-')) {
          name = name.split('-').last.trim();
        }
      }
      return ParsedDisplay(patientName: name, abhaAddress: abha, description: display);
    } catch (_) {
      return ParsedDisplay(patientName: 'N/A', abhaAddress: '', description: display);
    }
  }
}

class CareContextDataSource extends DataGridSource {
  CareContextDataSource({required List<CareContextModel> careContexts}) {
    _dataGridRows = careContexts.map<DataGridRow>((e) {
      final parsed = ParsedDisplay.parse(e.display);
      return DataGridRow(cells: [
        DataGridCell<String>(columnName: 'patientName', value: parsed.patientName),
        DataGridCell<String>(columnName: 'abhaAddress', value: parsed.abhaAddress),
        DataGridCell<String>(columnName: 'description', value: parsed.description),
        DataGridCell<String>(columnName: 'referenceNumber', value: e.referenceNumber),
        DataGridCell<String>(columnName: 'date', value: e.date),
        DataGridCell<String>(columnName: 'type', value: e.type),
      ]);
    }).toList();
  }

  List<DataGridRow> _dataGridRows = [];

  @override
  List<DataGridRow> get rows => _dataGridRows;

  @override
  DataGridRowAdapter buildRow(DataGridRow row) {
    return DataGridRowAdapter(
      cells: row.getCells().map<Widget>((dataGridCell) {
        if (dataGridCell.columnName == 'type') {
          return Container(
            alignment: Alignment.centerLeft,
            padding: const EdgeInsets.symmetric(horizontal: 16.0),
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
              decoration: BoxDecoration(
                color: AppColor.info.withOpacity(0.12),
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(
                dataGridCell.value.toString(),
                style: fontSmall.copyWith(
                  fontSize: 11,
                  fontWeight: FontWeight.bold,
                  color: AppColor.info,
                ),
              ),
            ),
          );
        } else if (dataGridCell.columnName == 'patientName') {
          return Container(
            alignment: Alignment.centerLeft,
            padding: const EdgeInsets.symmetric(horizontal: 16.0),
            child: Text(
              dataGridCell.value.toString(),
              style: fontMedium.copyWith(
                fontSize: 13,
                fontWeight: FontWeight.bold,
                color: AppColor.textPrimary,
              ),
              overflow: TextOverflow.ellipsis,
            ),
          );
        } else if (dataGridCell.columnName == 'abhaAddress') {
          return Container(
            alignment: Alignment.centerLeft,
            padding: const EdgeInsets.symmetric(horizontal: 16.0),
            child: Text(
              dataGridCell.value.toString(),
              style: fontMedium.copyWith(
                fontSize: 12,
                color: const Color(0xFF0F766E),
                fontWeight: FontWeight.w600,
              ),
              overflow: TextOverflow.ellipsis,
            ),
          );
        } else if (dataGridCell.columnName == 'referenceNumber') {
          return Container(
            alignment: Alignment.centerLeft,
            padding: const EdgeInsets.symmetric(horizontal: 16.0),
            child: Text(
              dataGridCell.value.toString(),
              style: fontMedium.copyWith(
                fontSize: 12.5,
                fontWeight: FontWeight.w600,
                color: AppColor.accent,
              ),
              overflow: TextOverflow.ellipsis,
            ),
          );
        }

        return Container(
          alignment: Alignment.centerLeft,
          padding: const EdgeInsets.symmetric(horizontal: 16.0),
          child: Text(
            dataGridCell.value.toString(),
            style: fontRegular.copyWith(
              fontSize: 12.5,
              color: AppColor.textSecondary,
            ),
            overflow: TextOverflow.ellipsis,
          ),
        );
      }).toList(),
    );
  }
}
