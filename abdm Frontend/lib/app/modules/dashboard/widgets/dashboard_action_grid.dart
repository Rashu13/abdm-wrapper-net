import 'package:flutter/material.dart';
import 'package:get/get.dart';
import '../../../routes/app_paths.dart';
import '../../../../util/style.dart';

class DashboardActionGrid extends StatelessWidget {
  final Function(int tabIndex) onNavigateTab;

  const DashboardActionGrid({Key? key, required this.onNavigateTab})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      color: const Color(0xFFEBF5FB), // Light blue background matching screenshot
      width: double.infinity,
      height: double.infinity,
      child: SingleChildScrollView(
        padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 32),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Page Header Title
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      "ABDM Gateway Dashboard",
                      style: fontBold.copyWith(
                        fontSize: 22,
                        color: const Color(0xFF0F172A),
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      "Quick access to ABHA management, patient records, consent, and sessions",
                      style: fontRegular.copyWith(
                        fontSize: 14,
                        color: const Color(0xFF64748B),
                      ),
                    ),
                  ],
                ),
                ElevatedButton.icon(
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFF0F4C81),
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 12),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                  ),
                  onPressed: () => Get.toNamed(Routes.GALAXY_ABHA),
                  icon: const Icon(Icons.add, color: Colors.white, size: 18),
                  label: Text(
                    "New ABHA Registration",
                    style: fontMedium.copyWith(
                        color: Colors.white, fontSize: 13),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 28),

            // 5 Action Cards Responsive Grid
            LayoutBuilder(
              builder: (context, constraints) {
                int crossAxisCount = constraints.maxWidth > 900 ? 2 : 1;
                double childAspectRatio = constraints.maxWidth > 1200
                    ? 3.2
                    : (constraints.maxWidth > 900 ? 2.6 : 2.8);

                return GridView(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: crossAxisCount,
                    crossAxisSpacing: 20,
                    mainAxisSpacing: 20,
                    childAspectRatio: childAspectRatio,
                  ),
                  children: [
                    // Card 1: Create ABHA Number & Link
                    _DashboardActionCard(
                      icon: Icons.add_circle_outline,
                      title: "Create ABHA Number & Link",
                      subtitle:
                          "Generate a new ABHA number and link patient records",
                      onTap: () => Get.toNamed(Routes.GALAXY_ABHA),
                    ),

                    // Card 2: Patients List
                    _DashboardActionCard(
                      icon: Icons.people_outline,
                      title: "Patients List",
                      subtitle: "View and manage all registered patients",
                      onTap: () => onNavigateTab(2), // M2 HIP Discovery
                    ),

                    // Card 3: Link Health Records
                    _DashboardActionCard(
                      icon: Icons.link,
                      title: "Link Health Records",
                      subtitle: "Link existing health records to ABHA",
                      onTap: () => onNavigateTab(2), // M2 Care Context
                    ),

                    // Card 4: Consent Management
                    _DashboardActionCard(
                      icon: Icons.shield_outlined,
                      title: "Consent Management",
                      subtitle: "Request and manage patient consent",
                      onTap: () => onNavigateTab(3), // M3 HIU Consents
                    ),

                    // Card 5: Scan & Share OPD Counter
                    _DashboardActionCard(
                      icon: Icons.qr_code_scanner,
                      title: "Scan & Share OPD Engine",
                      subtitle: "Generate OPD Counter QR & manage patient token queue",
                      onTap: () => onNavigateTab(4), // Tab index 4: ScanShareView
                    ),

                    // Card 6: Token History
                    _DashboardActionCard(
                      icon: Icons.history,
                      title: "Token History",
                      subtitle: "View token & session history",
                      onTap: () => _showTokenHistoryModal(context),
                    ),
                  ],
                );
              },
            ),
          ],
        ),
      ),
    );
  }

  // Token History Modal Dialog
  void _showTokenHistoryModal(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => Dialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Container(
          width: 520,
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(10),
                        decoration: BoxDecoration(
                          color: const Color(0xFFF1F5F9),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: const Icon(Icons.history,
                            color: Color(0xFF0F4C81), size: 22),
                      ),
                      const SizedBox(width: 12),
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            "ABDM Token & Session History",
                            style: fontBold.copyWith(
                                fontSize: 16, color: const Color(0xFF0F172A)),
                          ),
                          Text(
                            "Gateway Authentication Status",
                            style: fontRegular.copyWith(
                                fontSize: 12, color: const Color(0xFF64748B)),
                          ),
                        ],
                      ),
                    ],
                  ),
                  IconButton(
                    onPressed: () => Navigator.pop(context),
                    icon: const Icon(Icons.close, color: Color(0xFF64748B)),
                  ),
                ],
              ),
              const SizedBox(height: 20),
              const Divider(height: 1, color: Color(0xFFE2E8F0)),
              const SizedBox(height: 16),

              // Active Token List Tiles
              _buildTokenTile(
                title: "Gateway Auth Token (Bearer)",
                value: "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3Mi...",
                status: "ACTIVE",
                statusColor: Colors.green,
                timeAgo: "Expires in 23h 45m",
              ),
              const SizedBox(height: 12),
              _buildTokenTile(
                title: "Session Txn ID",
                value: "7c9e12a4-56b8-4e3a-91d0-abdm98765432",
                status: "VALID",
                statusColor: Colors.blue,
                timeAgo: "Issued 12m ago",
              ),
              const SizedBox(height: 12),
              _buildTokenTile(
                title: "HIP ID / Facility Key",
                value: "MIDHA_HOSPITAL_SBX_001",
                status: "CONNECTED",
                statusColor: Colors.green,
                timeAgo: "sbx.wati.digital",
              ),
              const SizedBox(height: 24),

              Align(
                alignment: Alignment.centerRight,
                child: TextButton(
                  onPressed: () => Navigator.pop(context),
                  child: Text(
                    "Close",
                    style: fontMedium.copyWith(
                        color: const Color(0xFF0F4C81), fontSize: 14),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTokenTile({
    required String title,
    required String value,
    required String status,
    required Color statusColor,
    required String timeAgo,
  }) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFFF8FAFC),
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: const Color(0xFFE2E8F0)),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text(
                      title,
                      style: fontMedium.copyWith(
                          fontSize: 13, color: const Color(0xFF1E293B)),
                    ),
                    const SizedBox(width: 8),
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 6, vertical: 2),
                      decoration: BoxDecoration(
                        color: statusColor.withOpacity(0.12),
                        borderRadius: BorderRadius.circular(4),
                      ),
                      child: Text(
                        status,
                        style: fontBold.copyWith(
                            fontSize: 10, color: statusColor),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  value,
                  style: fontRegular.copyWith(
                      fontSize: 11,
                      color: const Color(0xFF64748B),
                      fontFamily: 'monospace'),
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
          const SizedBox(width: 12),
          Text(
            timeAgo,
            style: fontRegular.copyWith(
                fontSize: 11, color: const Color(0xFF94A3B8)),
          ),
        ],
      ),
    );
  }
}

