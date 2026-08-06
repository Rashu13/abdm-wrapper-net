import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import '../controllers/health_record_controller.dart';
import '../widgets/emr_components.dart';
import '../../../../util/constants.dart';
import '../../../../util/style.dart';

class SnomedDrugConcept {
  final String name;
  final String snomedCode;
  final String category;

  const SnomedDrugConcept({
    required this.name,
    required this.snomedCode,
    required this.category,
  });
}

const List<SnomedDrugConcept> snomedDrugMasterList = [
  // Analgesics & Antipyretics
  SnomedDrugConcept(
      name: 'Dolo 650 mg (Paracetamol)',
      snomedCode: '322236009',
      category: 'Antipyretic'),
  SnomedDrugConcept(
      name: 'Paracetamol 500 mg Tablet',
      snomedCode: '387517004',
      category: 'Analgesic'),
  SnomedDrugConcept(
      name: 'Crocin 650 mg Tablet',
      snomedCode: '387517004',
      category: 'Analgesic'),
  SnomedDrugConcept(
      name: 'Calpol 500 mg Syrup',
      snomedCode: '387517004',
      category: 'Analgesic'),
  SnomedDrugConcept(
      name: 'Ibuprofen 400 mg (Brufen)',
      snomedCode: '387207008',
      category: 'NSAID'),
  SnomedDrugConcept(
      name: 'Combiflam (Ibuprofen + Paracetamol)',
      snomedCode: '387207008',
      category: 'NSAID'),
  SnomedDrugConcept(
      name: 'Diclofenac Sodium 50 mg (Voveran)',
      snomedCode: '372572004',
      category: 'NSAID'),
  SnomedDrugConcept(
      name: 'Aceclofenac 100 mg (Zerodol-P)',
      snomedCode: '387522004',
      category: 'NSAID'),
  SnomedDrugConcept(
      name: 'Mefenamic Acid 500 mg (Meftal-Spas)',
      snomedCode: '372861001',
      category: 'Antispasmodic'),
  SnomedDrugConcept(
      name: 'Tramadol 50 mg Capsule (Ultram)',
      snomedCode: '387140003',
      category: 'Opioid Analgesic'),
  SnomedDrugConcept(
      name: 'Nimesulide 100 mg (Nise)',
      snomedCode: '372552003',
      category: 'NSAID'),

  // Gastrointestinal & PPIs
  SnomedDrugConcept(
      name: 'Pantoprazole 40 mg (Pan 40)',
      snomedCode: '372605007',
      category: 'Antacid / PPI'),
  SnomedDrugConcept(
      name: 'Pan-D (Pantoprazole + Domperidone)',
      snomedCode: '372605007',
      category: 'Antacid / PPI'),
  SnomedDrugConcept(
      name: 'Omeprazole 20 mg (Omez)',
      snomedCode: '372722002',
      category: 'PPI'),
  SnomedDrugConcept(
      name: 'Rabeprazole 20 mg (Rablet / Razo)',
      snomedCode: '372619001',
      category: 'PPI'),
  SnomedDrugConcept(
      name: 'Ranitidine 150 mg (Aciloc / Rantac)',
      snomedCode: '372765001',
      category: 'H2 Blocker'),
  SnomedDrugConcept(
      name: 'Ondansetron 4 mg (Emeset / Vomikind)',
      snomedCode: '372561000',
      category: 'Antiemetic'),
  SnomedDrugConcept(
      name: 'Domperidone 10 mg (Vomiplus)',
      snomedCode: '372535003',
      category: 'Antiemetic'),
  SnomedDrugConcept(
      name: 'Sucralfate Syrup (Sucrafil)',
      snomedCode: '372810002',
      category: 'Ulcer Healing'),

  // Antibiotics & Anti-infectives
  SnomedDrugConcept(
      name: 'Amoxicillin 500 mg (Moxikind / Novamox)',
      snomedCode: '372687004',
      category: 'Antibiotic'),
  SnomedDrugConcept(
      name: 'Augmentin 625 (Amoxicillin + Clavulanate)',
      snomedCode: '372687004',
      category: 'Antibiotic'),
  SnomedDrugConcept(
      name: 'Azithromycin 500 mg (Azee / Zathrin)',
      snomedCode: '372522002',
      category: 'Macrolide Antibiotic'),
  SnomedDrugConcept(
      name: 'Ciprofloxacin 500 mg (Cifran)',
      snomedCode: '372828008',
      category: 'Fluoroquinolone'),
  SnomedDrugConcept(
      name: 'Levofloxacin 500 mg (Levoquin)',
      snomedCode: '372545009',
      category: 'Fluoroquinolone'),
  SnomedDrugConcept(
      name: 'Ofloxacin + Ornidazole (Oflomac-OZ)',
      snomedCode: '372584001',
      category: 'Antibacterial'),
  SnomedDrugConcept(
      name: 'Cefixime 200 mg (Taxim-O / Zifi)',
      snomedCode: '372527008',
      category: 'Cephalosporin'),
  SnomedDrugConcept(
      name: 'Cefpodoxime 200 mg (Gudcef / Doxcef)',
      snomedCode: '372530006',
      category: 'Cephalosporin'),
  SnomedDrugConcept(
      name: 'Ceftriaxone 1g Injection (Monocef)',
      snomedCode: '372528003',
      category: 'Injectable Antibiotic'),
  SnomedDrugConcept(
      name: 'Doxycycline 100 mg (Dox-SL)',
      snomedCode: '372827003',
      category: 'Tetracycline'),
  SnomedDrugConcept(
      name: 'Metronidazole 400 mg (Flagyl)',
      snomedCode: '372594002',
      category: 'Antiamoebic'),

  // Antihistamines & Respiratory
  SnomedDrugConcept(
      name: 'Cetirizine 10 mg (Cetzine / Okacet)',
      snomedCode: '372583007',
      category: 'Antihistamine'),
  SnomedDrugConcept(
      name: 'Levocetirizine 5 mg (Lecope / 1-AL)',
      snomedCode: '372583007',
      category: 'Antihistamine'),
  SnomedDrugConcept(
      name: 'Montelukast + Levocetirizine (Montek-LC)',
      snomedCode: '372583007',
      category: 'Anti-asthmatic'),
  SnomedDrugConcept(
      name: 'Fexofenadine 120 mg (Allegra)',
      snomedCode: '372541005',
      category: 'Antihistamine'),
  SnomedDrugConcept(
      name: 'Ascoril LS Syrup (Levosalbutamol + Ambroxol)',
      snomedCode: '372599007',
      category: 'Cough Syrup'),
  SnomedDrugConcept(
      name: 'Salbutamol Inhaler 100mcg (Asthalin)',
      snomedCode: '372599007',
      category: 'Bronchodilator'),
  SnomedDrugConcept(
      name: 'Budesonide Inhaler 200mcg (Budecort)',
      snomedCode: '372825006',
      category: 'Corticosteroid'),

  // Anti-Diabetic
  SnomedDrugConcept(
      name: 'Metformin 500 mg (Glycomet / Obimet)',
      snomedCode: '372567009',
      category: 'Anti-diabetic'),
  SnomedDrugConcept(
      name: 'Glimepiride 1 mg / 2 mg (Amaryl)',
      snomedCode: '372548006',
      category: 'Sulfonylurea'),
  SnomedDrugConcept(
      name: 'Teneligliptin 20 mg (Tenepure / Tenglyn)',
      snomedCode: '712398001',
      category: 'DPP-4 Inhibitor'),
  SnomedDrugConcept(
      name: 'Sitagliptin 50 mg (Januvia)',
      snomedCode: '702543004',
      category: 'DPP-4 Inhibitor'),
  SnomedDrugConcept(
      name: 'Dapagliflozin 10 mg (Forxiga)',
      snomedCode: '703663004',
      category: 'SGLT2 Inhibitor'),
  SnomedDrugConcept(
      name: 'Vildagliptin 50 mg (Galvus)',
      snomedCode: '702542009',
      category: 'DPP-4 Inhibitor'),
  SnomedDrugConcept(
      name: 'Human Mixtard 30/70 Insulin',
      snomedCode: '372560004',
      category: 'Insulin'),

  // Cardiovascular & Antihypertensive
  SnomedDrugConcept(
      name: 'Amlodipine 5 mg (Stamlo / Amlong)',
      snomedCode: '372833007',
      category: 'Calcium Blocker'),
  SnomedDrugConcept(
      name: 'Telmisartan 40 mg (Telma / Tazloc)',
      snomedCode: '372862008',
      category: 'ARB Antihypertensive'),
  SnomedDrugConcept(
      name: 'Telma-H (Telmisartan + Hydrochlorothiazide)',
      snomedCode: '372862008',
      category: 'Combination Antihypertensive'),
  SnomedDrugConcept(
      name: 'Losartan 50 mg (Repace / Losar)',
      snomedCode: '372860000',
      category: 'ARB Antihypertensive'),
  SnomedDrugConcept(
      name: 'Atenolol 50 mg (Aten / Betacard)',
      snomedCode: '372836004',
      category: 'Beta Blocker'),
  SnomedDrugConcept(
      name: 'Atorvastatin 10 mg / 20 mg (Atorva)',
      snomedCode: '372854002',
      category: 'Statin Lipid Lowering'),
  SnomedDrugConcept(
      name: 'Rosuvastatin 10 mg (Rosuvas)',
      snomedCode: '372856000',
      category: 'Statin Lipid Lowering'),
  SnomedDrugConcept(
      name: 'Ecosprin 75 mg (Aspirin)',
      snomedCode: '387170009',
      category: 'Antiplatelet'),
  SnomedDrugConcept(
      name: 'Clopidogrel 75 mg (Clopilet)',
      snomedCode: '372850006',
      category: 'Antiplatelet'),

  // Vitamins, Minerals & Supplements
  SnomedDrugConcept(
      name: 'Calcium + Vitamin D3 (Shelcal 500)',
      snomedCode: '421689004',
      category: 'Calcium Supplement'),
  SnomedDrugConcept(
      name: 'Vitamin D3 60,000 IU (Urisee 60K / Tayo)',
      snomedCode: '421689004',
      category: 'Vitamin Supplement'),
  SnomedDrugConcept(
      name: 'Becosules Capsules (Vitamin B-Complex)',
      snomedCode: '421689004',
      category: 'B-Complex'),
  SnomedDrugConcept(
      name: 'Autrin / Orofer XT (Ferrous Ascorbate + Folic Acid)',
      snomedCode: '421689004',
      category: 'Iron Supplement'),
  SnomedDrugConcept(
      name: 'Neurobion Forte (Vitamin B12 + B Complex)',
      snomedCode: '421689004',
      category: 'Neurotrophic Supplement'),

  // Dermatological & Topical
  SnomedDrugConcept(
      name: 'Candid Dusting Powder (Clotrimazole)',
      snomedCode: '372589008',
      category: 'Antifungal'),
  SnomedDrugConcept(
      name: 'Betnovate-N Ointment (Betamethasone + Neomycin)',
      snomedCode: '372824005',
      category: 'Topical Steroid'),
  SnomedDrugConcept(
      name: 'Quadriderm Cream',
      snomedCode: '372824005',
      category: 'Topical Cream'),
];

