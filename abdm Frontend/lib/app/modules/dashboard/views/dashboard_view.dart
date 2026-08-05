import 'package:abdm_frontend/app/routes/app_paths.dart' show Routes;
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../controllers/dashboard_controller.dart';
import '../widgets/dashboard_action_grid.dart';
import '../../m1_abha/views/create_abha_view.dart';
import '../../m2_hip/views/discovery_view.dart';
import '../../m3_hiu/views/consent_request_view.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class DashboardView extends GetView<DashboardController> {
  DashboardView({Key? key}) : super(key: key);

  late final List<Widget> _pages = [
    DashboardActionGrid(onNavigateTab: controller.changeTab),
    CreateAbhaView(),
    const DiscoveryView(),
    const ConsentRequestView(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColor.background,
      body: SafeArea(
        child: Column(
          children: [
            // Top Web Portal Header Bar (Overflow-Proof)
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
              decoration: BoxDecoration(
                color: AppColor.surface,
                border: Border(
                    bottom:
                        BorderSide(color: AppColor.border.withOpacity(0.5))),
              ),
              child: SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    // Logo & Brand Name
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(8),
                          decoration: BoxDecoration(
                            gradient: AppColor.primaryGradient,
                            borderRadius: BorderRadius.circular(10),
                          ),
                          child: const Icon(Icons.health_and_safety,
                              color: Colors.white, size: 22),
                        ),
                        const SizedBox(width: 12),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text("ABDM Gateway Portal",
                                style: fontBold.copyWith(fontSize: 16)),
                            Text("Ayushman Bharat Digital Mission • Sandbox v3",
                                style: fontSmall.copyWith(fontSize: 12)),
                          ],
                        ),
                      ],
                    ),
                    const SizedBox(width: 30),

                    // Connection & Facility Badges
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 5),
                          decoration: BoxDecoration(
                            color: AppColor.success.withOpacity(0.15),
                            borderRadius: BorderRadius.circular(20),
                            border: Border.all(
                                color: AppColor.success.withOpacity(0.3)),
                          ),
                          child: Row(
                            children: [
                              Container(
                                width: 8,
                                height: 8,
                                decoration: const BoxDecoration(
                                  color: AppColor.success,
                                  shape: BoxShape.circle,
                                ),
                              ),
                              const SizedBox(width: 6),
                              Text(
                                "sbx.wati.digital",
                                style: fontSmall.copyWith(
                                    color: AppColor.success,
                                    fontWeight: FontWeight.bold,
                                    fontSize: 11),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 10),
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 10, vertical: 5),
                          decoration: BoxDecoration(
                            color: AppColor.primary.withOpacity(0.15),
                            borderRadius: BorderRadius.circular(20),
                            border: Border.all(
                                color: AppColor.primary.withOpacity(0.3)),
                          ),
                          child: Text(
                            "HIP ID: MIDHA HOSPITAL",
                            style: fontSmall.copyWith(
                                color: AppColor.accent,
                                fontWeight: FontWeight.bold,
                                fontSize: 11),
                          ),
                        ),
                        const SizedBox(width: 12),
                        OutlinedButton.icon(
                          style: OutlinedButton.styleFrom(
                            side: const BorderSide(color: AppColor.accent),
                            shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(20)),
                            padding: const EdgeInsets.symmetric(
                                horizontal: 12, vertical: 6),
                          ),
                          onPressed: () => Get.toNamed(Routes.GALAXY_ABHA),
                          icon: const Icon(Icons.open_in_new,
                              color: AppColor.accent, size: 14),
                          label: Text("Galaxy Health ABHA Page",
                              style: fontSmall.copyWith(
                                  color: AppColor.accent,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 11)),
                        ),
                      ],
                    )
                  ],
                ),
              ),
            ),

            // Main Web Layout (Sidebar + Content Area)
            Expanded(
              child: LayoutBuilder(
                builder: (context, constraints) {
                  bool isWide = constraints.maxWidth >= 768;

                  if (isWide) {
                    // Desktop Layout with Left Sidebar
                    return Row(
                      children: [
                        // Left Sidebar
                        Container(
                          width: 240,
                          decoration: BoxDecoration(
                            color: AppColor.surface.withOpacity(0.5),
                            border: Border(
                                right: BorderSide(
                                    color: AppColor.border.withOpacity(0.5))),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const SizedBox(height: 16),
                              Padding(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 16, vertical: 8),
                                child: Text("NAVIGATION MODULES",
                                    style: fontSmall.copyWith(
                                        fontSize: 10,
                                        fontWeight: FontWeight.bold)),
                              ),
                              Obx(() => _buildSidebarTile(
                                    index: 0,
                                    icon: Icons.grid_view_outlined,
                                    activeIcon: Icons.grid_view,
                                    title: "Dashboard Overview",
                                    subtitle: "5 Quick Action Hub",
                                  )),
                              Obx(() => _buildSidebarTile(
                                    index: 1,
                                    icon: Icons.badge_outlined,
                                    activeIcon: Icons.badge,
                                    title: "M1: ABHA Creation",
                                    subtitle: "Aadhaar OTP & ABHA Card",
                                  )),
                              Obx(() => _buildSidebarTile(
                                    index: 2,
                                    icon: Icons.local_hospital_outlined,
                                    activeIcon: Icons.local_hospital,
                                    title: "M2: HIP Records",
                                    subtitle: "Care Contexts & Link Status",
                                  )),
                              Obx(() => _buildSidebarTile(
                                    index: 3,
                                    icon: Icons.folder_shared_outlined,
                                    activeIcon: Icons.folder_shared,
                                    title: "M3: HIU Records",
                                    subtitle: "Consents & FHIR Data",
                                  )),
                            ],
                          ),
                        ),

                        // Right Content Page Area
                        Expanded(
                          child:
                              Obx(() => _pages[controller.currentIndex.value]),
                        ),
                      ],
                    );
                  }

                  // Mobile Layout
                  return Obx(() => _pages[controller.currentIndex.value]);
                },
              ),
            ),
          ],
        ),
      ),
      bottomNavigationBar: LayoutBuilder(
        builder: (context, constraints) {
          if (MediaQuery.of(context).size.width >= 768) {
            return const SizedBox.shrink();
          }
          return Obx(
            () => BottomNavigationBar(
              currentIndex: controller.currentIndex.value,
              onTap: controller.changeTab,
              backgroundColor: AppColor.surface,
              selectedItemColor: AppColor.accent,
              unselectedItemColor: AppColor.textSecondary,
              items: const [
                BottomNavigationBarItem(
                  icon: Icon(Icons.grid_view),
                  label: 'Dashboard',
                ),
                BottomNavigationBarItem(
                  icon: Icon(Icons.badge),
                  label: 'M1: ABHA',
                ),
                BottomNavigationBarItem(
                  icon: Icon(Icons.local_hospital),
                  label: 'M2: HIP',
                ),
                BottomNavigationBarItem(
                  icon: Icon(Icons.folder_shared),
                  label: 'M3: HIU',
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildSidebarTile({
    required int index,
    required IconData icon,
    required IconData activeIcon,
    required String title,
    required String subtitle,
  }) {
    bool isSelected = controller.currentIndex.value == index;
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      decoration: BoxDecoration(
        color:
            isSelected ? AppColor.primary.withOpacity(0.2) : Colors.transparent,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: isSelected
              ? AppColor.primary.withOpacity(0.5)
              : Colors.transparent,
        ),
      ),
      child: ListTile(
        onTap: () => controller.changeTab(index),
        leading: Icon(
          isSelected ? activeIcon : icon,
          color: isSelected ? AppColor.accent : AppColor.textSecondary,
        ),
        title: Text(
          title,
          style: fontMedium.copyWith(
            fontSize: 13,
            fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
            color: isSelected ? AppColor.primary : AppColor.textSecondary,
          ),
        ),
        subtitle: Text(
          subtitle,
          style: fontSmall.copyWith(fontSize: 10),
        ),
      ),
    );
  }
}