// Single Action Card Component matching exact user mockup styling
class _DashboardActionCard extends StatefulWidget {
  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  const _DashboardActionCard({
    Key? key,
    required this.icon,
    required this.title,
    required String subtitle,
    required this.onTap,
  })  : subtitle = subtitle,
        super(key: key);

  @override
  State<_DashboardActionCard> createState() => _DashboardActionCardState();
}

class _DashboardActionCardState extends State<_DashboardActionCard> {
  bool _isHovered = false;

  @override
  Widget build(BuildContext context) {
    return MouseRegion(
      cursor: SystemMouseCursors.click,
      onEnter: (_) => setState(() => _isHovered = true),
      onExit: (_) => setState(() => _isHovered = false),
      child: GestureDetector(
        onTap: widget.onTap,
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 150),
          padding: const EdgeInsets.all(20),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(16),
            border: Border.all(
              color: _isHovered
                  ? const Color(0xFF0F4C81).withOpacity(0.4)
                  : const Color(0xFFE2E8F0),
              width: _isHovered ? 1.5 : 1.0,
            ),
            boxShadow: [
              BoxShadow(
                color: _isHovered
                    ? const Color(0xFF0F4C81).withOpacity(0.08)
                    : Colors.black.withOpacity(0.02),
                blurRadius: _isHovered ? 14 : 8,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              // Left Light Grey Rounded Icon Container
              Container(
                width: 52,
                height: 52,
                decoration: BoxDecoration(
                  color: const Color(0xFFF1F5F9), // Light grey matching screenshot
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Icon(
                  widget.icon,
                  size: 24,
                  color: const Color(0xFF334155), // Dark slate icon color
                ),
              ),
              const SizedBox(width: 16),

              // Title and Subtitle Text Column
              Expanded(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.title,
                      style: fontBold.copyWith(
                        fontSize: 16,
                        color: const Color(0xFF0F172A),
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 4),
                    Text(
                      widget.subtitle,
                      style: fontRegular.copyWith(
                        fontSize: 13,
                        color: const Color(0xFF64748B),
                        height: 1.3,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