// ═══════════════════════════════════════════════════════════════════════════════
// EMR HEALTH RECORDS PAGE — Launcher Grid
// Shows 6 cards for each ABDM HI Type. Tap to navigate to dedicated form page.
// ═══════════════════════════════════════════════════════════════════════════════
class EmrHealthRecordsPage extends GetView<HealthRecordController> {
  const EmrHealthRecordsPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final hiCards = [
      _HiCard(
        icon: Icons.local_hospital_rounded,
        label: 'OPConsultation',
        subtitle: 'Outpatient visit record',
        gradient: [const Color(0xFF64748B), const Color(0xFF475569)],
        page: const EmrOpConsultationPage(),
      ),
      _HiCard(
        icon: Icons.medication_rounded,
        label: 'Prescription',
        subtitle: 'Medicines & dosage (Rx)',
        gradient: [const Color(0xFF059669), const Color(0xFF047857)],
        page: const EmrPrescriptionPage(),
      ),
      _HiCard(
        icon: Icons.biotech_rounded,
        label: 'DiagnosticReport',
        subtitle: 'Lab tests & observations',
        gradient: [const Color(0xFF2563EB), const Color(0xFF1D4ED8)],
        page: const EmrDiagnosticPage(),
      ),
      _HiCard(
        icon: Icons.description_rounded,
        label: 'DischargeSummary',
        subtitle: 'Hospitalization & discharge',
        gradient: [const Color(0xFF7C3AED), const Color(0xFF6D28D9)],
        page: const EmrDischargeSummaryPage(),
      ),
      _HiCard(
        icon: Icons.vaccines_rounded,
        label: 'ImmunizationRecord',
        subtitle: 'Vaccinations & immunizations',
        gradient: [const Color(0xFFD97706), const Color(0xFFB45309)],
        page: const EmrImmunizationPage(),
      ),
      _HiCard(
        icon: Icons.favorite_rounded,
        label: 'WellnessRecord',
        subtitle: 'Vitals & health assessment',
        gradient: [const Color(0xFFDC2626), const Color(0xFFB91C1C)],
        page: const EmrWellnessPage(),
      ),
    ];

    return Scaffold(
      backgroundColor: AppColor.background,
      body: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Fixed Left Patient Selector Sidebar
            SizedBox(
              width: 310,
              child: PatientLeftSidebarPanel(controller: controller),
            ),
            const SizedBox(width: 14),
            // Right Main Content Grid
            Expanded(
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // ── Header Bar ──────────────────────────────────────────────────
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 14, vertical: 10),
                      decoration: glassDecoration(),
                      child: Row(
                        children: [
                          Container(
                            padding: const EdgeInsets.all(10),
                            decoration: BoxDecoration(
                              gradient: const LinearGradient(
                                colors: [Color(0xFF64748B), Color(0xFF475569)],
                              ),
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: const Icon(Icons.medical_services_rounded,
                                color: Colors.white, size: 22),
                          ),
                          const SizedBox(width: 14),
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text('Sonomed EMR Studio',
                                  style: fontBold.copyWith(fontSize: 16)),
                              Text('Health Record & Care Context Generator',
                                  style: fontSmall),
                            ],
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 20),

                    Text('Select Health Information Type',
                        style: fontBold.copyWith(fontSize: 15)),
                    const SizedBox(height: 6),
                    Text(
                        'Choose a record type to fill and link to ABDM Gateway',
                        style: fontSmall),
                    const SizedBox(height: 16),

                    // ── 3×2 Card Grid ────────────────────────────────────────────────
                    GridView.count(
                      crossAxisCount: 3,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      crossAxisSpacing: 14,
                      mainAxisSpacing: 14,
                      childAspectRatio: 2.6,
                      children: hiCards.map((card) {
                        return _EmrLauncherCard(card: card);
                      }).toList(),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ─── Launcher Card Data Model ─────────────────────────────────────────────────
class _HiCard {
  final IconData icon;
  final String label;
  final String subtitle;
  final List<Color> gradient;
  final Widget page;
  const _HiCard({
    required this.icon,
    required this.label,
    required this.subtitle,
    required this.gradient,
    required this.page,
  });
}

// ─── Launcher Card Widget ─────────────────────────────────────────────────────
class _EmrLauncherCard extends StatefulWidget {
  final _HiCard card;
  const _EmrLauncherCard({required this.card});

  @override
  State<_EmrLauncherCard> createState() => _EmrLauncherCardState();
}

class _EmrLauncherCardState extends State<_EmrLauncherCard> {
  bool _hovered = false;

  @override
  Widget build(BuildContext context) {
    return MouseRegion(
      onEnter: (_) => setState(() => _hovered = true),
      onExit: (_) => setState(() => _hovered = false),
      cursor: SystemMouseCursors.click,
      child: GestureDetector(
        onTap: () => Get.to(() => widget.card.page,
            transition: Transition.rightToLeft,
            duration: const Duration(milliseconds: 250)),
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 180),
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: _hovered
                  ? widget.card.gradient
                  : [AppColor.surface, AppColor.surface],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(14),
            border: Border.all(
              color: _hovered
                  ? widget.card.gradient.first.withOpacity(0.8)
                  : AppColor.border,
              width: _hovered ? 1.5 : 1.0,
            ),
            boxShadow: _hovered
                ? [
                    BoxShadow(
                      color: widget.card.gradient.last.withOpacity(0.25),
                      blurRadius: 16,
                      offset: const Offset(0, 4),
                    )
                  ]
                : [],
          ),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: _hovered
                      ? Colors.white.withOpacity(0.2)
                      : widget.card.gradient.first.withOpacity(0.12),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(widget.card.icon,
                    color: _hovered ? Colors.white : widget.card.gradient.first,
                    size: 22),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      widget.card.label,
                      style: fontBold.copyWith(
                        fontSize: 13.5,
                        color: _hovered ? Colors.white : AppColor.textPrimary,
                      ),
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    Text(
                      widget.card.subtitle,
                      style: fontSmall.copyWith(
                        fontSize: 11,
                        color: _hovered
                            ? Colors.white.withOpacity(0.8)
                            : AppColor.textSecondary,
                      ),
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ),
              ),
              Icon(Icons.arrow_forward_ios_rounded,
                  size: 14,
                  color: _hovered
                      ? Colors.white.withOpacity(0.8)
                      : AppColor.textSecondary),
            ],
          ),
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// SHARED: EmrFormShell — Consistent Scaffold for each HI Type page
// ═══════════════════════════════════════════════════════════════════════════════
class EmrFormShell extends GetView<HealthRecordController> {
  final IconData hiIcon;
  final List<Color> hiGradient;
  final String hiTitle;
  final String hiSubtitle;
  final Widget formBody;

  const EmrFormShell({
    Key? key,
    required this.hiIcon,
    required this.hiGradient,
    required this.hiTitle,
    required this.hiSubtitle,
    required this.formBody,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (controller.activeHiType.value != hiTitle) {
        controller.activeHiType.value = hiTitle;
      }
    });

    return Scaffold(
      backgroundColor: AppColor.background,
      appBar: AppBar(
        backgroundColor: AppColor.surface,
        elevation: 0,
        surfaceTintColor: Colors.transparent,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, size: 18),
          color: AppColor.textPrimary,
          onPressed: () => Get.back(),
        ),
        title: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                gradient: LinearGradient(colors: hiGradient),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(hiIcon, color: Colors.white, size: 18),
            ),
            const SizedBox(width: 10),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(hiTitle,
                    style: fontBold.copyWith(
                        fontSize: 14, color: AppColor.textPrimary)),
                Text(hiSubtitle, style: fontSmall.copyWith(fontSize: 11)),
              ],
            ),
          ],
        ),
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(1),
          child: Container(height: 1, color: AppColor.border.withOpacity(0.4)),
        ),
      ),
      body: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Fixed Left Patient Selector Sidebar
            SizedBox(
              width: 310,
              child: PatientLeftSidebarPanel(controller: controller),
            ),
            const SizedBox(width: 14),
            // Right Main Form Body Area
            Expanded(
              child: SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Dynamic Authored Visit Date Selector Card
                    Obx(() {
                      final date = controller.selectedVisitDate.value;
                      final formattedDate =
                          "${date.day.toString().padLeft(2, '0')}/${date.month.toString().padLeft(2, '0')}/${date.year} ${date.hour.toString().padLeft(2, '0')}:${date.minute.toString().padLeft(2, '0')}";
                      return Container(
                        width: double.infinity,
                        padding: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 12),
                        margin: const EdgeInsets.only(bottom: 16),
                        decoration: BoxDecoration(
                          color: AppColor.surface,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(color: AppColor.border),
                          boxShadow: [
                            BoxShadow(
                              color: Colors.black.withOpacity(0.02),
                              blurRadius: 4,
                              offset: const Offset(0, 2),
                            ),
                          ],
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Row(
                              children: [
                                const Icon(Icons.calendar_today_rounded,
                                    color: Color(0xFF2563EB), size: 20),
                                const SizedBox(width: 12),
                                Column(
                                  crossAxisAlignment:
                                      CrossAxisAlignment.start,
                                  children: [
                                    const Text(
                                      "Authored Visit Date & Time",
                                      style: TextStyle(
                                          fontSize: 13,
                                          fontWeight: FontWeight.bold,
                                          color: Color(0xFF1E293B)),
                                    ),
                                    const SizedBox(height: 2),
                                    Text(
                                      formattedDate,
                                      style: const TextStyle(
                                          fontSize: 12,
                                          fontWeight: FontWeight.w500,
                                          color: Color(0xFF64748B)),
                                    ),
                                  ],
                                ),
                              ],
                            ),
                            ElevatedButton.icon(
                              onPressed: () async {
                                final selectedDate = await showDatePicker(
                                  context: context,
                                  initialDate: date,
                                  firstDate: DateTime(2000),
                                  lastDate: DateTime.now()
                                      .add(const Duration(days: 365)),
                                  builder: (context, child) {
                                    return Theme(
                                      data: Theme.of(context).copyWith(
                                        colorScheme: const ColorScheme.light(
                                          primary: Color(0xFF2563EB),
                                          onPrimary: Colors.white,
                                          onSurface: Color(0xFF1E293B),
                                        ),
                                      ),
                                      child: child!,
                                    );
                                  },
                                );
                                if (selectedDate != null) {
                                  if (!context.mounted) return;
                                  final selectedTime = await showTimePicker(
                                    context: context,
                                    initialTime:
                                        TimeOfDay.fromDateTime(date),
                                  );
                                  if (selectedTime != null) {
                                    controller.selectedVisitDate.value =
                                        DateTime(
                                      selectedDate.year,
                                      selectedDate.month,
                                      selectedDate.day,
                                      selectedTime.hour,
                                      selectedTime.minute,
                                    );
                                  } else {
                                    controller.selectedVisitDate.value =
                                        DateTime(
                                      selectedDate.year,
                                      selectedDate.month,
                                      selectedDate.day,
                                      date.hour,
                                      date.minute,
                                    );
                                  }
                                }
                              },
                              style: ElevatedButton.styleFrom(
                                backgroundColor:
                                    const Color(0xFF2563EB).withOpacity(0.12),
                                foregroundColor: const Color(0xFF2563EB),
                                elevation: 0,
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 14, vertical: 8),
                                shape: RoundedRectangleBorder(
                                    borderRadius:
                                        BorderRadius.circular(8)),
                              ),
                              icon: const Icon(Icons.edit_calendar_rounded,
                                  size: 16),
                              label: const Text(
                                "Change Date",
                                style: TextStyle(
                                    fontSize: 12,
                                    fontWeight: FontWeight.bold),
                              ),
                            ),
                          ],
                        ),
                      );
                    }),
                    formBody,
                    const SizedBox(height: 16),
                    // PDF Attachment Section
                    Obx(() {
                      final hasPdf =
                          controller.attachedPdfName.value.isNotEmpty;
                      return Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 12),
                        margin: const EdgeInsets.only(bottom: 16),
                        decoration: BoxDecoration(
                          color: AppColor.surface,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: hasPdf ? AppColor.accent : AppColor.border,
                            width: hasPdf ? 1.5 : 1.0,
                          ),
                        ),
                        child: Row(
                          children: [
                            Icon(
                              hasPdf
                                  ? Icons.picture_as_pdf_rounded
                                  : Icons.picture_as_pdf_outlined,
                              color: hasPdf
                                  ? Colors.redAccent
                                  : AppColor.textSecondary,
                              size: 24,
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  Text(
                                    hasPdf
                                        ? 'Manual PDF Attached'
                                        : 'Attach Manual Record PDF (Optional)',
                                    style: fontBold.copyWith(
                                        fontSize: 13,
                                        color: AppColor.textPrimary),
                                  ),
                                  const SizedBox(height: 2),
                                  Text(
                                    hasPdf
                                        ? controller.attachedPdfName.value
                                        : 'If not attached, a default clinical summary PDF will be linked.',
                                    style: fontSmall.copyWith(
                                      color: hasPdf
                                          ? AppColor.textPrimary
                                          : AppColor.textSecondary,
                                      fontWeight: hasPdf
                                          ? FontWeight.bold
                                          : FontWeight.normal,
                                    ),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ],
                              ),
                            ),
                            if (hasPdf)
                              IconButton(
                                icon: const Icon(Icons.clear_rounded,
                                    color: Colors.grey, size: 20),
                                onPressed: controller.clearAttachedPdf,
                              )
                            else
                              ElevatedButton.icon(
                                style: ElevatedButton.styleFrom(
                                  backgroundColor:
                                      AppColor.accent.withOpacity(0.12),
                                  foregroundColor: AppColor.accent,
                                  elevation: 0,
                                  padding: const EdgeInsets.symmetric(
                                      horizontal: 14, vertical: 8),
                                  shape: RoundedRectangleBorder(
                                      borderRadius: BorderRadius.circular(8)),
                                ),
                                icon: const Icon(Icons.attach_file_rounded,
                                    size: 16),
                                label: const Text('Attach PDF',
                                    style: TextStyle(
                                        fontSize: 12,
                                        fontWeight: FontWeight.bold)),
                                onPressed: controller.selectPdfFile,
                              ),
                          ],
                        ),
                      );
                    }),
                    // 2 Separate Action Buttons: Save Locally vs Save & Link to ABDM Gateway
                    Obx(() {
                      final isSaving = controller.isSavingHealthRecord.value;
                      return Row(
                        children: [
                          // Button 1: Save Local EMR Record
                          Expanded(
                            flex: 1,
                            child: EmrPrimaryButton(
                              label: 'Save Record 💾',
                              icon: Icons.save_rounded,
                              backgroundColor: AppColor.surface,
                              foregroundColor: const Color(0xFF334155),
                              height: 44,
                              isLoading: isSaving,
                              onPressed: controller.saveRecordLocally,
                            ),
                          ),
                          const SizedBox(width: 12),
                          // Button 2: Save & Link to ABDM Gateway
                          Expanded(
                            flex: 2,
                            child: EmrPrimaryButton(
                              label: isSaving
                                  ? 'Processing ABDM Link...'
                                  : 'Link to ABDM Gateway 🚀',
                              icon: Icons.cloud_upload_rounded,
                              backgroundColor: hiGradient.last,
                              foregroundColor: Colors.white,
                              height: 44,
                              isLoading: isSaving,
                              onPressed: controller.generateAndLinkCareContext,
                            ),
                          ),
                        ],
                      );
                    }),
                    const SizedBox(height: 16),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PAGE 1: OPConsultation
// ═══════════════════════════════════════════════════════════════════════════════
class EmrOpConsultationPage extends GetView<HealthRecordController> {
  const EmrOpConsultationPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return EmrFormShell(
      hiIcon: Icons.local_hospital_rounded,
      hiGradient: const [Color(0xFF64748B), Color(0xFF475569)],
      hiTitle: 'OPConsultation',
      hiSubtitle: 'Outpatient Visit Record',
      formBody: _OpConsultationBody(controller: controller),
    );
  }
}

class _OpConsultationBody extends StatelessWidget {
  final HealthRecordController controller;
  const _OpConsultationBody({required this.controller});

  @override
  Widget build(BuildContext context) {
    final content = Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('ENCOUNTER TYPE & METADATA',
            style: fontBold.copyWith(fontSize: 14)),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: Obx(() => Container(
                    padding: const EdgeInsets.symmetric(horizontal: 14),
                    decoration: BoxDecoration(
                      color: AppColor.background,
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppColor.border),
                    ),
                    child: DropdownButtonHideUnderline(
                      child: DropdownButton<String>(
                        value: controller.encounterType.value,
                        isExpanded: true,
                        dropdownColor: AppColor.surface,
                        style: TextStyle(
                            color: AppColor.textPrimary, fontSize: 13),
                        items: const [
                          DropdownMenuItem(
                              value: 'Outpatient', child: Text('Outpatient')),
                          DropdownMenuItem(
                              value: 'Inpatient', child: Text('Inpatient')),
                          DropdownMenuItem(
                              value: 'Emergency', child: Text('Emergency')),
                          DropdownMenuItem(
                              value: 'Ambulatory', child: Text('Ambulatory')),
                        ],
                        onChanged: (val) {
                          if (val != null) controller.encounterType.value = val;
                        },
                      ),
                    ),
                  )),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                decoration: BoxDecoration(
                  color: AppColor.background,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColor.border),
                ),
                child: Text(
                  'Visit Date: ${DateTime.now().day}/${DateTime.now().month}/${DateTime.now().year} ${DateTime.now().hour}:${DateTime.now().minute}',
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 13),
                ),
              ),
            ),
          ],
        ),

        const Divider(height: 32),

        Text('BODY MEASUREMENTS', style: fontBold.copyWith(fontSize: 14)),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: EmrCompactTextField(
                controller: controller.opHeightCtrl,
                labelText: 'HEIGHT (CM)',
                hintText: '170',
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: EmrCompactTextField(
                controller: controller.opWeightCtrl,
                labelText: 'WEIGHT (KG)',
                hintText: '68',
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: EmrCompactTextField(
                controller: controller.opBmiCtrl,
                labelText: 'BMI (KG/M²)',
              ),
            ),
          ],
        ),

        const Divider(height: 32),

        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text('VITALS', style: fontBold.copyWith(fontSize: 14)),
            EmrAddButton(
              label: '+ Add Vital',
              onPressed: controller.addVital,
            ),
          ],
        ),
        const SizedBox(height: 10),
        Obx(() => ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: controller.vitalsList.length,
              separatorBuilder: (_, __) => const SizedBox(height: 10),
              itemBuilder: (context, idx) {
                final v = controller.vitalsList[idx];
                return Row(
                  children: [
                    Expanded(
                      flex: 3,
                      child: EmrCompactTextField(
                        controller: v.vitalNameCtrl,
                        labelText: 'Vital Name',
                        hintText: 'e.g. Blood Pressure',
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      flex: 2,
                      child: EmrCompactTextField(
                        controller: v.valueCtrl,
                        labelText: 'Value',
                        hintText: '120/80',
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      flex: 1,
                      child: EmrCompactTextField(
                        controller: v.unitCtrl,
                        labelText: 'Unit',
                        hintText: 'mmHg',
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.delete_outline_rounded,
                          color: Colors.redAccent, size: 18),
                      onPressed: () => controller.removeVital(idx),
                    ),
                  ],
                );
              },
            )),

        const Divider(height: 32),

        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text('CHIEF COMPLAINTS', style: fontBold.copyWith(fontSize: 14)),
            EmrAddButton(
              label: '+ Add Complaint',
              onPressed: controller.addComplaint,
            ),
          ],
        ),
        const SizedBox(height: 10),
        Obx(() => ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: controller.complaintsList.length,
              separatorBuilder: (_, __) => const SizedBox(height: 8),
              itemBuilder: (context, idx) {
                return Row(
                  children: [
                    Expanded(
                      child: EmrCompactTextField(
                        controller: controller.complaintsList[idx],
                        labelText: 'Chief Complaint',
                        hintText:
                            'Enter complaint details (e.g. Fever with chills for 2 days)',
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.delete_outline_rounded,
                          color: Colors.redAccent, size: 20),
                      onPressed: () => controller.removeComplaint(idx),
                    ),
                  ],
                );
              },
            )),

        const Divider(height: 32),

        Text('CLINICAL OBSERVATION / EXAMINATION RESULT',
            style: fontBold.copyWith(fontSize: 14)),
        const SizedBox(height: 8),
        EmrCompactTextField(
          controller: controller.opObservationResultCtrl,
          labelText: 'CLINICAL OBSERVATION / EXAMINATION RESULT',
          hintText: 'Enter clinical examination notes or test results...',
          maxLines: 2,
        ),

        const Divider(height: 32),

        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text('ALLERGIES', style: fontBold.copyWith(fontSize: 14)),
            EmrAddButton(
              label: '+ Add Allergy',
              onPressed: controller.addAllergy,
            ),
          ],
        ),
        const SizedBox(height: 10),
        Obx(() => ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: controller.allergiesList.length,
              separatorBuilder: (_, __) => const SizedBox(height: 8),
              itemBuilder: (context, idx) {
                final allergy = controller.allergiesList[idx];
                return Row(
                  children: [
                    Expanded(
                      flex: 3,
                      child: EmrCompactTextField(
                        controller: allergy.allergyNameCtrl,
                        labelText: 'Allergy / Substance',
                        hintText: 'e.g. Penicillin',
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      flex: 2,
                      child: EmrSelectBox<String>(
                        labelText: 'Type',
                        value: allergy.type,
                        items: const [
                          'medication',
                          'food',
                          'environment',
                          'biologic'
                        ],
                        onChanged: (val) {
                          if (val != null) allergy.type = val;
                        },
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.delete_outline_rounded,
                          color: Colors.redAccent, size: 20),
                      onPressed: () => controller.removeAllergy(idx),
                    ),
                  ],
                );
              },
            )),

        const Divider(height: 32),

        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text('MEDICAL HISTORY', style: fontBold.copyWith(fontSize: 14)),
            EmrAddButton(
              label: '+ Add Condition',
              onPressed: controller.addMedicalHistory,
            ),
          ],
        ),
        const SizedBox(height: 10),
        Obx(() => ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: controller.medicalHistoryList.length,
              separatorBuilder: (_, __) => const SizedBox(height: 8),
              itemBuilder: (context, idx) {
                return Row(
                  children: [
                    Expanded(
                      child: EmrCompactTextField(
                        controller: controller.medicalHistoryList[idx],
                        labelText: 'Medical History',
                        hintText:
                            'Medical condition (e.g. Type 2 Diabetes Mellitus)',
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.delete_outline_rounded,
                          color: Colors.redAccent, size: 20),
                      onPressed: () => controller.removeMedicalHistory(idx),
                    ),
                  ],
                );
              },
            )),

        const Divider(height: 32),

        // Embedded Prescription section
        _PrescriptionBody(controller: controller),
      ],
    );

    return Container(
      padding: const EdgeInsets.all(12),
      decoration: glassDecoration(),
      child: content,
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PAGE 2: Prescription
// ═══════════════════════════════════════════════════════════════════════════════
class EmrPrescriptionPage extends GetView<HealthRecordController> {
  const EmrPrescriptionPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return EmrFormShell(
      hiIcon: Icons.medication_rounded,
      hiGradient: const [Color(0xFF059669), Color(0xFF047857)],
      hiTitle: 'Prescription',
      hiSubtitle: 'Medicines & Dosage (Rx)',
      formBody: Container(
        padding: const EdgeInsets.all(12),
        decoration: glassDecoration(),
        child: _PrescriptionBody(controller: controller),
      ),
    );
  }
}

class _PrescriptionBody extends StatelessWidget {
  final HealthRecordController controller;
  const _PrescriptionBody({required this.controller});

  @override
  Widget build(BuildContext context) {
    const dosageOptions = [
      'Morning Only (1-0-0)',
      'Morning & Night (1-0-1)',
      'Thrice a Day (1-1-1)',
      'Once a Day (0-0-1)',
      'As Needed (PRN)',
    ];
    const routeOptions = [
      'Oral',
      'IV',
      'IM',
      'Topical',
      'Inhalation',
      'Sublingual'
    ];
    const methodOptions = [
      'After Food',
      'Before Food',
      'With Food',
      'Empty Stomach'
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text('PRESCRIPTION MEDICINES (RX)',
                style: fontBold.copyWith(fontSize: 14)),
            EmrAddButton(
              label: '+ Add Medicine',
              onPressed: controller.addMedicine,
            ),
          ],
        ),
        const SizedBox(height: 12),
        Obx(() => ListView.separated(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: controller.medicines.length,
              separatorBuilder: (_, __) => const SizedBox(height: 12),
              itemBuilder: (context, idx) {
                final med = controller.medicines[idx];
                String currentDosage = dosageOptions.contains(med.dosagePattern)
                    ? med.dosagePattern
                    : dosageOptions.first;
                String currentRoute = routeOptions.contains(med.route)
                    ? med.route
                    : routeOptions.first;
                String currentMethod = methodOptions.contains(med.method)
                    ? med.method
                    : methodOptions.first;

                return StatefulBuilder(
                  builder: (context, setMedState) {
                    return Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: AppColor.surface,
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: AppColor.border),
                        boxShadow: [
                          BoxShadow(
                              color: Colors.black.withOpacity(0.03),
                              blurRadius: 6,
                              offset: const Offset(0, 2))
                        ],
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                flex: 3,
                                child: RawAutocomplete<SnomedDrugConcept>(
                                  textEditingController: med.drugNameCtrl,
                                  focusNode: med.focusNode,
                                  optionsBuilder: (TextEditingValue val) {
                                    if (val.text.trim().isEmpty) {
                                      return const Iterable<
                                          SnomedDrugConcept>.empty();
                                    }
                                    final q = val.text.toLowerCase().trim();
                                    return snomedDrugMasterList.where((d) =>
                                        d.name.toLowerCase().contains(q) ||
                                        d.category.toLowerCase().contains(q) ||
                                        d.snomedCode.contains(q));
                                  },
                                  displayStringForOption:
                                      (SnomedDrugConcept option) => option.name,
                                  onSelected: (SnomedDrugConcept selection) {
                                    med.drugNameCtrl.text = selection.name;
                                    med.snomedCodeCtrl.text =
                                        selection.snomedCode;
                                    setMedState(() {});
                                  },
                                  fieldViewBuilder: (context, controller,
                                      focusNode, onFieldSubmitted) {
                                    return EmrCompactTextField(
                                      controller: controller,
                                      labelText: 'MEDICINE / DRUG NAME',
                                      hintText:
                                          'Search Drug / SNOMED (e.g. Telma, Pan)',
                                    );
                                  },
                                  optionsViewBuilder:
                                      (context, onSelected, options) {
                                    return Align(
                                      alignment: Alignment.topLeft,
                                      child: Material(
                                        elevation: 6,
                                        color: AppColor.surface,
                                        borderRadius: BorderRadius.circular(8),
                                        child: Container(
                                          width: 320,
                                          height: 220,
                                          decoration: BoxDecoration(
                                            color: AppColor.surface,
                                            borderRadius:
                                                BorderRadius.circular(8),
                                            border: Border.all(
                                                color: AppColor.border),
                                          ),
                                          child: ListView.separated(
                                            padding: const EdgeInsets.all(4),
                                            shrinkWrap: true,
                                            itemCount: options.length,
                                            separatorBuilder: (_, __) =>
                                                const Divider(height: 1),
                                            itemBuilder: (context, index) {
                                              final option =
                                                  options.elementAt(index);
                                              return InkWell(
                                                onTap: () => onSelected(option),
                                                child: Padding(
                                                  padding: const EdgeInsets
                                                      .symmetric(
                                                      horizontal: 10,
                                                      vertical: 8),
                                                  child: Text(
                                                    option.name,
                                                    style: TextStyle(
                                                        color: AppColor
                                                            .textPrimary,
                                                        fontSize: 12,
                                                        fontWeight:
                                                            FontWeight.bold),
                                                  ),
                                                ),
                                              );
                                            },
                                          ),
                                        ),
                                      ),
                                    );
                                  },
                                ),
                              ),
                              const SizedBox(width: 10),
                              Expanded(
                                flex: 2,
                                child: EmrSelectBox<String>(
                                  labelText: 'DOSAGE',
                                  value: currentDosage,
                                  items: dosageOptions,
                                  onChanged: (val) {
                                    if (val != null) {
                                      med.dosagePattern = val;
                                      setMedState(() {});
                                    }
                                  },
                                ),
                              ),
                              const SizedBox(width: 10),
                              Expanded(
                                flex: 1,
                                child: EmrCompactTextField(
                                  controller: med.snomedCodeCtrl,
                                  labelText: 'SNOMED CODE',
                                  hintText: '322236009',
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 12),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                flex: 2,
                                child: EmrSelectBox<String>(
                                  labelText: 'ROUTE',
                                  value: currentRoute,
                                  items: routeOptions,
                                  onChanged: (val) {
                                    if (val != null) {
                                      med.route = val;
                                      setMedState(() {});
                                    }
                                  },
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                flex: 2,
                                child: EmrSelectBox<String>(
                                  labelText: 'METHOD',
                                  value: currentMethod,
                                  items: methodOptions,
                                  onChanged: (val) {
                                    if (val != null) {
                                      med.method = val;
                                      setMedState(() {});
                                    }
                                  },
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                flex: 3,
                                child: EmrCompactTextField(
                                  controller: med.reasonCtrl,
                                  labelText: 'REASON',
                                  hintText: 'Reason (e.g. Fever & Pain)',
                                ),
                              ),
                              if (controller.medicines.length > 1) ...[
                                const SizedBox(width: 8),
                                Padding(
                                  padding: const EdgeInsets.only(top: 22),
                                  child: IconButton(
                                    icon: const Icon(Icons.close_rounded,
                                        color: Colors.redAccent, size: 20),
                                    onPressed: () =>
                                        controller.removeMedicine(idx),
                                  ),
                                ),
                              ],
                            ],
                          ),
                        ],
                      ),
                    );
                  },
                );
              },
            )),
        const SizedBox(height: 20),
        EmrCompactTextField(
          controller: controller.adviceCtrl,
          labelText: 'Doctor Advice & Instructions',
          hintText:
              'Rest for 3 days, drink plenty of fluids. Review after 1 week.',
          maxLines: 3,
        ),
      ],
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PAGE 3: Diagnostic Report
// ═══════════════════════════════════════════════════════════════════════════════
class EmrDiagnosticPage extends GetView<HealthRecordController> {
  const EmrDiagnosticPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return EmrFormShell(
      hiIcon: Icons.biotech_rounded,
      hiGradient: const [Color(0xFF2563EB), Color(0xFF1D4ED8)],
      hiTitle: 'DiagnosticReport',
      hiSubtitle: 'Lab Tests & Observations',
      formBody: Container(
        padding: const EdgeInsets.all(12),
        decoration: glassDecoration(),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.biotech_rounded,
                    color: Color(0xFF2563EB), size: 22),
                const SizedBox(width: 8),
                Text('Diagnostic Report & Lab Tests',
                    style: fontBold.copyWith(fontSize: 16)),
              ],
            ),
            const SizedBox(height: 16),
            EmrCompactTextField(
              controller: controller.reportTitleCtrl,
              labelText: 'REPORT TITLE',
              hintText: 'e.g. Blood Test Report',
            ),
            const SizedBox(height: 18),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Lab Test Observations & Results',
                    style: fontBold.copyWith(fontSize: 14)),
                EmrAddButton(
                  label: 'Add Test Result',
                  onPressed: controller.addLabResult,
                ),
              ],
            ),
            const SizedBox(height: 10),
            Obx(() => ListView.separated(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemCount: controller.labResults.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 10),
                  itemBuilder: (context, idx) {
                    final lab = controller.labResults[idx];
                    return Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColor.border),
                      ),
                      child: Row(
                        children: [
                          Expanded(
                            flex: 3,
                            child: EmrCompactTextField(
                              controller: lab.testNameCtrl,
                              labelText: 'Test Name',
                              hintText: 'e.g. Hemoglobin',
                            ),
                          ),
                          const SizedBox(width: 6),
                          Expanded(
                            flex: 2,
                            child: EmrCompactTextField(
                              controller: lab.snomedCodeCtrl,
                              labelText: 'SNOMED/LOINC',
                              hintText: 'e.g. 721981007',
                            ),
                          ),
                          const SizedBox(width: 6),
                          Expanded(
                            flex: 2,
                            child: EmrCompactTextField(
                              controller: lab.valueCtrl,
                              labelText: 'Value',
                            ),
                          ),
                          const SizedBox(width: 6),
                          Expanded(
                            flex: 1,
                            child: EmrCompactTextField(
                              controller: lab.unitCtrl,
                              labelText: 'Unit',
                              hintText: 'g/dL',
                            ),
                          ),
                          IconButton(
                            icon: const Icon(Icons.delete_outline_rounded,
                                color: Colors.redAccent, size: 20),
                            onPressed: () => controller.removeLabResult(idx),
                          ),
                        ],
                      ),
                    );
                  },
                )),
          ],
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PAGE 4: Discharge Summary
// ═══════════════════════════════════════════════════════════════════════════════
class EmrDischargeSummaryPage extends GetView<HealthRecordController> {
  const EmrDischargeSummaryPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return EmrFormShell(
      hiIcon: Icons.description_rounded,
      hiGradient: const [Color(0xFF7C3AED), Color(0xFF6D28D9)],
      hiTitle: 'DischargeSummary',
      hiSubtitle: 'Hospitalization & Discharge Notes',
      formBody: Container(
        padding: const EdgeInsets.all(12),
        decoration: glassDecoration(),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.description_rounded,
                    color: Color(0xFF7C3AED), size: 22),
                const SizedBox(width: 8),
                Text('Discharge Summary & Hospitalization Notes',
                    style: fontBold.copyWith(fontSize: 16)),
              ],
            ),
            const SizedBox(height: 12),
            EmrCompactTextField(
              controller: controller.dischargeNotesCtrl,
              labelText: 'HOSPITALIZATION SUMMARY & DISCHARGE NOTES',
              hintText:
                  'Enter hospitalization summary, admission findings, treatment given & discharge condition...',
              maxLines: 4,
            ),
            const SizedBox(height: 20),
            const Divider(),
            const SizedBox(height: 12),
            Text('ASSOCIATED OP CONSULTATION',
                style: fontBold.copyWith(
                    fontSize: 13, color: AppColor.textSecondary)),
            const SizedBox(height: 8),
            _OpConsultationBody(controller: controller),
          ],
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PAGE 5: Immunization Record
// ═══════════════════════════════════════════════════════════════════════════════
class EmrImmunizationPage extends GetView<HealthRecordController> {
  const EmrImmunizationPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return EmrFormShell(
      hiIcon: Icons.vaccines_rounded,
      hiGradient: const [Color(0xFFD97706), Color(0xFFB45309)],
      hiTitle: 'ImmunizationRecord',
      hiSubtitle: 'Vaccinations & Immunizations',
      formBody: Container(
        padding: const EdgeInsets.all(20),
        decoration: glassDecoration(),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Vaccination & Immunization Record',
                    style: fontBold.copyWith(fontSize: 16)),
                EmrAddButton(
                  label: '+ Add Vaccine',
                  onPressed: controller.addImmunization,
                ),
              ],
            ),
            const SizedBox(height: 12),
            Obx(() => ListView.separated(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemCount: controller.immunizationList.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 10),
                  itemBuilder: (context, idx) {
                    final item = controller.immunizationList[idx];
                    return Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColor.border),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Expanded(
                                flex: 3,
                                child: EmrCompactTextField(
                                  controller: item.vaccineNameCtrl,
                                  labelText:
                                      'Vaccine Name (Covishield, Hepatitis B)',
                                ),
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                flex: 2,
                                child: EmrCompactTextField(
                                  controller: item.lotNumberCtrl,
                                  labelText: 'Lot / Batch No.',
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          Row(
                            children: [
                              Expanded(
                                flex: 2,
                                child: EmrCompactTextField(
                                  controller: item.manufacturerCtrl,
                                  labelText: 'Manufacturer / Brand',
                                  hintText: 'e.g. Serum Institute of India',
                                ),
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                flex: 1,
                                child: EmrCompactTextField(
                                  controller: item.doseNumberCtrl,
                                  labelText: 'Dose No.',
                                ),
                              ),
                              IconButton(
                                icon: const Icon(Icons.delete_outline_rounded,
                                    color: Colors.redAccent, size: 20),
                                onPressed: () =>
                                    controller.removeImmunization(idx),
                              ),
                            ],
                          ),
                        ],
                      ),
                    );
                  },
                )),
          ],
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PAGE 6: Wellness Record
// ═══════════════════════════════════════════════════════════════════════════════
class EmrWellnessPage extends GetView<HealthRecordController> {
  const EmrWellnessPage({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return EmrFormShell(
      hiIcon: Icons.favorite_rounded,
      hiGradient: const [Color(0xFFDC2626), Color(0xFFB91C1C)],
      hiTitle: 'WellnessRecord',
      hiSubtitle: 'Vitals & Health Assessment',
      formBody: Container(
        padding: const EdgeInsets.all(12),
        decoration: glassDecoration(),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.favorite_rounded,
                    color: Color(0xFFDC2626), size: 20),
                const SizedBox(width: 8),
                Text('Wellness Record & Health Assessment',
                    style: fontBold.copyWith(fontSize: 15)),
              ],
            ),
            const SizedBox(height: 12),

            // Vital Signs
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                  color: AppColor.surface,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColor.border)),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('VITAL SIGNS',
                      style: fontBold.copyWith(
                          fontSize: 12, color: const Color(0xFF475569))),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Expanded(
                          child: _wellField('RESPIRATORY RATE (/MIN)',
                              controller.wellRespRateCtrl, '16')),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellField('HEART RATE (/MIN)',
                              controller.wellHeartRateCtrl, '72')),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellField(
                              'SPO2 (%)', controller.wellSpo2Ctrl, '98')),
                    ],
                  ),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Expanded(
                          child: _wellField('BODY TEMP (°F)',
                              controller.wellTempCtrl, '98.6')),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellField('SYSTOLIC BP (MMHG)',
                              controller.wellSysBpCtrl, '120')),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellField('DIASTOLIC BP (MMHG)',
                              controller.wellDiaBpCtrl, '80')),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),

            // Body Measurements
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                  color: AppColor.surface,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColor.border)),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('BODY MEASUREMENTS',
                      style: fontBold.copyWith(
                          fontSize: 12, color: const Color(0xFF475569))),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Expanded(
                          child: _wellFieldWithChange(
                              'HEIGHT (CM)',
                              controller.wellHeightCtrl,
                              '170',
                              (_) => controller.calculateWellBmi())),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellFieldWithChange(
                              'WEIGHT (KG)',
                              controller.wellWeightCtrl,
                              '68',
                              (_) => controller.calculateWellBmi())),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellField(
                              'BMI (KG/M²)', controller.wellBmiCtrl, '23.5')),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellField(
                              'WAIST (CM)', controller.wellWaistCtrl, '80')),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),

            // Lifestyle & Habits
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                  color: AppColor.surface,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColor.border)),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('LIFESTYLE & HABITS',
                      style: fontBold.copyWith(
                          fontSize: 12, color: const Color(0xFF475569))),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Expanded(
                          child: _wellDropdown(
                              'DIET TYPE',
                              controller.wellDietType,
                              ['veg', 'non-veg', 'vegan', 'eggetarian'])),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellDropdown(
                              'TOBACCO USE',
                              controller.wellTobaccoUse,
                              ['no', 'yes', 'former'])),
                      const SizedBox(width: 10),
                      Expanded(
                          child: _wellDropdown(
                              'ALCOHOL CONSUMPTION',
                              controller.wellAlcoholConsumption,
                              ['no', 'moderate', 'heavy'])),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),

            Text('OTHER HEALTH OBSERVATIONS & NOTES',
                style: TextStyle(
                    color: AppColor.textSecondary,
                    fontSize: 10.5,
                    fontWeight: FontWeight.bold,
                    letterSpacing: 0.5)),
            const SizedBox(height: 4),
            EmrCompactTextField(
              controller: controller.wellOtherObsCtrl,
              labelText: 'OTHER HEALTH OBSERVATIONS & NOTES',
              hintText:
                  'Enter general health assessment or doctor wellness notes...',
              maxLines: 2,
            ),
          ],
        ),
      ),
    );
  }

  Widget _wellField(String label, TextEditingController ctrl, String hint) {
    return EmrCompactTextField(
      controller: ctrl,
      labelText: label,
      hintText: hint,
    );
  }

  Widget _wellFieldWithChange(String label, TextEditingController ctrl,
      String hint, Function(String) onChanged) {
    return EmrCompactTextField(
      controller: ctrl,
      labelText: label,
      hintText: hint,
      onChanged: onChanged,
    );
  }

  Widget _wellDropdown(String label, RxString rxVal, List<String> options) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label,
            style:
                const TextStyle(fontSize: 10.5, fontWeight: FontWeight.bold)),
        const SizedBox(height: 4),
        Obx(() => Container(
              height: 36,
              padding: const EdgeInsets.symmetric(horizontal: 8),
              decoration: BoxDecoration(
                color: AppColor.background,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppColor.border),
              ),
              child: DropdownButtonHideUnderline(
                child: DropdownButton<String>(
                  value: rxVal.value,
                  isExpanded: true,
                  dropdownColor: AppColor.surface,
                  style: TextStyle(color: AppColor.textPrimary, fontSize: 12),
                  items: options
                      .map((opt) => DropdownMenuItem<String>(
                          value: opt, child: Text(opt)))
                      .toList(),
                  onChanged: (val) {
                    if (val != null) rxVal.value = val;
                  },
                ),
              ),
            )),
      ],
    );
  }
}

// ═══════════════════════════════════════════════════════════════════════════════
// FIXED LEFT SIDEBAR: PatientLeftSidebarPanel
// Permanent left-side panel for patient search and live selection.
// ═══════════════════════════════════════════════════════════════════════════════
class PatientLeftSidebarPanel extends StatefulWidget {
  final HealthRecordController controller;
  const PatientLeftSidebarPanel({Key? key, required this.controller})
      : super(key: key);

  @override
  State<PatientLeftSidebarPanel> createState() =>
      _PatientLeftSidebarPanelState();
}

class _PatientLeftSidebarPanelState extends State<PatientLeftSidebarPanel> {
  final TextEditingController _searchCtrl = TextEditingController();
  String _query = '';
  int _selectedTab = 0; // 0 = Patients, 1 = Saved Records

  @override
  void dispose() {
    _searchCtrl.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 680,
      decoration: BoxDecoration(
        color: AppColor.surface,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColor.border),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.04),
            blurRadius: 6,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Sidebar Tab Header: Patients vs Saved EMR Records
          Container(
            padding: const EdgeInsets.all(4),
            decoration: const BoxDecoration(
              color: Color(0xFF334155),
              borderRadius: BorderRadius.only(
                topLeft: Radius.circular(11),
                topRight: Radius.circular(11),
              ),
            ),
            child: Row(
              children: [
                Expanded(
                  child: InkWell(
                    onTap: () => setState(() => _selectedTab = 0),
                    borderRadius: BorderRadius.circular(8),
                    child: Container(
                      padding: const EdgeInsets.symmetric(vertical: 8),
                      decoration: BoxDecoration(
                        color: _selectedTab == 0
                            ? Colors.white
                            : Colors.transparent,
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Center(
                        child: Text(
                          '👥 Patients (${widget.controller.patients.length})',
                          style: TextStyle(
                            color: _selectedTab == 0
                                ? const Color(0xFF334155)
                                : Colors.white70,
                            fontSize: 11.5,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 4),
                Expanded(
                  child: Obx(() => InkWell(
                        onTap: () => setState(() => _selectedTab = 1),
                        borderRadius: BorderRadius.circular(8),
                        child: Container(
                          padding: const EdgeInsets.symmetric(vertical: 8),
                          decoration: BoxDecoration(
                            color: _selectedTab == 1
                                ? Colors.white
                                : Colors.transparent,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Center(
                            child: Text(
                              '📂 Saved EMRs (${widget.controller.savedLocalRecords.length})',
                              style: TextStyle(
                                color: _selectedTab == 1
                                    ? const Color(0xFF334155)
                                    : Colors.white70,
                                fontSize: 11.5,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ),
                      )),
                ),
              ],
            ),
          ),

          if (_selectedTab == 0) ...[
            // Active Patient Banner (If selected)
            Obx(() {
              final active = widget.controller.selectedPatient.value;
              if (active == null) {
                return Container(
                  width: double.infinity,
                  padding:
                      const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  color: const Color(0xFFFEF3C7),
                  child: Row(
                    children: const [
                      Icon(Icons.warning_amber_rounded,
                          color: Color(0xFFD97706), size: 16),
                      SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          'Select a patient from list below',
                          style: TextStyle(
                              color: Color(0xFFB45309),
                              fontSize: 11,
                              fontWeight: FontWeight.w600),
                        ),
                      ),
                    ],
                  ),
                );
              }

              return Container(
                width: double.infinity,
                padding:
                    const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                color: const Color(0xFF10B981).withOpacity(0.12),
                child: Row(
                  children: [
                    const Icon(Icons.check_circle_rounded,
                        color: Color(0xFF059669), size: 16),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Active: ${active.name}',
                            style: const TextStyle(
                                color: Color(0xFF047857),
                                fontSize: 12,
                                fontWeight: FontWeight.bold),
                          ),
                          Text(
                            active.abhaAddress,
                            style: const TextStyle(
                                color: Color(0xFF059669), fontSize: 10.5),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              );
            }),

            // Search Field Input
            Padding(
              padding: const EdgeInsets.all(10),
              child: EmrCompactTextField(
                controller: _searchCtrl,
                labelText: 'Search Patient',
                hintText: 'Search Name, ABHA, Mobile...',
              ),
            ),

            const Divider(height: 1),

            // Patient List View
            Expanded(
              child: Obx(() {
                if (widget.controller.isLoadingPatients.value) {
                  return const Center(
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: Color(0xFF475569),
                    ),
                  );
                }

                final patients = widget.controller.groupedPatientsList;
                final selected = widget.controller.selectedPatient.value;

                if (patients.isEmpty) {
                  return const Center(
                    child: Padding(
                      padding: EdgeInsets.all(16.0),
                      child: Text(
                        'No patients registered',
                        style: TextStyle(color: Colors.grey, fontSize: 12),
                      ),
                    ),
                  );
                }

                final filtered = patients.where((p) {
                  if (_query.isEmpty) return true;
                  return p.name.toLowerCase().contains(_query) ||
                      p.mobile.toLowerCase().contains(_query) ||
                      p.models.any((m) => m.abhaAddress.toLowerCase().contains(_query));
                }).toList();

                if (filtered.isEmpty) {
                  return const Center(
                    child: Padding(
                      padding: EdgeInsets.all(16.0),
                      child: Text(
                        'No matching patients',
                        style: TextStyle(color: Colors.grey, fontSize: 12),
                      ),
                    ),
                  );
                }

                return ListView.separated(
                  padding: const EdgeInsets.all(8),
                  itemCount: filtered.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 6),
                  itemBuilder: (context, idx) {
                    final p = filtered[idx];

                    return Obx(() {
                      final activeModel = p.selectedModel;
                      final isSelected = selected?.abhaAddress.toLowerCase() == activeModel.abhaAddress.toLowerCase();

                      return InkWell(
                        onTap: () {
                          widget.controller.selectedPatient.value = activeModel;
                          widget.controller.fetchSavedHealthRecords(activeModel.abhaAddress);
                        },
                        borderRadius: BorderRadius.circular(8),
                        child: AnimatedContainer(
                          duration: const Duration(milliseconds: 150),
                          padding: const EdgeInsets.all(10),
                          decoration: BoxDecoration(
                            color: isSelected
                                ? const Color(0xFF475569).withOpacity(0.12)
                                : AppColor.background,
                            borderRadius: BorderRadius.circular(8),
                            border: Border.all(
                              color: isSelected
                                  ? const Color(0xFF475569)
                                  : AppColor.border,
                              width: isSelected ? 1.5 : 1.0,
                            ),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  CircleAvatar(
                                    radius: 14,
                                    backgroundColor: isSelected
                                        ? const Color(0xFF475569)
                                        : const Color(0xFF64748B).withOpacity(0.15),
                                    child: Text(
                                      p.name.isNotEmpty
                                          ? p.name[0].toUpperCase()
                                          : 'P',
                                      style: TextStyle(
                                        color: isSelected
                                            ? Colors.white
                                            : const Color(0xFF334155),
                                        fontWeight: FontWeight.bold,
                                        fontSize: 11,
                                      ),
                                    ),
                                  ),
                                  const SizedBox(width: 8),
                                  Expanded(
                                    child: Text(
                                      p.name,
                                      style: TextStyle(
                                        color: AppColor.textPrimary,
                                        fontWeight: isSelected
                                            ? FontWeight.bold
                                            : FontWeight.w600,
                                        fontSize: 12.5,
                                      ),
                                      overflow: TextOverflow.ellipsis,
                                    ),
                                  ),
                                  if (isSelected)
                                    const Icon(Icons.check_circle_rounded,
                                        color: Color(0xFF475569), size: 16),
                                ],
                              ),
                              const SizedBox(height: 6),
                              if (p.models.length > 1) ...[
                                Container(
                                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                                  decoration: BoxDecoration(
                                    color: Colors.white.withOpacity(0.6),
                                    borderRadius: BorderRadius.circular(6),
                                    border: Border.all(color: AppColor.border),
                                  ),
                                  child: DropdownButtonHideUnderline(
                                    child: DropdownButton<String>(
                                      value: p.selectedAbhaAddress,
                                      isDense: true,
                                      isExpanded: true,
                                      style: fontMedium.copyWith(fontSize: 11, color: const Color(0xFF2563EB)),
                                      items: p.models.map((m) {
                                        return DropdownMenuItem<String>(
                                          value: m.abhaAddress,
                                          child: Text(m.abhaAddress, overflow: TextOverflow.ellipsis),
                                        );
                                      }).toList(),
                                      onChanged: (val) {
                                        if (val != null) {
                                          p.selectedAbhaAddress = val;
                                          widget.controller.selectedPatient.value = p.selectedModel;
                                          widget.controller.fetchSavedHealthRecords(val);
                                        }
                                      },
                                    ),
                                  ),
                                ),
                              ] else ...[
                                Padding(
                                  padding: const EdgeInsets.only(left: 4.0),
                                  child: Text(
                                    p.selectedAbhaAddress,
                                    style: const TextStyle(
                                      color: Color(0xFF2563EB),
                                      fontSize: 11,
                                      fontWeight: FontWeight.w500,
                                    ),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                              ],
                              if (activeModel.abhaNumber.isNotEmpty) ...[
                                const SizedBox(height: 4),
                                Padding(
                                  padding: const EdgeInsets.only(left: 4.0),
                                  child: Text(
                                    "No: ${activeModel.abhaNumber}",
                                    style: const TextStyle(
                                      color: Colors.grey,
                                      fontSize: 10,
                                      fontWeight: FontWeight.w400,
                                    ),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                              ],
                              if (activeModel.pincode.isNotEmpty) ...[
                                const SizedBox(height: 2),
                                Padding(
                                  padding: const EdgeInsets.only(left: 4.0),
                                  child: Text(
                                    "Pin: ${activeModel.pincode}",
                                    style: const TextStyle(
                                      color: Colors.grey,
                                      fontSize: 10,
                                      fontWeight: FontWeight.w400,
                                    ),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                ),
                              ],
                            ],
                          ),
                        ),
                      );
                    });
                  },
                );
              }),
            ),
          ] else ...[
            // Tab 1: Saved Records List (with Link to ABDM Gateway Button)
            Expanded(
              child: Obx(() {
                final records = widget.controller.savedLocalRecords;
                if (records.isEmpty) {
                  return Center(
                    child: Padding(
                      padding: const EdgeInsets.all(20.0),
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: const [
                          Icon(Icons.folder_open_rounded,
                              size: 40, color: Colors.grey),
                          SizedBox(height: 10),
                          Text(
                            'No Saved Records Yet',
                            style: TextStyle(
                                fontWeight: FontWeight.bold, fontSize: 13),
                          ),
                          SizedBox(height: 4),
                          Text(
                            'Fill any EMR form and click "Save Record 💾" to store records here.',
                            textAlign: TextAlign.center,
                            style: TextStyle(color: Colors.grey, fontSize: 11),
                          ),
                        ],
                      ),
                    ),
                  );
                }

                return ListView.separated(
                  padding: const EdgeInsets.all(8),
                  itemCount: records.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, idx) {
                    final rec = records[idx];
                    return Container(
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        color: AppColor.background,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(
                          color: rec.isLinked
                              ? const Color(0xFF10B981)
                              : const Color(0xFFF59E0B),
                          width: 1.2,
                        ),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 6, vertical: 2),
                                decoration: BoxDecoration(
                                  color: rec.isLinked
                                      ? const Color(0xFF10B981)
                                          .withOpacity(0.15)
                                      : const Color(0xFFF59E0B)
                                          .withOpacity(0.15),
                                  borderRadius: BorderRadius.circular(4),
                                ),
                                child: Text(
                                  rec.hiType,
                                  style: TextStyle(
                                    color: rec.isLinked
                                        ? const Color(0xFF047857)
                                        : const Color(0xFFB45309),
                                    fontSize: 10.5,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                              const Spacer(),
                              Text(
                                rec.isLinked ? 'Linked 🟢' : 'Local Only 💾',
                                style: TextStyle(
                                  color: rec.isLinked
                                      ? const Color(0xFF047857)
                                      : const Color(0xFFB45309),
                                  fontSize: 10.5,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 6),
                          Text(
                            rec.visitRef,
                            style: const TextStyle(
                                fontWeight: FontWeight.bold, fontSize: 12),
                          ),
                          Text(
                            'Patient: ${rec.patientName}',
                            style: TextStyle(
                                color: AppColor.textPrimary,
                                fontWeight: FontWeight.w600,
                                fontSize: 11),
                          ),
                          Text(
                            'ABHA: ${rec.abhaAddress}',
                            style: const TextStyle(
                                color: Color(0xFF2563EB), fontSize: 10.5),
                          ),
                          Text(
                            'Saved: ${rec.createdTime}',
                            style: TextStyle(
                                color: AppColor.textSecondary.withOpacity(0.7),
                                fontSize: 10),
                          ),
                          if (!rec.isLinked) ...[
                            const SizedBox(height: 8),
                            EmrPrimaryButton(
                              label: 'Link to ABDM Gateway',
                              icon: Icons.cloud_upload_rounded,
                              backgroundColor: const Color(0xFF10B981),
                              height: 32,
                              isLoading:
                                  widget.controller.isSavingHealthRecord.value,
                              onPressed: () =>
                                  widget.controller.linkSingleRecordToAbdm(rec),
                            ),
                          ],
                        ],
                      ),
                    );
                  },
                );
              }),
            ),
          ],
        ],
      ),
    );
  }
}
