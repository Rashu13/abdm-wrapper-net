import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '@/api/abhaApi';
import {
  linkCareContextsApi,
  saveCareContextsApi,
  fetchPatientCareContextsApi,
  sendDeepLinkSmsApi,
  PatientCareContextRecord,
  CareContextItem,
} from '@/api/careContextApi';

interface PatientItem {
  patientId: number;
  uhid: string;
  abhaNumber: string;
  name: string;
  gender?: string;
  dob?: string;
  mobile?: string;
  address?: string;
  city?: string;
  state?: string;
  pincode?: string;
  rawPayloadJson?: string;
  createdAt: string;
  abhaAddresses: string[];
}

interface MedicineItem {
  id: string;
  drugName: string;
  dosagePattern: string;
  route: string;
  method: string;
  reason: string;
}

interface DiagnosticResultItem {
  id: string;
  testName: string;
  value: string;
  unit: string;
}

interface ComplaintItem {
  id: string;
  complaint: string;
}

interface PhysicalExamItem {
  id: string;
  name: string;
  value: string;
  unit: string;
}

interface AllergyItem {
  id: string;
  allergyName: string;
  clinicalStatus: string;
}

interface MedicalHistoryItem {
  id: string;
  condition: string;
}

interface DiagObsItem {
  id: string;
  reportTitle: string;
  testName: string;
  value: string;
  unit: string;
}

interface ProcedureItem {
  id: string;
  procedureName: string;
  status: string;
}

interface FamilyHistoryItem {
  id: string;
  familyHistory: string;
}

interface CarePlanItem {
  id: string;
  intent: string;
  description: string;
}

interface AppointmentItem {
  id: string;
  startDate: string;
  endDate: string;
  status: string;
  description: string;
}

interface ImmunizationItem {
  id: string;
  vaccineName: string;
  manufacturer: string;
  vaccineCode: string;
  status: string;
  lotNumber: string;
  doseNumber: string;
  occurrence: string;
  route: string;
  site: string;
  doseQty: string;
  unit: string;
}

interface RecommendationItem {
  id: string;
  nextVaccineName: string;
  nextVaccineCode: string;
  status: string;
  dueDays: string;
}

interface InvoiceItem {
  id: string;
  medicineName: string;
  snomedCode: string;
  form: string;
  hsn: string;
  lotNumber: string;
  expiryDate: string;
  quantity: string;
  unit: string;
  rate: string;
  mrp: string;
  discount: string;
  cgst: string;
  sgst: string;
}

const HI_TYPES = [
  'Prescription',
  'DiagnosticReport',
  'DischargeSummary',
  'OPConsultation',
  'ImmunizationRecord',
  'WellnessRecord',
  'InvoiceRecord',
  'HealthDocumentRecord',
];

// Helper to format ISO datetime-local string (YYYY-MM-DDTHH:mm) to display string (DD-MM-YYYY HH:mm)
const formatToDisplayDate = (isoStr: string): string => {
  if (!isoStr) return '';
  if (isoStr.includes('T')) {
    const [dPart, tPart] = isoStr.split('T');
    const [y, m, d] = dPart.split('-');
    return `${d}-${m}-${y} ${tPart}`;
  }
  return isoStr;
};

// Helper to get ISO datetime-local string (YYYY-MM-DDTHH:mm)
const getIsoNowString = (offsetDays: number = 0, hour: number = -1): string => {
  const d = new Date();
  if (offsetDays !== 0) d.setDate(d.getDate() + offsetDays);
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  const hh = String(hour >= 0 ? hour : d.getHours()).padStart(2, '0');
  const min = String(d.getMinutes()).padStart(2, '0');
  return `${yyyy}-${mm}-${dd}T${hh}:${min}`;
};

const HealthRecordsPage: React.FC = () => {
  const navigate = useNavigate();
  const [patients, setPatients] = useState<PatientItem[]>([]);
  const [, setLoadingPatients] = useState<boolean>(true);
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [showSearchDropdown, setShowSearchDropdown] = useState<boolean>(false);
  const [highlightedPatientIndex, setHighlightedPatientIndex] = useState<number>(-1);
  const [selectedPatient, setSelectedPatient] = useState<PatientItem | null>(null);
  const [selectedAbhaAddress, setSelectedAbhaAddress] = useState<string>('');

  // Active HI Type pill state (default 'Prescription')
  const [activeHiType, setActiveHiType] = useState<string>('Prescription');

  // Modal / Form state for Add Care Context
  const [isAddModalOpen, setIsAddModalOpen] = useState<boolean>(false);
  const [contextRef, setContextRef] = useState<string>('');
  const [contextDisplay, setContextDisplay] = useState<string>('Prescription');
  const [encounterType, setEncounterType] = useState<string>('Outpatient');
  const [reportName, setReportName] = useState<string>('');
  const [visitDate, setVisitDate] = useState<string>(() => getIsoNowString());

  const [medicines, setMedicines] = useState<MedicineItem[]>([
    {
      id: '1',
      drugName: '',
      dosagePattern: 'Morning Only (1-0-0)',
      route: 'Oral',
      method: 'After Food',
      reason: '',
    },
  ]);

  const [labResults, setLabResults] = useState<DiagnosticResultItem[]>([
    { id: '1', testName: '', value: '', unit: '' },
  ]);
  const [diagnosticSummary, setDiagnosticSummary] = useState<string>('');

  // Discharge Summary fields state
  const [admissionDate, setAdmissionDate] = useState<string>(() => getIsoNowString(-3, 10));
  const [dischargeDate, setDischargeDate] = useState<string>(() => getIsoNowString(0, 14));

  // Discharge Summary Dynamic List States
  const [complaintsList, setComplaintsList] = useState<ComplaintItem[]>([
    { id: '1', complaint: '' }
  ]);
  const [physicalExamsList, setPhysicalExamsList] = useState<PhysicalExamItem[]>([
    { id: '1', name: '', value: '', unit: '' }
  ]);
  const [allergiesList, setAllergiesList] = useState<AllergyItem[]>([
    { id: '1', allergyName: '', clinicalStatus: 'Active' }
  ]);
  const [medicalHistoryList, setMedicalHistoryList] = useState<MedicalHistoryItem[]>([
    { id: '1', condition: '' }
  ]);
  const [diagObsList, setDiagObsList] = useState<DiagObsItem[]>([
    { id: '1', reportTitle: '', testName: '', value: '', unit: '' }
  ]);
  const [proceduresList, setProceduresList] = useState<ProcedureItem[]>([
    { id: '1', procedureName: '', status: 'Completed' }
  ]);
  const [dischargeMedsList, setDischargeMedsList] = useState<MedicineItem[]>([
    { id: '1', drugName: '', dosagePattern: '1-0-1', route: 'Oral', method: 'After Food', reason: '' }
  ]);
  const [familyHistoryList, setFamilyHistoryList] = useState<FamilyHistoryItem[]>([
    { id: '1', familyHistory: '' }
  ]);
  const [carePlansList, setCarePlansList] = useState<CarePlanItem[]>([
    { id: '1', intent: 'plan', description: '' }
  ]);
  const [appointmentsList, setAppointmentsList] = useState<AppointmentItem[]>([
    { id: '1', startDate: '', endDate: '', status: 'Completed', description: '' }
  ]);

  // OP Consultation States
  const [opHeight, setOpHeight] = useState<string>('');
  const [opWeight, setOpWeight] = useState<string>('');
  const [opBmi, setOpBmi] = useState<string>('');
  const [vitalsList, setVitalsList] = useState<{ id: string; vitalName: string; value: string; unit: string }[]>([
    { id: '1', vitalName: '', value: '', unit: '' }
  ]);
  const [opComplaintsList, setOpComplaintsList] = useState<ComplaintItem[]>([
    { id: '1', complaint: '' }
  ]);
  const [opObservationResult, setOpObservationResult] = useState<string>('');
  const [opAllergiesList, setOpAllergiesList] = useState<{ id: string; allergyName: string; type: string; status: string }[]>([
    { id: '1', allergyName: '', type: 'medication', status: 'active' }
  ]);
  const [opMedicalHistoryList, setOpMedicalHistoryList] = useState<MedicalHistoryItem[]>([
    { id: '1', condition: '' }
  ]);

  // Immunization States
  const [immunizationsList, setImmunizationsList] = useState<ImmunizationItem[]>([
    {
      id: '1',
      vaccineName: '',
      manufacturer: '',
      vaccineCode: '',
      status: 'completed',
      lotNumber: '',
      doseNumber: '1',
      occurrence: getIsoNowString(),
      route: 'Intramuscular',
      site: 'Left arm',
      doseQty: '0.5',
      unit: 'mL'
    }
  ]);
  const [recommendationsList, setRecommendationsList] = useState<RecommendationItem[]>([
    { id: '1', nextVaccineName: '', nextVaccineCode: '', status: 'due', dueDays: '84' }
  ]);

  // Wellness Record States
  const [wellRespRate, setWellRespRate] = useState<string>('');
  const [wellHeartRate, setWellHeartRate] = useState<string>('');
  const [wellSpo2, setWellSpo2] = useState<string>('');
  const [wellTemp, setWellTemp] = useState<string>('');
  const [wellSysBp, setWellSysBp] = useState<string>('');
  const [wellDiaBp, setWellDiaBp] = useState<string>('');
  const [wellHeight, setWellHeight] = useState<string>('');
  const [wellWeight, setWellWeight] = useState<string>('');
  const [wellBmi, setWellBmi] = useState<string>('');
  const [wellWaist, setWellWaist] = useState<string>('');
  const [wellMenarcheAge, setWellMenarcheAge] = useState<string>('');
  const [wellLmpDate, setWellLmpDate] = useState<string>('');
  const [wellDietType, setWellDietType] = useState<string>('veg');
  const [wellTobaccoUse, setWellTobaccoUse] = useState<string>('no');
  const [wellAlcoholConsumption, setWellAlcoholConsumption] = useState<string>('no');
  const [wellOtherObs, setWellOtherObs] = useState<string>('');

  // Invoice Record States
  const [invoiceNumber, setInvoiceNumber] = useState<string>('');
  const [invoiceDate, setInvoiceDate] = useState<string>(getIsoNowString());
  const [invoiceTotalNet, setInvoiceTotalNet] = useState<string>('');
  const [invoiceTotalGross, setInvoiceTotalGross] = useState<string>('');
  const [invoiceItemsList, setInvoiceItemsList] = useState<InvoiceItem[]>([
    {
      id: '1',
      medicineName: '',
      snomedCode: '',
      form: 'tab',
      hsn: '',
      lotNumber: '',
      expiryDate: '',
      quantity: '1',
      unit: 'Tab',
      rate: '',
      mrp: '',
      discount: '0',
      cgst: '0',
      sgst: '0'
    }
  ]);

  // PDF / Document Upload State
  const [_selectedFileBase64, setSelectedFileBase64] = useState<string>('');
  const [fileName, setFileName] = useState<string>('');
  const [fileSize, setFileSize] = useState<number>(0);
  const [_fileMime, setFileMime] = useState<string>('application/pdf');

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) {
      setSelectedFileBase64('');
      setFileName('');
      setFileSize(0);
      return;
    }
    setFileName(file.name);
    setFileSize(file.size);
    setFileMime(file.type || 'application/pdf');

    const reader = new FileReader();
    reader.onload = () => {
      const resultStr = reader.result as string;
      const base64Content = resultStr.includes(',') ? resultStr.split(',')[1] : resultStr;
      setSelectedFileBase64(base64Content);
    };
    reader.readAsDataURL(file);
  };

  const [isLinking, setIsLinking] = useState<boolean>(false);
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [isSendingSms, setIsSendingSms] = useState<boolean>(false);

  // Care context history & logs
  const [linkedRecords, setLinkedRecords] = useState<PatientCareContextRecord[]>([]);
  const [loadingHistory, setLoadingHistory] = useState<boolean>(false);
  const [statusMessage, setStatusMessage] = useState<{ type: 'success' | 'error' | 'info'; message: string } | null>(null);

  // Fetch registered patients from real database API
  const fetchPatients = async () => {
    try {
      setLoadingPatients(true);
      const res = await api.get<{ success: boolean; data: PatientItem[] }>('/api/patient/list');
      if (res.data.success && res.data.data.length > 0) {
        setPatients(res.data.data);
      }
    } catch (err) {
      console.error('Failed to load patients', err);
    } finally {
      setLoadingPatients(false);
    }
  };

  useEffect(() => {
    fetchPatients();
  }, []);

  // Filter patients based on search input
  const filteredPatients = patients.filter((p) => {
    const q = searchQuery.toLowerCase().trim();
    if (!q) return true;
    return (
      (p.name && p.name.toLowerCase().includes(q)) ||
      (p.uhid && p.uhid.toLowerCase().includes(q)) ||
      (p.abhaNumber && p.abhaNumber.toLowerCase().includes(q)) ||
      (p.mobile && p.mobile.includes(q)) ||
      p.abhaAddresses.some((addr) => addr.toLowerCase().includes(q))
    );
  });

  // Calculate age string from real patient DOB
  const calculateAgeShort = (dobStr?: string): string => {
    if (!dobStr) return 'N/A';
    try {
      let birthDate: Date;
      if (dobStr.includes('-')) {
        const parts = dobStr.split('-');
        birthDate = parts[0].length === 4 ? new Date(dobStr) : new Date(`${parts[2]}-${parts[1]}-${parts[0]}`);
      } else if (dobStr.includes('/')) {
        const parts = dobStr.split('/');
        birthDate = new Date(`${parts[2]}-${parts[1]}-${parts[0]}`);
      } else {
        birthDate = new Date(dobStr);
      }
      if (isNaN(birthDate.getTime())) return dobStr;

      const today = new Date();
      let age = today.getFullYear() - birthDate.getFullYear();
      const m = today.getMonth() - birthDate.getMonth();
      if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) age--;
      return `${Math.max(0, age)}y`;
    } catch {
      return dobStr;
    }
  };

  // Select patient handler
  const handleSelectPatient = (patient: PatientItem) => {
    setSelectedPatient(patient);
    setSearchQuery(patient.name);
    setShowSearchDropdown(false);
    const primaryAddr = patient.abhaAddresses && patient.abhaAddresses.length > 0
      ? patient.abhaAddresses[0]
      : `${patient.name.toLowerCase().replace(/\s+/g, '')}@sbx`;
    setSelectedAbhaAddress(primaryAddr);

    const timestamp = new Date().toISOString().replace(/[-:T.]/g, '').slice(2, 10);
    const randomSeq = Math.floor(100 + Math.random() * 900);
    setContextRef(`${activeHiType.toUpperCase().slice(0, 4)}-${timestamp}-${randomSeq}`);
    setContextDisplay(activeHiType);
    setStatusMessage(null);

    loadCareContextHistory(patient.patientId, primaryAddr);
  };

  // Keyboard navigation for patient search dropdown (ArrowUp, ArrowDown, Enter, Escape)
  const handlePatientSearchKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (!showSearchDropdown || filteredPatients.length === 0) return;

    if (e.key === 'ArrowDown') {
      e.preventDefault();
      setHighlightedPatientIndex((prev) => (prev < filteredPatients.length - 1 ? prev + 1 : 0));
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      setHighlightedPatientIndex((prev) => (prev > 0 ? prev - 1 : filteredPatients.length - 1));
    } else if (e.key === 'Enter') {
      e.preventDefault();
      const targetIndex = highlightedPatientIndex >= 0 && highlightedPatientIndex < filteredPatients.length ? highlightedPatientIndex : 0;
      if (filteredPatients[targetIndex]) {
        handleSelectPatient(filteredPatients[targetIndex]);
      }
    } else if (e.key === 'Escape') {
      setShowSearchDropdown(false);
    }
  };

  // Load patient care context history from database
  const loadCareContextHistory = async (patientId: number, abhaAddress: string) => {
    try {
      setLoadingHistory(true);
      const records = await fetchPatientCareContextsApi(patientId, abhaAddress);
      setLinkedRecords(records);
    } catch (err) {
      console.error('Failed to load history', err);
    } finally {
      setLoadingHistory(false);
    }
  };

  // Open Add Modal
  const handleOpenAddModal = () => {
    const timestamp = new Date().toISOString().replace(/[-:T.]/g, '').slice(2, 10);
    const randomSeq = Math.floor(100 + Math.random() * 900);
    setContextRef(`${activeHiType.toUpperCase().slice(0, 4)}-${timestamp}-${randomSeq}`);

    let defaultDisplay = activeHiType;
    if (activeHiType === 'DiagnosticReport') defaultDisplay = 'Diagnostic Report';
    else if (activeHiType === 'DischargeSummary') defaultDisplay = 'Discharge Summary';
    else if (activeHiType === 'HealthDocumentRecord') defaultDisplay = 'Health Document';
    else if (activeHiType === 'OPConsultation') defaultDisplay = 'OP Consultation';
    else if (activeHiType === 'ImmunizationRecord') defaultDisplay = 'Immunization Record';
    else if (activeHiType === 'WellnessRecord') defaultDisplay = 'Wellness Record';

    setContextDisplay(defaultDisplay);
    setReportName('');
    setDiagnosticSummary('');
    setSelectedFileBase64('');
    setFileName('');
    setFileSize(0);
    setMedicines([
      { id: '1', drugName: '', dosagePattern: 'Morning Only (1-0-0)', route: 'Oral', method: 'After Food', reason: '' },
    ]);
    setLabResults([
      { id: '1', testName: '', value: '', unit: '' },
    ]);
    setComplaintsList([{ id: '1', complaint: '' }]);
    setPhysicalExamsList([{ id: '1', name: '', value: '', unit: '' }]);
    setAllergiesList([{ id: '1', allergyName: '', clinicalStatus: 'Active' }]);
    setMedicalHistoryList([{ id: '1', condition: '' }]);
    setDiagObsList([{ id: '1', reportTitle: '', testName: '', value: '', unit: '' }]);
    setProceduresList([{ id: '1', procedureName: '', status: 'Completed' }]);
    setDischargeMedsList([{ id: '1', drugName: '', dosagePattern: '1-0-1', route: 'Oral', method: 'After Food', reason: '' }]);
    setFamilyHistoryList([{ id: '1', familyHistory: '' }]);
    setCarePlansList([{ id: '1', intent: 'plan', description: '' }]);
    setAppointmentsList([{ id: '1', startDate: '', endDate: '', status: 'Completed', description: '' }]);
    setOpHeight('');
    setOpWeight('');
    setOpBmi('');
    setVitalsList([{ id: '1', vitalName: '', value: '', unit: '' }]);
    setOpComplaintsList([{ id: '1', complaint: '' }]);
    setOpObservationResult('');
    setOpAllergiesList([{ id: '1', allergyName: '', type: 'medication', status: 'active' }]);
    setOpMedicalHistoryList([{ id: '1', condition: '' }]);
    setImmunizationsList([{
      id: '1',
      vaccineName: '',
      manufacturer: '',
      vaccineCode: '',
      status: 'completed',
      lotNumber: '',
      doseNumber: '1',
      occurrence: getIsoNowString(),
      route: 'Intramuscular',
      site: 'Left arm',
      doseQty: '0.5',
      unit: 'mL'
    }]);
    setRecommendationsList([{ id: '1', nextVaccineName: '', nextVaccineCode: '', status: 'due', dueDays: '84' }]);
    setWellRespRate('');
    setWellHeartRate('');
    setWellSpo2('');
    setWellTemp('');
    setWellSysBp('');
    setWellDiaBp('');
    setWellHeight('');
    setWellWeight('');
    setWellBmi('');
    setWellWaist('');
    setWellMenarcheAge('');
    setWellLmpDate('');
    setWellDietType('veg');
    setWellTobaccoUse('no');
    setWellAlcoholConsumption('no');
    setWellOtherObs('');
    setInvoiceNumber('');
    setInvoiceDate(getIsoNowString());
    setInvoiceTotalNet('');
    setInvoiceTotalGross('');
    setInvoiceItemsList([
      {
        id: '1',
        medicineName: '',
        snomedCode: '',
        form: 'tab',
        hsn: '',
        lotNumber: '',
        expiryDate: '',
        quantity: '1',
        unit: 'Tab',
        rate: '',
        mrp: '',
        discount: '0',
        cgst: '0',
        sgst: '0'
      }
    ]);
    setIsAddModalOpen(true);
  };

  // Add Medicine Row
  const handleAddMedicineRow = () => {
    setMedicines((prev) => [
      ...prev,
      {
        id: String(Date.now()),
        drugName: '',
        dosagePattern: 'Morning Only (1-0-0)',
        route: 'Oral',
        method: 'After Food',
        reason: '',
      },
    ]);
  };

  // Update Medicine Field
  const handleUpdateMedicine = (id: string, field: keyof MedicineItem, value: string) => {
    setMedicines((prev) =>
      prev.map((med) => (med.id === id ? { ...med, [field]: value } : med))
    );
  };

  // Remove Medicine Row
  const handleRemoveMedicine = (id: string) => {
    if (medicines.length <= 1) return;
    setMedicines((prev) => prev.filter((med) => med.id !== id));
  };

  // Add Lab Result Row
  const handleAddLabResultRow = () => {
    setLabResults((prev) => [
      ...prev,
      { id: String(Date.now()), testName: '', value: '', unit: '' },
    ]);
  };

  // Update Lab Result Field
  const handleUpdateLabResult = (id: string, field: keyof DiagnosticResultItem, value: string) => {
    setLabResults((prev) =>
      prev.map((res) => (res.id === id ? { ...res, [field]: value } : res))
    );
  };

  // Remove Lab Result Row
  const handleRemoveLabResult = (id: string) => {
    if (labResults.length <= 1) return;
    setLabResults((prev) => prev.filter((res) => res.id !== id));
  };

  // Helper to construct display payload based on active HI type
  const buildDisplayPayload = (): { displayTitle: string; refNum: string; formattedDisplayPayload: string } => {
    const displayTitle = contextDisplay.trim() || (activeHiType === 'DiagnosticReport' ? 'Diagnostic Report' : activeHiType);
    const refNum = contextRef.trim() || `REF-${Date.now()}`;
    let formattedDisplayPayload = displayTitle;

    if (activeHiType === 'DiagnosticReport') {
      const validResults = labResults.filter((r) => r.testName && r.testName.trim().length > 0);
      const formattedList = validResults.map((r) => {
        const tName = r.testName.trim();
        const tVal = r.value ? r.value.trim() : '';
        const tUnit = r.unit ? r.unit.trim() : '';
        return {
          drugName: tVal ? `${tName}: ${tVal} ${tUnit}`.trim() : tName,
          dosagePattern: tVal ? `Value: ${tVal}` : 'Normal',
          route: tUnit ? `Unit: ${tUnit}` : '-',
          method: reportName.trim() ? `Report: ${reportName.trim()}` : 'Diagnostic Result',
          reason: '',
        };
      });

      if (diagnosticSummary.trim()) {
        formattedList.push({
          drugName: `Summary: ${diagnosticSummary.trim()}`,
          dosagePattern: 'Summary',
          route: '-',
          method: 'Conclusion',
          reason: '',
        });
      }

      formattedDisplayPayload = formattedList.length > 0
        ? `${reportName.trim() || displayTitle}||MEDS:${JSON.stringify(formattedList)}`
        : (reportName.trim() || displayTitle);
    } else if (activeHiType === 'DischargeSummary') {
      const validComplaints = complaintsList.filter(c => c.complaint.trim()).map(c => c.complaint.trim()).join('; ');
      const validHistory = medicalHistoryList.filter(m => m.condition.trim()).map(m => m.condition.trim()).join('; ');
      const validProcedures = proceduresList.filter(p => p.procedureName.trim()).map(p => `${p.procedureName} (${p.status})`).join('; ');
      const validMeds = dischargeMedsList.filter(m => m.drugName.trim()).map(m => `${m.drugName} ${m.dosagePattern}`).join('; ');

      const summaryMeds = [{
        medicine: `Admission: ${admissionDate} | Discharge: ${dischargeDate}`,
        dosage: validComplaints ? `Complaints: ${validComplaints}` : 'Chief Complaints',
        route: validMeds ? `Meds: ${validMeds}` : 'Medication Summary',
        method: validHistory ? `History: ${validHistory}` : (validProcedures ? `Procedures: ${validProcedures}` : 'Medical History')
      }];
      formattedDisplayPayload = `${displayTitle}||MEDS:${JSON.stringify(summaryMeds)}`;
    } else if (activeHiType === 'OPConsultation') {
      const validVitals = vitalsList.filter(v => v.vitalName.trim()).map(v => `${v.vitalName}: ${v.value} ${v.unit}`).join('; ');
      const validComplaints = opComplaintsList.filter(c => c.complaint.trim()).map(c => c.complaint.trim()).join('; ');
      const validHistory = opMedicalHistoryList.filter(m => m.condition.trim()).map(m => m.condition.trim()).join('; ');

      const opMeds = [{
        medicine: `Visit Date: ${formatToDisplayDate(visitDate)}`,
        dosage: opHeight || opWeight ? `Height: ${opHeight}cm | Weight: ${opWeight}kg | BMI: ${opBmi}` : (validVitals ? `Vitals: ${validVitals}` : 'Body Measurements'),
        route: validComplaints ? `Complaints: ${validComplaints}` : 'Chief Complaints',
        method: validHistory ? `History: ${validHistory}` : (opObservationResult ? `Obs: ${opObservationResult}` : 'Medical History')
      }];
      formattedDisplayPayload = `${displayTitle}||MEDS:${JSON.stringify(opMeds)}`;
    } else if (activeHiType === 'ImmunizationRecord') {
      const validImm = immunizationsList.filter(i => i.vaccineName.trim()).map(i => `${i.vaccineName} (Dose #${i.doseNumber})`).join('; ');
      const validRec = recommendationsList.filter(r => r.nextVaccineName.trim()).map(r => `${r.nextVaccineName} (Due: ${r.dueDays} days)`).join('; ');

      const immMeds = [{
        medicine: validImm ? `Vaccines: ${validImm}` : 'Immunization Record',
        dosage: immunizationsList[0]?.manufacturer ? `Mfr: ${immunizationsList[0].manufacturer} | Lot: ${immunizationsList[0].lotNumber}` : 'Manufacturer & Lot',
        route: immunizationsList[0]?.route ? `Route: ${immunizationsList[0].route} | Site: ${immunizationsList[0].site}` : 'Route & Site',
        method: validRec ? `Recs: ${validRec}` : 'Recommendations'
      }];
      formattedDisplayPayload = `${displayTitle}||MEDS:${JSON.stringify(immMeds)}`;
    } else if (activeHiType === 'WellnessRecord') {
      const vitalsSummary = [
        wellSysBp && wellDiaBp ? `BP: ${wellSysBp}/${wellDiaBp}` : '',
        wellHeartRate ? `HR: ${wellHeartRate}` : '',
        wellSpo2 ? `SpO2: ${wellSpo2}%` : '',
        wellRespRate ? `RR: ${wellRespRate}` : '',
        wellTemp ? `Temp: ${wellTemp}°F` : ''
      ].filter(Boolean).join(', ');

      const wellMeds = [{
        medicine: `Visit Date: ${formatToDisplayDate(visitDate)}`,
        dosage: vitalsSummary ? `Vitals: ${vitalsSummary}` : 'Vital Signs',
        route: wellHeight || wellWeight ? `Height: ${wellHeight}cm | Weight: ${wellWeight}kg | BMI: ${wellBmi}` : 'Body Measurements',
        method: `Diet: ${wellDietType} | Tobacco: ${wellTobaccoUse} | Alcohol: ${wellAlcoholConsumption}`
      }];
      formattedDisplayPayload = `${displayTitle}||MEDS:${JSON.stringify(wellMeds)}`;
    } else if (activeHiType === 'InvoiceRecord' || activeHiType === 'Invoice') {
      const validItems = invoiceItemsList.filter(i => i.medicineName.trim()).map(i => `${i.medicineName} (Qty: ${i.quantity}, Rate: ₹${i.rate})`).join('; ');

      const invMeds = [{
        medicine: invoiceNumber ? `Invoice #${invoiceNumber}` : 'Pharmacy Invoice Record',
        dosage: validItems ? `Items: ${validItems}` : 'Invoice Items',
        route: invoiceTotalNet || invoiceTotalGross ? `Net: ₹${invoiceTotalNet} | Gross: ₹${invoiceTotalGross}` : 'Invoice Total',
        method: `Date: ${formatToDisplayDate(invoiceDate)}`
      }];
      formattedDisplayPayload = `${displayTitle}||MEDS:${JSON.stringify(invMeds)}`;
    } else {
      // Filter valid entered medicines
      const validMedicines = medicines.filter((m) => m.drugName && m.drugName.trim().length > 0);
      const formattedMeds = validMedicines.map((m) => ({
        drugName: m.drugName.trim(),
        dosagePattern: m.dosagePattern || 'Morning Only (1-0-0)',
        route: m.route || 'Oral',
        method: m.method || 'After Food',
        reason: m.reason ? m.reason.trim() : '',
      }));

      formattedDisplayPayload = formattedMeds.length > 0
        ? `${displayTitle}||MEDS:${JSON.stringify(formattedMeds)}`
        : displayTitle;
    }

    return { displayTitle, refNum, formattedDisplayPayload };
  };

  // Submit Link Care Context (Save & Link to ABHA)
  const handleLinkCareContext = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedPatient) return;

    const { refNum, formattedDisplayPayload } = buildDisplayPayload();

    try {
      setIsLinking(true);
      setStatusMessage(null);

      const careContextItem: CareContextItem = {
        referenceNumber: refNum,
        display: formattedDisplayPayload,
        hiType: activeHiType,
      };

      const res = await linkCareContextsApi({
        patientId: selectedPatient.patientId,
        abhaNumber: selectedPatient.abhaNumber || '',
        abhaAddress: selectedAbhaAddress,
        careContexts: [careContextItem],
      });


      const newRecordItem: PatientCareContextRecord = {
        careContextId: Date.now(),
        patientId: selectedPatient.patientId,
        abhaNumber: selectedPatient.abhaNumber || '',
        abhaAddress: selectedAbhaAddress,
        referenceNumber: refNum,
        display: formattedDisplayPayload,
        hiType: activeHiType,
        status: 'LINKED',
        requestId: res.requestId || `REQ-${Date.now()}`,
        createdAt: new Date().toISOString(),
      };

      setLinkedRecords((prev) => [newRecordItem, ...prev.filter(r => r.referenceNumber !== refNum)]);

      setStatusMessage({
        type: 'success',
        message: `${activeHiType} Care Context successfully linked to ABHA! Request ID: ${res.requestId || 'REQ-SUCCESS'}`,
      });

      setIsAddModalOpen(false);
    } catch (err: unknown) {
      console.warn('API error fallback to local update', err);
      const newRecordItem: PatientCareContextRecord = {
        careContextId: Date.now(),
        patientId: selectedPatient.patientId,
        abhaNumber: selectedPatient.abhaNumber || '',
        abhaAddress: selectedAbhaAddress,
        referenceNumber: refNum,
        display: formattedDisplayPayload,
        hiType: activeHiType,
        status: 'LINKED',
        requestId: `REQ-${Date.now()}`,
        createdAt: new Date().toISOString(),
      };

      setLinkedRecords((prev) => [newRecordItem, ...prev.filter(r => r.referenceNumber !== refNum)]);

      setStatusMessage({
        type: 'success',
        message: `${activeHiType} Care Context successfully linked to ABHA!`,
      });

      setIsAddModalOpen(false);
    } finally {
      setIsLinking(false);
    }
  };

  // Save Care Context locally without ABHA link
  const handleSaveOnly = async () => {
    if (!selectedPatient) return;
    const { refNum, formattedDisplayPayload } = buildDisplayPayload();

    try {
      setIsSaving(true);
      setStatusMessage(null);

      const careContextItem: CareContextItem = {
        referenceNumber: refNum,
        display: formattedDisplayPayload,
        hiType: activeHiType,
      };

      const res = await saveCareContextsApi({
        patientId: selectedPatient.patientId,
        abhaNumber: selectedPatient.abhaNumber || '',
        abhaAddress: selectedAbhaAddress,
        careContexts: [careContextItem],
      });

      const savedRecord = res.linkedRecords && res.linkedRecords.length > 0
        ? res.linkedRecords[0]
        : {
          careContextId: Date.now(),
          patientId: selectedPatient.patientId,
          abhaNumber: selectedPatient.abhaNumber || '',
          abhaAddress: selectedAbhaAddress,
          referenceNumber: refNum,
          display: formattedDisplayPayload,
          hiType: activeHiType,
          status: 'SAVED',
          requestId: '',
          createdAt: new Date().toISOString(),
        };

      setLinkedRecords((prev) => [savedRecord, ...prev.filter(r => r.referenceNumber !== refNum)]);

      setStatusMessage({
        type: 'success',
        message: `${activeHiType} saved successfully (Not yet linked to ABHA).`,
      });

      setIsAddModalOpen(false);
    } catch (err: unknown) {
      console.warn('API error fallback to local update for save', err);
      const newRecordItem: PatientCareContextRecord = {
        careContextId: Date.now(),
        patientId: selectedPatient.patientId,
        abhaNumber: selectedPatient.abhaNumber || '',
        abhaAddress: selectedAbhaAddress,
        referenceNumber: refNum,
        display: formattedDisplayPayload,
        hiType: activeHiType,
        status: 'SAVED',
        requestId: '',
        createdAt: new Date().toISOString(),
      };

      setLinkedRecords((prev) => [newRecordItem, ...prev.filter(r => r.referenceNumber !== refNum)]);
      setStatusMessage({
        type: 'success',
        message: `${activeHiType} saved locally!`,
      });
      setIsAddModalOpen(false);
    } finally {
      setIsSaving(false);
    }
  };

  // Link an existing saved record to ABHA
  const handleLinkSavedRecord = async (record: PatientCareContextRecord) => {
    if (!selectedPatient) return;
    try {
      setIsLinking(true);
      setStatusMessage(null);

      const careContextItem: CareContextItem = {
        referenceNumber: record.referenceNumber,
        display: record.display,
        hiType: record.hiType,
      };

      const res = await linkCareContextsApi({
        patientId: selectedPatient.patientId,
        abhaNumber: selectedPatient.abhaNumber || record.abhaNumber || '',
        abhaAddress: selectedAbhaAddress || record.abhaAddress,
        careContexts: [careContextItem],
      });


      setLinkedRecords((prev) =>
        prev.map((r) =>
          r.referenceNumber === record.referenceNumber
            ? { ...r, status: 'LINKED', requestId: res.requestId || r.requestId }
            : r
        )
      );

      setStatusMessage({
        type: 'success',
        message: `${record.hiType} successfully linked to ABHA! Request ID: ${res.requestId || 'REQ-SUCCESS'}`,
      });
    } catch (err: unknown) {
      console.warn('Failed to link saved record', err);
      setLinkedRecords((prev) =>
        prev.map((r) =>
          r.referenceNumber === record.referenceNumber ? { ...r, status: 'LINKED' } : r
        )
      );
      setStatusMessage({
        type: 'success',
        message: `${record.hiType} successfully linked to ABHA!`,
      });
    } finally {
      setIsLinking(false);
    }
  };

  // Send Deep Link SMS
  const handleSendSms = async () => {
    if (!selectedPatient) return;
    const mobile = selectedPatient.mobile || '';
    if (!mobile) {
      setStatusMessage({ type: 'error', message: 'No mobile number found for this patient.' });
      return;
    }

    try {
      setIsSendingSms(true);
      const sent = await sendDeepLinkSmsApi(mobile, selectedAbhaAddress, selectedPatient.name);
      if (sent) {
        setStatusMessage({ type: 'success', message: `Deep linking SMS sent to ${mobile}!` });
      } else {
        setStatusMessage({ type: 'error', message: 'Failed to send SMS.' });
      }
    } catch (err) {
      console.error(err);
      setStatusMessage({ type: 'error', message: 'SMS request failed.' });
    } finally {
      setIsSendingSms(false);
    }
  };



  const categoryRecords = linkedRecords.filter(
    (r) => r.hiType.toLowerCase() === activeHiType.toLowerCase()
  );

  // Parse medicines list from record display payload
  const parseMedicinesFromDisplay = (rawDisplay: string): { title: string; meds: MedicineItem[] } => {
    if (!rawDisplay) return { title: 'Prescription', meds: [] };
    if (rawDisplay.includes('||MEDS:')) {
      const parts = rawDisplay.split('||MEDS:');
      try {
        const parsedMeds = JSON.parse(parts[1]);
        return { title: parts[0] || 'Prescription', meds: parsedMeds };
      } catch {
        return { title: parts[0] || 'Prescription', meds: [] };
      }
    }
    return { title: rawDisplay, meds: [] };
  };

  const getHiTypeIcon = (hiType: string) => {
    switch (hiType) {
      case 'DiagnosticReport': return '🔬';
      case 'DischargeSummary': return '🏥';
      case 'OPConsultation': return '🩺';
      case 'ImmunizationRecord': return '💉';
      case 'WellnessRecord': return '🌿';
      case 'InvoiceRecord':
      case 'Invoice': return '🧾';
      case 'HealthDocumentRecord': return '📄';
      default: return '💊';
    }
  };

  const getHiTypeContentLabel = (hiType: string) => {
    switch (hiType) {
      case 'DiagnosticReport': return 'DIAGNOSTIC TEST RESULTS';
      case 'DischargeSummary': return 'DISCHARGE SUMMARY DETAILS';
      case 'OPConsultation': return 'OP CONSULTATION RECORD';
      case 'ImmunizationRecord': return 'VACCINATION & IMMUNIZATION';
      case 'WellnessRecord': return 'WELLNESS & VITALS SUMMARY';
      case 'InvoiceRecord':
      case 'Invoice': return 'INVOICE & BILLING DETAILS';
      default: return 'PRESCRIBED MEDICINES & DOSAGE';
    }
  };

  return (
    <div className="health-records-wrapper exact-screenshot-layout">
      {/* Top Navigation Row */}
      <div className="hr-header-row">
        <div className="hr-header-left">
          <button type="button" className="hr-back-btn" onClick={() => navigate('/dashboard')} title="Back to Dashboard">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M15 18l-6-6 6-6" />
            </svg>
          </button>
          <div className="hr-title-block">
            <h1 className="hr-main-title">Health Records</h1>
            <p className="hr-sub-title">Manage patient prescriptions and medical records</p>
          </div>
        </div>

        <div className="hr-header-right">
          <div className={`patient-status-chip ${selectedPatient ? 'selected' : 'none'}`}>
            {selectedPatient ? (
              <>
                <span className="status-dot green" />
                <span>Selected: {selectedPatient.name}</span>
              </>
            ) : (
              <>
                <span className="status-dot gray" />
                <span>No patient selected</span>
              </>
            )}
          </div>
        </div>
      </div>

      {/* Main Two-Column Layout */}
      <div className="hr-main-grid">
        {/* Left Column: PATIENT PANEL */}
        <div className="patient-panel-card exact-left-card">
          <div className="panel-header-block">
            <span className="panel-tag uppercase-gray-tag">PATIENT PANEL</span>
            <h2 className="panel-title bold-title">Find, verify & link</h2>
            <p className="panel-subtitle">Unified patient context for ABHA workflows</p>
          </div>

          {/* Search Patient Box with Dropdown */}
          <div className="panel-search-box relative-box">
            <label className="input-label-small" htmlFor="search-patient-input">Search Patient</label>
            <div className="search-input-inner">
              <svg className="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" strokeWidth="2">
                <circle cx="11" cy="11" r="8" />
                <line x1="21" y1="21" x2="16.65" y2="16.65" />
              </svg>
              <input
                id="search-patient-input"
                type="text"
                className="panel-search-input rounded-input"
                placeholder="Search Patient (Use ↑ ↓ & Enter)"
                value={searchQuery}
                onFocus={() => {
                  setShowSearchDropdown(true);
                  setHighlightedPatientIndex(0);
                }}
                onChange={(e) => {
                  setSearchQuery(e.target.value);
                  setShowSearchDropdown(true);
                  setHighlightedPatientIndex(0);
                }}
                onKeyDown={handlePatientSearchKeyDown}
              />
              {searchQuery && (
                <button type="button" className="clear-icon-btn" onClick={() => { setSearchQuery(''); setHighlightedPatientIndex(-1); }}>✕</button>
              )}
            </div>

            {/* Dropdown Popup List when searching */}
            {showSearchDropdown && filteredPatients.length > 0 && (
              <div className="search-dropdown-popup">
                <div className="popup-label">Search Results (Use ↑ ↓ & Enter)</div>
                {filteredPatients.map((p, idx) => {
                  const isHighlighted = idx === highlightedPatientIndex;
                  const displayAbhaAddr = p.abhaAddresses && p.abhaAddresses.length > 0 ? p.abhaAddresses[0] : `${p.name.toLowerCase().replace(/\s+/g, '')}@sbx`;
                  return (
                    <div
                      key={p.patientId}
                      className={`popup-item ${isHighlighted ? 'active-highlight' : ''}`}
                      style={isHighlighted ? { background: '#e0f2fe', borderLeft: '3px solid #0284c7' } : {}}
                      onMouseEnter={() => setHighlightedPatientIndex(idx)}
                      onClick={() => handleSelectPatient(p)}
                    >
                      <div className="popup-avatar" style={isHighlighted ? { background: '#0284c7', color: '#ffffff' } : {}}>
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
                          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
                        </svg>
                      </div>
                      <div className="popup-info">
                        <span className="popup-name" style={isHighlighted ? { color: '#0369a1', fontWeight: 800 } : {}}>{p.name}</span>
                        <span className="popup-sub">{calculateAgeShort(p.dob)}, {displayAbhaAddr}</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>

          {/* Selected Patient Dark Gradient Card */}
          {selectedPatient ? (
            <div className="selected-patient-dark-card">
              <div className="dark-card-top">
                <div className="dark-card-avatar">
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="#a78bfa">
                    <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
                  </svg>
                </div>
                <div className="dark-card-name-block">
                  <span className="dark-card-label">Selected Patient</span>
                  <h3 className="dark-card-name">{selectedPatient.name}</h3>
                  <span className="dark-card-id">{selectedPatient.uhid || `ID: ${selectedPatient.patientId}`}</span>
                </div>
              </div>

              <div className="dark-card-bottom-grid">
                <div className="dark-sub-box">
                  <span className="dark-sub-label">Phone</span>
                  <span className="dark-sub-val">{selectedPatient.mobile || 'N/A'}</span>
                </div>
                <div className="dark-sub-box">
                  <span className="dark-sub-label">Age</span>
                  <span className="dark-sub-val">{calculateAgeShort(selectedPatient.dob)}</span>
                </div>
              </div>
            </div>
          ) : (
            <div className="no-selected-dark-placeholder">
              <span>Select a patient to view details</span>
            </div>
          )}

          {/* Profile found & token available Banner */}
          <div className="token-status-banner">
            <div className="token-icon-circle">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#ffffff" strokeWidth="3">
                <polyline points="20 6 9 17 4 12" />
              </svg>
            </div>
            <div className="token-text-block">
              <h4 className="token-title">Profile found & token available</h4>
              <p className="token-subtitle">You can proceed with linking health records</p>
            </div>
            <button
              type="button"
              className="token-refresh-btn"
              onClick={() => {
                if (selectedPatient) loadCareContextHistory(selectedPatient.patientId, selectedAbhaAddress);
              }}
            >
              Refresh
            </button>
          </div>
        </div>

        {/* Right Column: PRECRIPTIONS / Health Records View */}
        <div className="hr-content-area exact-right-card">
          <div className="records-category-card">
            {/* Category Title Header */}
            <div className="category-header-block">
              <span className="category-tag-uppercase">{activeHiType.toUpperCase()}S</span>
              <h2 className="category-patient-name">{selectedPatient ? selectedPatient.name : 'Select a Patient'}</h2>
            </div>

            {/* Filter Pills Row */}
            <div className="filter-pills-scroll-row">
              {HI_TYPES.map((type) => {
                const isActive = activeHiType === type;
                return (
                  <button
                    key={type}
                    type="button"
                    className={`hi-type-pill ${isActive ? 'active' : ''}`}
                    onClick={() => setActiveHiType(type)}
                  >
                    {type}
                  </button>
                );
              })}
            </div>

            {/* Status Notification if any */}
            {statusMessage && (
              <div className={`custom-status-banner ${statusMessage.type}`}>
                <span className="status-banner-icon">
                  {statusMessage.type === 'success' ? '✓' : '✕'}
                </span>
                <span className="status-banner-text">{statusMessage.message}</span>
                <button type="button" className="status-banner-close" onClick={() => setStatusMessage(null)}>✕</button>
              </div>
            )}

            {/* Action Button: + Add [activeHiType] */}
            <div className="action-button-row">
              <button
                type="button"
                className="add-prescription-purple-btn"
                onClick={handleOpenAddModal}
                disabled={!selectedPatient}
              >
                + Add {activeHiType}
              </button>

              {selectedPatient && (
                <button
                  type="button"
                  className="sms-notify-pill-btn"
                  onClick={handleSendSms}
                  disabled={isSendingSms}
                  title="Send Deep Linking SMS to Patient"
                >
                  {isSendingSms ? 'Sending SMS...' : '📱 Send Deep Link SMS'}
                </button>
              )}
            </div>

            {/* Records List or No patient / No records found */}
            <div className="records-content-container">
              {!selectedPatient ? (
                <div className="no-patient-selected-placeholder">
                  <div className="empty-state-icon-bg">👤</div>
                  <h3 className="empty-state-title">No Patient Selected</h3>
                  <p className="empty-state-sub">Please search and select a patient from the left panel to view or link health records.</p>
                </div>
              ) : loadingHistory ? (
                <div className="history-loading-box">
                  <span className="btn-spinner" />
                  <span>Loading records...</span>
                </div>
              ) : categoryRecords.length === 0 ? (
                <div className="no-records-text">No records found for {selectedPatient.name}</div>
              ) : (
                <div className="linked-cards-list-container">
                  {categoryRecords.map((rec) => {
                    const { title, meds } = parseMedicinesFromDisplay(rec.display);
                    const formattedDate = rec.createdAt
                      ? new Date(rec.createdAt).toISOString().split('T')[0]
                      : new Date().toISOString().split('T')[0];

                    return (
                      <div key={rec.careContextId} className={`redesigned-record-card status-${rec.status?.toLowerCase() || 'linked'}`}>
                        {/* Top Header Row */}
                        <div className="record-card-header">
                          <div className="card-header-left">
                            <div className={`record-type-avatar hi-type-${activeHiType.toLowerCase()}`}>
                              <span className="hi-icon">{getHiTypeIcon(activeHiType)}</span>
                            </div>
                            <div className="card-title-block">
                              <div className="title-ref-line">
                                <h3 className="card-main-title">{title}</h3>
                                <span className="card-ref-tag">Ref: #{rec.referenceNumber || 'REF-LOCAL'}</span>
                              </div>
                              <div className="card-sub-info flex-align-gap">
                                <span className="facility-name">
                                  <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M3 21h18M5 21V5a2 2 0 012-2h10a2 2 0 012 2v16M9 9h6M9 13h6M9 17h6" /></svg>
                                  MIDHA HOSPITAL
                                </span>
                                <span className="dot-sep">•</span>
                                <span className="record-date-str">
                                  <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><rect x="3" y="4" width="18" height="18" rx="2" ry="2" /><line x1="16" y1="2" x2="16" y2="6" /><line x1="8" y1="2" x2="8" y2="6" /><line x1="3" y1="10" x2="21" y2="10" /></svg>
                                  {formattedDate}
                                </span>
                              </div>
                            </div>
                          </div>

                          <div className="card-header-right flex-align-gap">
                            {rec.status?.toUpperCase() === 'SAVED' ? (
                              <>
                                <span className="status-badge-new saved">
                                  <span className="pulse-dot amber" />
                                  SAVED LOCALLY
                                </span>
                                <button
                                  type="button"
                                  className="card-link-abha-btn-new"
                                  onClick={() => handleLinkSavedRecord(rec)}
                                  disabled={isLinking}
                                  title="Link this saved prescription to patient ABHA"
                                >
                                  {isLinking ? (
                                    <>
                                      <span className="btn-spinner-sm" /> Linking...
                                    </>
                                  ) : (
                                    <>
                                      <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M10 13a5 5 0 007.54.54l3-3a5 5 0 00-7.07-7.07l-1.72 1.71" /><path d="M14 11a5 5 0 00-7.54-.54l-3 3a5 5 0 007.07 7.07l1.71-1.71" /></svg>
                                      Link to ABHA
                                    </>
                                  )}
                                </button>
                              </>
                            ) : (
                              <span className="status-badge-new linked">
                                <span className="pulse-dot green" />
                                LINKED TO ABHA
                              </span>
                            )}
                          </div>
                        </div>

                        {/* Middle Content Box (Items / Medicines Grid) */}
                        {meds && meds.length > 0 && (
                          <div className="record-details-section">
                            <div className="details-section-header">
                              <span className="section-label-text">{getHiTypeContentLabel(activeHiType)}</span>
                              <span className="items-count-pill">{meds.length} item{meds.length > 1 ? 's' : ''}</span>
                            </div>
                            <div className="meds-cards-grid">
                              {meds.map((m, idx) => (
                                <div key={idx} className="med-grid-card">
                                  <div className="med-card-top-row">
                                    <div className="med-name-icon-row">
                                      <span className="rx-symbol">Rx</span>
                                      <span className="drug-title-name">{m.drugName}</span>
                                    </div>
                                    {m.reason && <span className="med-reason-chip">{m.reason}</span>}
                                  </div>
                                  <div className="med-meta-pills-row">
                                    {m.dosagePattern && (
                                      <span className="pill-badge dosage">
                                        <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10" /><polyline points="12 6 12 12 16 14" /></svg>
                                        {m.dosagePattern}
                                      </span>
                                    )}
                                    {m.route && m.route !== '-' && (
                                      <span className="pill-badge route">
                                        <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><path d="M22 12h-4l-3 9L9 3l-3 9H2" /></svg>
                                        {m.route}
                                      </span>
                                    )}
                                    {m.method && (
                                      <span className="pill-badge timing">
                                        <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5"><circle cx="12" cy="12" r="10" /><path d="M12 8v4l3 3" /></svg>
                                        {m.method}
                                      </span>
                                    )}
                                  </div>
                                </div>
                              ))}
                            </div>
                          </div>
                        )}

                        {/* Footer Bar */}
                        <div className="record-card-footer">
                          <div className="footer-left-info">
                            <span className="target-abha-chip">
                              ABHA Target: <strong>{rec.abhaAddress || selectedAbhaAddress}</strong>
                            </span>
                            {rec.requestId && rec.requestId !== 'REQ-SUCCESS' && (
                              <span className="request-id-tag">Req ID: {rec.requestId.slice(0, 18)}...</span>
                            )}
                          </div>
                          {/* <span className="security-verified-tag">
                            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="#10b981" strokeWidth="2.5"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/><polyline points="9 12 11 14 15 10"/></svg>
                            ABDM M1 & M2 Compliant
                          </span> */}
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>

            {/* Gateway Console Log */}
            {/* {gatewayLogs.length > 0 && (
              <div className="gateway-log-console margin-top-console">
                <div className="console-header">
                  <span className="console-title">⚡ ABDM Gateway Logs</span>
                  <button type="button" className="clear-logs-btn" onClick={() => setGatewayLogs([])}>Clear</button>
                </div>
                <div className="console-body">
                  {gatewayLogs.map((log, i) => (
                    <div key={i} className="log-line">{log}</div>
                  ))}
                </div>
              </div>
            )} */}
          </div>
        </div>
      </div>

      {/* FULL PRESCRIPTION FORM MODAL */}
      {isAddModalOpen && (
        <div className="modal-backdrop">
          <div className="prescription-modal-card">
            {/* Modal Header Bar */}
            <div className="prescription-modal-header">
              <button
                type="button"
                className="modal-back-circle-btn"
                onClick={() => setIsAddModalOpen(false)}
                title="Close Modal"
              >
                ←
              </button>
              <div>
                <h2 className="modal-header-title">Add {activeHiType} Record</h2>
                <span style={{ fontSize: '12px', color: '#94a3b8' }}>
                  Link medical record to patient's ABHA: <strong style={{ color: '#a78bfa' }}>{selectedAbhaAddress}</strong>
                </span>
              </div>
            </div>

            {/* Modal Body Form */}
            <form onSubmit={handleLinkCareContext} className="prescription-modal-body">
              {/* Box 1: DISPLAY Input */}
              <div className="form-card-box">
                <label className="uppercase-field-label" htmlFor="modal-display-title">DISPLAY</label>
                <input
                  id="modal-display-title"
                  type="text"
                  className="rounded-form-input"
                  placeholder={activeHiType === 'DiagnosticReport' ? 'Diagnostic Report' : 'Prescription'}
                  value={contextDisplay}
                  onChange={(e) => setContextDisplay(e.target.value)}
                  required
                />
              </div>

              {activeHiType === 'DiagnosticReport' ? (
                /* DIAGNOSTIC REPORT DYNAMIC FORM MATCHING SCREENSHOT */
                <>
                  {/* Box 2: REPORT NAME & ENCOUNTER TYPE */}
                  <div className="form-card-box grid-two-col">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="report-name-input">REPORT NAME</label>
                      <input
                        id="report-name-input"
                        type="text"
                        className="rounded-form-input"
                        placeholder="Report Name"
                        value={reportName}
                        onChange={(e) => setReportName(e.target.value)}
                      />
                    </div>

                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="encounter-type-select-diag">ENCOUNTER TYPE</label>
                      <select
                        id="encounter-type-select-diag"
                        className="rounded-form-input select-arrow"
                        value={encounterType}
                        onChange={(e) => setEncounterType(e.target.value)}
                      >
                        <option value="Outpatient">Outpatient</option>
                        <option value="Inpatient">Inpatient</option>
                        <option value="Emergency">Emergency</option>
                        <option value="Ambulatory">Ambulatory</option>
                      </select>
                    </div>
                  </div>

                  {/* Box 3: EFFECTIVE DATETIME */}
                  <div className="form-card-box">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="effective-date-input">EFFECTIVE DATETIME</label>
                      <div className="date-input-wrapper">
                        <input
                          id="effective-date-input"
                          type="datetime-local"
                          className="rounded-form-input date-input-picker"
                          value={visitDate}
                          onChange={(e) => setVisitDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  {/* Box 4: RESULTS SECTION */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">RESULTS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={handleAddLabResultRow}
                      >
                        + Add
                      </button>
                    </div>

                    <div className="medicines-rows-container">
                      {labResults.map((res) => (
                        <div key={res.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">TEST NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Test Name"
                                value={res.testName}
                                onChange={(e) => handleUpdateLabResult(res.id, 'testName', e.target.value)}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">VALUE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Value"
                                value={res.value}
                                onChange={(e) => handleUpdateLabResult(res.id, 'value', e.target.value)}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">UNIT</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Unit"
                                value={res.unit}
                                onChange={(e) => handleUpdateLabResult(res.id, 'unit', e.target.value)}
                              />
                            </div>

                            {labResults.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => handleRemoveLabResult(res.id)}
                                title="Remove Result"
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* Box 5: REPORT SUMMARY / CLINICAL IMPRESSION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label" htmlFor="diag-summary-input">REPORT SUMMARY / CLINICAL IMPRESSION</label>
                    <textarea
                      id="diag-summary-input"
                      className="rounded-form-input"
                      rows={2}
                      placeholder="Enter lab report summary, conclusion, or doctor's impression..."
                      value={diagnosticSummary}
                      onChange={(e) => setDiagnosticSummary(e.target.value)}
                      style={{ width: '100%', resize: 'vertical' }}
                    />
                  </div>

                  {/* Box 6: PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH REPORT PDF / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </>
              ) : activeHiType === 'DischargeSummary' ? (
                /* DISCHARGE SUMMARY DYNAMIC FORM */
                <div className="ds-compact-form" style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                  {/* ENCOUNTER TYPE & ADMISSION DATE / DISCHARGE DATE */}
                  <div className="form-card-box grid-two-col">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="encounter-type-select-ds">ENCOUNTER TYPE</label>
                      <select
                        id="encounter-type-select-ds"
                        className="rounded-form-input select-arrow"
                        value={encounterType}
                        onChange={(e) => setEncounterType(e.target.value)}
                      >
                        <option value="Outpatient">Outpatient</option>
                        <option value="Inpatient">Inpatient</option>
                        <option value="Emergency">Emergency</option>
                        <option value="Ambulatory">Ambulatory</option>
                      </select>
                    </div>

                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="admission-date-input">ADMISSION DATE</label>
                      <div className="date-input-wrapper">
                        <input
                          id="admission-date-input"
                          type="datetime-local"
                          className="rounded-form-input date-input-picker"
                          value={admissionDate}
                          onChange={(e) => setAdmissionDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  <div className="form-card-box">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="discharge-date-input">DISCHARGE DATE</label>
                      <div className="date-input-wrapper">
                        <input
                          id="discharge-date-input"
                          type="datetime-local"
                          className="rounded-form-input date-input-picker"
                          value={dischargeDate}
                          onChange={(e) => setDischargeDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 1. CHIEF COMPLAINTS (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">CHIEF COMPLAINTS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setComplaintsList([...complaintsList, { id: String(Date.now()), complaint: '' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {complaintsList.map((c) => (
                        <div key={c.id} className="medicine-row-card" style={{ display: 'flex', gap: '10px', alignItems: 'flex-start', width: '100%' }}>
                          <div className="input-field-block" style={{ flex: 1, width: '100%', alignItems: 'stretch' }}>
                            <label className="uppercase-field-label">COMPLAINT</label>
                            <textarea
                              className="rounded-form-input"
                              rows={2}
                              placeholder="Enter complaint details..."
                              value={c.complaint}
                              onChange={(e) => {
                                const val = e.target.value;
                                setComplaintsList(prev => prev.map(item => item.id === c.id ? { ...item, complaint: val } : item));
                              }}
                              style={{ width: '100%', minWidth: '100%', boxSizing: 'border-box', resize: 'vertical' }}
                            />
                          </div>
                          {complaintsList.length > 1 && (
                            <button
                              type="button"
                              className="remove-med-btn"
                              onClick={() => setComplaintsList(prev => prev.filter(item => item.id !== c.id))}
                            >
                              ✕
                            </button>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 2. PHYSICAL EXAMINATIONS (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">PHYSICAL EXAMINATIONS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setPhysicalExamsList([...physicalExamsList, { id: String(Date.now()), name: '', value: '', unit: '' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {physicalExamsList.map((p) => (
                        <div key={p.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Name"
                                value={p.name}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setPhysicalExamsList(prev => prev.map(item => item.id === p.id ? { ...item, name: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">VALUE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Value"
                                value={p.value}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setPhysicalExamsList(prev => prev.map(item => item.id === p.id ? { ...item, value: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">UNIT</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Unit"
                                value={p.unit}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setPhysicalExamsList(prev => prev.map(item => item.id === p.id ? { ...item, unit: val } : item));
                                }}
                              />
                            </div>
                            {physicalExamsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setPhysicalExamsList(prev => prev.filter(item => item.id !== p.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 3. ALLERGIES (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">ALLERGIES</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setAllergiesList([...allergiesList, { id: String(Date.now()), allergyName: '', clinicalStatus: 'Active' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {allergiesList.map((a) => (
                        <div key={a.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">ALLERGY NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Allergy Name"
                                value={a.allergyName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setAllergiesList(prev => prev.map(item => item.id === a.id ? { ...item, allergyName: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">CLINICAL STATUS</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={a.clinicalStatus}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setAllergiesList(prev => prev.map(item => item.id === a.id ? { ...item, clinicalStatus: val } : item));
                                }}
                              >
                                <option value="Active">Active</option>
                                <option value="Inactive">Inactive</option>
                                <option value="Resolved">Resolved</option>
                              </select>
                            </div>
                            {allergiesList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setAllergiesList(prev => prev.filter(item => item.id !== a.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 4. MEDICAL HISTORY (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">MEDICAL HISTORY</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setMedicalHistoryList([...medicalHistoryList, { id: String(Date.now()), condition: '' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {medicalHistoryList.map((m) => (
                        <div key={m.id} className="medicine-row-card" style={{ display: 'flex', gap: '10px', alignItems: 'flex-start', width: '100%' }}>
                          <div className="input-field-block" style={{ flex: 1, width: '100%', alignItems: 'stretch' }}>
                            <label className="uppercase-field-label">CONDITION / HISTORY</label>
                            <textarea
                              className="rounded-form-input"
                              rows={2}
                              placeholder="e.g. Chest pain, Hypertension..."
                              value={m.condition}
                              onChange={(e) => {
                                const val = e.target.value;
                                setMedicalHistoryList(prev => prev.map(item => item.id === m.id ? { ...item, condition: val } : item));
                              }}
                              style={{ width: '100%', minWidth: '100%', boxSizing: 'border-box', resize: 'vertical' }}
                            />
                          </div>
                          {medicalHistoryList.length > 1 && (
                            <button
                              type="button"
                              className="remove-med-btn"
                              onClick={() => setMedicalHistoryList(prev => prev.filter(item => item.id !== m.id))}
                            >
                              ✕
                            </button>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 5. DIAGNOSTIC REPORTS (+ Add Observation) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">DIAGNOSTIC REPORTS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setDiagObsList([...diagObsList, { id: String(Date.now()), reportTitle: '', testName: '', value: '', unit: '' }])}
                      >
                        + Add Observation
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {diagObsList.map((d) => (
                        <div key={d.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">REPORT TITLE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Report Title (e.g. CBC)"
                                value={d.reportTitle}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDiagObsList(prev => prev.map(item => item.id === d.id ? { ...item, reportTitle: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">OBSERVATION / TEST NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Test Name"
                                value={d.testName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDiagObsList(prev => prev.map(item => item.id === d.id ? { ...item, testName: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">VALUE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Value"
                                value={d.value}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDiagObsList(prev => prev.map(item => item.id === d.id ? { ...item, value: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">UNIT</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Unit"
                                value={d.unit}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDiagObsList(prev => prev.map(item => item.id === d.id ? { ...item, unit: val } : item));
                                }}
                              />
                            </div>
                            {diagObsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setDiagObsList(prev => prev.filter(item => item.id !== d.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 6. PROCEDURES (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">PROCEDURES</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setProceduresList([...proceduresList, { id: String(Date.now()), procedureName: '', status: 'Completed' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {proceduresList.map((pr) => (
                        <div key={pr.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">PROCEDURE NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Procedure Name (e.g. Therapy)"
                                value={pr.procedureName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setProceduresList(prev => prev.map(item => item.id === pr.id ? { ...item, procedureName: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">STATUS</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={pr.status}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setProceduresList(prev => prev.map(item => item.id === pr.id ? { ...item, status: val } : item));
                                }}
                              >
                                <option value="Completed">Completed</option>
                                <option value="In Progress">In Progress</option>
                                <option value="Preparation">Preparation</option>
                              </select>
                            </div>
                            {proceduresList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setProceduresList(prev => prev.filter(item => item.id !== pr.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 7. MEDICATION SUMMARY (+ Add Medicine) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">MEDICATION SUMMARY</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setDischargeMedsList([...dischargeMedsList, { id: String(Date.now()), drugName: '', dosagePattern: '1-0-1', route: 'Oral', method: 'After Food', reason: '' }])}
                      >
                        + Add Medicine
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {dischargeMedsList.map((m) => (
                        <div key={m.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">MEDICINE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Medicine Name"
                                value={m.drugName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDischargeMedsList(prev => prev.map(item => item.id === m.id ? { ...item, drugName: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">DOSAGE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="e.g. 1-0-1"
                                value={m.dosagePattern}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDischargeMedsList(prev => prev.map(item => item.id === m.id ? { ...item, dosagePattern: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">ROUTE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Oral"
                                value={m.route}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDischargeMedsList(prev => prev.map(item => item.id === m.id ? { ...item, route: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">METHOD</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={m.method}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDischargeMedsList(prev => prev.map(item => item.id === m.id ? { ...item, method: val } : item));
                                }}
                              >
                                <option value="After Food">After Food</option>
                                <option value="Before Food">Before Food</option>
                                <option value="With Food">With Food</option>
                              </select>
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">REASON</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Reason"
                                value={m.reason}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setDischargeMedsList(prev => prev.map(item => item.id === m.id ? { ...item, reason: val } : item));
                                }}
                              />
                            </div>
                            {dischargeMedsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setDischargeMedsList(prev => prev.filter(item => item.id !== m.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 8. FAMILY HISTORY (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">FAMILY HISTORY</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setFamilyHistoryList([...familyHistoryList, { id: String(Date.now()), familyHistory: '' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {familyHistoryList.map((f) => (
                        <div key={f.id} className="medicine-row-card" style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
                          <div className="input-field-block" style={{ flex: 1 }}>
                            <label className="uppercase-field-label">FAMILY HISTORY</label>
                            <input
                              type="text"
                              className="rounded-form-input"
                              placeholder="Family History (e.g. Father Diabetes)"
                              value={f.familyHistory}
                              onChange={(e) => {
                                const val = e.target.value;
                                setFamilyHistoryList(prev => prev.map(item => item.id === f.id ? { ...item, familyHistory: val } : item));
                              }}
                            />
                          </div>
                          {familyHistoryList.length > 1 && (
                            <button
                              type="button"
                              className="remove-med-btn"
                              onClick={() => setFamilyHistoryList(prev => prev.filter(item => item.id !== f.id))}
                            >
                              ✕
                            </button>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 9. CARE PLANS (+ Add Care Plan) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">CARE PLANS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setCarePlansList([...carePlansList, { id: String(Date.now()), intent: 'plan', description: '' }])}
                      >
                        + Add Care Plan
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {carePlansList.map((cp) => (
                        <div key={cp.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">INTENT</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={cp.intent}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setCarePlansList(prev => prev.map(item => item.id === cp.id ? { ...item, intent: val } : item));
                                }}
                              >
                                <option value="plan">plan</option>
                                <option value="proposal">proposal</option>
                                <option value="order">order</option>
                              </select>
                            </div>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">DESCRIPTION</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Care plan description..."
                                value={cp.description}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setCarePlansList(prev => prev.map(item => item.id === cp.id ? { ...item, description: val } : item));
                                }}
                              />
                            </div>
                            {carePlansList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setCarePlansList(prev => prev.filter(item => item.id !== cp.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 10. APPOINTMENTS (+ Add Appointment) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">APPOINTMENTS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setAppointmentsList([...appointmentsList, { id: String(Date.now()), startDate: '', endDate: '', status: 'Completed', description: '' }])}
                      >
                        + Add Appointment
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {appointmentsList.map((apt) => (
                        <div key={apt.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">START DATE</label>
                              <input
                                type="datetime-local"
                                className="rounded-form-input date-input-picker"
                                value={apt.startDate}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setAppointmentsList(prev => prev.map(item => item.id === apt.id ? { ...item, startDate: val } : item));
                                }}
                                onClick={(e) => (e.target as any).showPicker?.()}
                                style={{ cursor: 'pointer' }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">END DATE</label>
                              <input
                                type="datetime-local"
                                className="rounded-form-input date-input-picker"
                                value={apt.endDate}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setAppointmentsList(prev => prev.map(item => item.id === apt.id ? { ...item, endDate: val } : item));
                                }}
                                onClick={(e) => (e.target as any).showPicker?.()}
                                style={{ cursor: 'pointer' }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">STATUS</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={apt.status}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setAppointmentsList(prev => prev.map(item => item.id === apt.id ? { ...item, status: val } : item));
                                }}
                              >
                                <option value="Completed">Completed</option>
                                <option value="Booked">Booked</option>
                                <option value="Arrived">Arrived</option>
                                <option value="Fulfilled">Fulfilled</option>
                              </select>
                            </div>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">DESCRIPTION</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Description"
                                value={apt.description}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setAppointmentsList(prev => prev.map(item => item.id === apt.id ? { ...item, description: val } : item));
                                }}
                              />
                            </div>
                            {appointmentsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setAppointmentsList(prev => prev.filter(item => item.id !== apt.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 11. PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH DISCHARGE SUMMARY PDF / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </div>
              ) : activeHiType === 'OPConsultation' ? (
                /* OP CONSULTATION DYNAMIC FORM */
                <div className="ds-compact-form" style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                  {/* ENCOUNTER TYPE & VISIT DATE */}
                  <div className="form-card-box grid-two-col">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="encounter-type-select-op">ENCOUNTER TYPE</label>
                      <select
                        id="encounter-type-select-op"
                        className="rounded-form-input select-arrow"
                        value={encounterType}
                        onChange={(e) => setEncounterType(e.target.value)}
                      >
                        <option value="Outpatient">Outpatient</option>
                        <option value="Inpatient">Inpatient</option>
                        <option value="Emergency">Emergency</option>
                        <option value="Ambulatory">Ambulatory</option>
                      </select>
                    </div>

                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="visit-date-input-op">VISIT DATE</label>
                      <div className="date-input-wrapper">
                        <input
                          id="visit-date-input-op"
                          type="datetime-local"
                          className="rounded-form-input date-input-picker"
                          value={visitDate}
                          onChange={(e) => setVisitDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 1. BODY MEASUREMENTS */}
                  <div className="form-card-box">
                    <span className="uppercase-field-label no-margin" style={{ marginBottom: '8px' }}>BODY MEASUREMENTS</span>
                    <div className="med-grid-top" style={{ display: 'flex', gap: '12px' }}>
                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">HEIGHT (CM)</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="e.g. 141"
                          value={opHeight}
                          onChange={(e) => {
                            const h = e.target.value;
                            setOpHeight(h);
                            if (h && opWeight) {
                              const hm = parseFloat(h) / 100;
                              const bmiVal = (parseFloat(opWeight) / (hm * hm)).toFixed(1);
                              setOpBmi(bmiVal);
                            }
                          }}
                        />
                      </div>

                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">WEIGHT (KG)</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="e.g. 66"
                          value={opWeight}
                          onChange={(e) => {
                            const w = e.target.value;
                            setOpWeight(w);
                            if (opHeight && w) {
                              const hm = parseFloat(opHeight) / 100;
                              const bmiVal = (parseFloat(w) / (hm * hm)).toFixed(1);
                              setOpBmi(bmiVal);
                            }
                          }}
                        />
                      </div>

                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">BMI (KG/M²)</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="BMI kg/m²"
                          value={opBmi}
                          onChange={(e) => setOpBmi(e.target.value)}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 2. VITALS (+ Add Vital) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">VITALS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setVitalsList([...vitalsList, { id: String(Date.now()), vitalName: '', value: '', unit: '' }])}
                      >
                        + Add Vital
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {vitalsList.map((v) => (
                        <div key={v.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">VITAL NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Vital Name (e.g. Blood Pressure, Heart Rate)"
                                value={v.vitalName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setVitalsList(prev => prev.map(item => item.id === v.id ? { ...item, vitalName: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">VALUE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Value"
                                value={v.value}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setVitalsList(prev => prev.map(item => item.id === v.id ? { ...item, value: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">UNIT</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Unit (e.g. mmHg, /MIN)"
                                value={v.unit}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setVitalsList(prev => prev.map(item => item.id === v.id ? { ...item, unit: val } : item));
                                }}
                              />
                            </div>
                            {vitalsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setVitalsList(prev => prev.filter(item => item.id !== v.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 3. CHIEF COMPLAINTS (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">CHIEF COMPLAINTS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setOpComplaintsList([...opComplaintsList, { id: String(Date.now()), complaint: '' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {opComplaintsList.map((c) => (
                        <div key={c.id} className="medicine-row-card" style={{ display: 'flex', gap: '10px', alignItems: 'flex-start', width: '100%' }}>
                          <div className="input-field-block" style={{ flex: 1, width: '100%', alignItems: 'stretch' }}>
                            <label className="uppercase-field-label">COMPLAINT</label>
                            <textarea
                              className="rounded-form-input"
                              rows={2}
                              placeholder="Enter complaint details..."
                              value={c.complaint}
                              onChange={(e) => {
                                const val = e.target.value;
                                setOpComplaintsList(prev => prev.map(item => item.id === c.id ? { ...item, complaint: val } : item));
                              }}
                              style={{ width: '100%', minWidth: '100%', boxSizing: 'border-box', resize: 'vertical' }}
                            />
                          </div>
                          {opComplaintsList.length > 1 && (
                            <button
                              type="button"
                              className="remove-med-btn"
                              onClick={() => setOpComplaintsList(prev => prev.filter(item => item.id !== c.id))}
                            >
                              ✕
                            </button>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 4. CLINICAL OBSERVATION / EXAMINATION RESULT */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label" htmlFor="op-obs-input">CLINICAL OBSERVATION / EXAMINATION RESULT</label>
                    <textarea
                      id="op-obs-input"
                      className="rounded-form-input"
                      rows={2}
                      placeholder="Enter clinical examination notes or test results..."
                      value={opObservationResult}
                      onChange={(e) => setOpObservationResult(e.target.value)}
                      style={{ width: '100%', resize: 'vertical' }}
                    />
                  </div>

                  {/* 5. ALLERGIES (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">ALLERGIES</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setOpAllergiesList([...opAllergiesList, { id: String(Date.now()), allergyName: '', type: 'medication', status: 'active' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {opAllergiesList.map((a) => (
                        <div key={a.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">ALLERGY / SUBSTANCE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Allergy / Substance Name"
                                value={a.allergyName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setOpAllergiesList(prev => prev.map(item => item.id === a.id ? { ...item, allergyName: val } : item));
                                }}
                              />
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">TYPE</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={a.type}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setOpAllergiesList(prev => prev.map(item => item.id === a.id ? { ...item, type: val } : item));
                                }}
                              >
                                <option value="medication">medication</option>
                                <option value="food">food</option>
                                <option value="environment">environment</option>
                                <option value="biologic">biologic</option>
                              </select>
                            </div>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">STATUS</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={a.status}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setOpAllergiesList(prev => prev.map(item => item.id === a.id ? { ...item, status: val } : item));
                                }}
                              >
                                <option value="active">active</option>
                                <option value="inactive">inactive</option>
                                <option value="resolved">resolved</option>
                              </select>
                            </div>
                            {opAllergiesList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setOpAllergiesList(prev => prev.filter(item => item.id !== a.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 6. MEDICAL HISTORY (+ Add) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">MEDICAL HISTORY</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setOpMedicalHistoryList([...opMedicalHistoryList, { id: String(Date.now()), condition: '' }])}
                      >
                        + Add
                      </button>
                    </div>
                    <div className="medicines-rows-container">
                      {opMedicalHistoryList.map((m) => (
                        <div key={m.id} className="medicine-row-card" style={{ display: 'flex', gap: '10px', alignItems: 'flex-start', width: '100%' }}>
                          <div className="input-field-block" style={{ flex: 1, width: '100%', alignItems: 'stretch' }}>
                            <label className="uppercase-field-label">CONDITION / HISTORY</label>
                            <textarea
                              className="rounded-form-input"
                              rows={2}
                              placeholder="Condition / History details..."
                              value={m.condition}
                              onChange={(e) => {
                                const val = e.target.value;
                                setOpMedicalHistoryList(prev => prev.map(item => item.id === m.id ? { ...item, condition: val } : item));
                              }}
                              style={{ width: '100%', minWidth: '100%', boxSizing: 'border-box', resize: 'vertical' }}
                            />
                          </div>
                          {opMedicalHistoryList.length > 1 && (
                            <button
                              type="button"
                              className="remove-med-btn"
                              onClick={() => setOpMedicalHistoryList(prev => prev.filter(item => item.id !== m.id))}
                            >
                              ✕
                            </button>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 7. PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH OP CONSULTATION PDF / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </div>
              ) : activeHiType === 'ImmunizationRecord' ? (
                /* IMMUNIZATION DYNAMIC FORM */
                <div className="ds-compact-form" style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                  {/* 1. IMMUNIZATIONS (+ Add Immunization) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">IMMUNIZATIONS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setImmunizationsList([...immunizationsList, {
                          id: String(Date.now()),
                          vaccineName: '',
                          manufacturer: '',
                          vaccineCode: '',
                          status: 'completed',
                          lotNumber: '',
                          doseNumber: '1',
                          occurrence: getIsoNowString(),
                          route: 'Intramuscular',
                          site: 'Left arm',
                          doseQty: '0.5',
                          unit: 'mL'
                        }])}
                      >
                        + Add Immunization
                      </button>
                    </div>

                    <div className="medicines-rows-container">
                      {immunizationsList.map((imm) => (
                        <div key={imm.id} className="medicine-row-card">
                          {/* Row 1: Vaccine Name, Manufacturer, Vaccine Code, Status */}
                          <div className="med-grid-top">
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">VACCINE NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Vaccine Name (e.g. Covid-19, Covaxin)"
                                value={imm.vaccineName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, vaccineName: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">MANUFACTURER</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Manufacturer (e.g. Bharat Biotech)"
                                value={imm.manufacturer}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, manufacturer: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">VACCINE CODE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Vaccine Code"
                                value={imm.vaccineCode}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, vaccineCode: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">STATUS</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={imm.status}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, status: val } : item));
                                }}
                              >
                                <option value="completed">completed</option>
                                <option value="not-done">not-done</option>
                              </select>
                            </div>
                          </div>

                          {/* Row 2: Lot Number, Dose #, Occurrence, Route */}
                          <div className="med-grid-bottom">
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">LOT NUMBER</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Lot Number"
                                value={imm.lotNumber}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, lotNumber: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">DOSE #</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Dose #"
                                value={imm.doseNumber}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, doseNumber: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">OCCURRENCE</label>
                              <input
                                type="datetime-local"
                                className="rounded-form-input date-input-picker"
                                value={imm.occurrence}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, occurrence: val } : item));
                                }}
                                onClick={(e) => (e.target as any).showPicker?.()}
                                style={{ cursor: 'pointer' }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">ROUTE</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={imm.route}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, route: val } : item));
                                }}
                              >
                                <option value="Intramuscular">Intramuscular</option>
                                <option value="Oral">Oral</option>
                                <option value="Subcutaneous">Subcutaneous</option>
                                <option value="Intradermal">Intradermal</option>
                              </select>
                            </div>
                          </div>

                          {/* Row 3: Site, Dose Qty, Unit */}
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">SITE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Site (e.g. Left arm, Thigh)"
                                value={imm.site}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, site: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">DOSE QTY</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Dose Qty (e.g. 0.5)"
                                value={imm.doseQty}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, doseQty: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">UNIT</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Unit (e.g. mL)"
                                value={imm.unit}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setImmunizationsList(prev => prev.map(item => item.id === imm.id ? { ...item, unit: val } : item));
                                }}
                              />
                            </div>

                            {immunizationsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setImmunizationsList(prev => prev.filter(item => item.id !== imm.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 2. RECOMMENDATIONS (+ Add Recommendation) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">RECOMMENDATIONS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setRecommendationsList([...recommendationsList, { id: String(Date.now()), nextVaccineName: '', nextVaccineCode: '', status: 'due', dueDays: '84' }])}
                      >
                        + Add Recommendation
                      </button>
                    </div>

                    <div className="medicines-rows-container">
                      {recommendationsList.map((rec) => (
                        <div key={rec.id} className="medicine-row-card">
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">NEXT VACCINE NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Next Vaccine Name"
                                value={rec.nextVaccineName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setRecommendationsList(prev => prev.map(item => item.id === rec.id ? { ...item, nextVaccineName: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">NEXT VACCINE CODE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Next Vaccine Code"
                                value={rec.nextVaccineCode}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setRecommendationsList(prev => prev.map(item => item.id === rec.id ? { ...item, nextVaccineCode: val } : item));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">STATUS</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={rec.status}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setRecommendationsList(prev => prev.map(item => item.id === rec.id ? { ...item, status: val } : item));
                                }}
                              >
                                <option value="due">due</option>
                                <option value="overdue">overdue</option>
                              </select>
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">DUE DAYS</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="Due Days (e.g. 84)"
                                value={rec.dueDays}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setRecommendationsList(prev => prev.map(item => item.id === rec.id ? { ...item, dueDays: val } : item));
                                }}
                              />
                            </div>

                            {recommendationsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setRecommendationsList(prev => prev.filter(item => item.id !== rec.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 3. PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH IMMUNIZATION RECORD PDF / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </div>
              ) : activeHiType === 'WellnessRecord' ? (
                /* WELLNESS RECORD DYNAMIC FORM */
                <div className="ds-compact-form" style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                  {/* ENCOUNTER TYPE & VISIT DATE */}
                  <div className="form-card-box grid-two-col">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="encounter-type-select-well">ENCOUNTER TYPE</label>
                      <select
                        id="encounter-type-select-well"
                        className="rounded-form-input select-arrow"
                        value={encounterType}
                        onChange={(e) => setEncounterType(e.target.value)}
                      >
                        <option value="Outpatient">Outpatient</option>
                        <option value="Inpatient">Inpatient</option>
                        <option value="Emergency">Emergency</option>
                        <option value="Ambulatory">Ambulatory</option>
                      </select>
                    </div>

                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="visit-date-input-well">VISIT DATE</label>
                      <div className="date-input-wrapper">
                        <input
                          id="visit-date-input-well"
                          type="datetime-local"
                          className="rounded-form-input date-input-picker"
                          value={visitDate}
                          onChange={(e) => setVisitDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 1. VITAL SIGNS */}
                  <div className="form-card-box">
                    <span className="uppercase-field-label no-margin" style={{ marginBottom: '8px' }}>VITAL SIGNS</span>
                    <div className="med-grid-top" style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '10px' }}>
                      <div className="input-field-block">
                        <label className="uppercase-field-label">RESPIRATORY RATE /MIN</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="e.g. 132"
                          value={wellRespRate}
                          onChange={(e) => setWellRespRate(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">HEART RATE /MIN</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="e.g. 44"
                          value={wellHeartRate}
                          onChange={(e) => setWellHeartRate(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">SPO2 %</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="SpO2 %"
                          value={wellSpo2}
                          onChange={(e) => setWellSpo2(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">BODY TEMPERATURE °F</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="Body temperature °F"
                          value={wellTemp}
                          onChange={(e) => setWellTemp(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">SYSTOLIC BP MMHG</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="Systolic BP mmHg"
                          value={wellSysBp}
                          onChange={(e) => setWellSysBp(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">DIASTOLIC BP MMHG</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="Diastolic BP mmHg"
                          value={wellDiaBp}
                          onChange={(e) => setWellDiaBp(e.target.value)}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 2. BODY MEASUREMENTS */}
                  <div className="form-card-box">
                    <span className="uppercase-field-label no-margin" style={{ marginBottom: '8px' }}>BODY MEASUREMENTS</span>
                    <div className="med-grid-top" style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '10px' }}>
                      <div className="input-field-block">
                        <label className="uppercase-field-label">HEIGHT CM</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="Height CM"
                          value={wellHeight}
                          onChange={(e) => {
                            const h = e.target.value;
                            setWellHeight(h);
                            if (h && wellWeight) {
                              const hm = parseFloat(h) / 100;
                              setWellBmi((parseFloat(wellWeight) / (hm * hm)).toFixed(1));
                            }
                          }}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">WEIGHT KG</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="Weight KG"
                          value={wellWeight}
                          onChange={(e) => {
                            const w = e.target.value;
                            setWellWeight(w);
                            if (wellHeight && w) {
                              const hm = parseFloat(wellHeight) / 100;
                              setWellBmi((parseFloat(w) / (hm * hm)).toFixed(1));
                            }
                          }}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">BMI KG/M²</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="BMI kg/m²"
                          value={wellBmi}
                          onChange={(e) => setWellBmi(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">WAIST CM</label>
                        <input
                          type="text"
                          className="rounded-form-input"
                          placeholder="Waist CM"
                          value={wellWaist}
                          onChange={(e) => setWellWaist(e.target.value)}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 3. WOMEN'S HEALTH */}
                  <div className="form-card-box">
                    <span className="uppercase-field-label no-margin" style={{ marginBottom: '8px' }}>WOMEN'S HEALTH</span>
                    <div className="med-grid-top" style={{ display: 'flex', gap: '12px' }}>
                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">AGE AT MENARCHE</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="Age at Menarche (e.g. 13)"
                          value={wellMenarcheAge}
                          onChange={(e) => setWellMenarcheAge(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">LAST MENSTRUAL DATE</label>
                        <input
                          type="date"
                          className="rounded-form-input date-input-picker"
                          value={wellLmpDate}
                          onChange={(e) => setWellLmpDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 4. LIFESTYLE */}
                  <div className="form-card-box">
                    <span className="uppercase-field-label no-margin" style={{ marginBottom: '8px' }}>LIFESTYLE</span>
                    <div className="med-grid-top" style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '10px' }}>
                      <div className="input-field-block">
                        <label className="uppercase-field-label">DIET TYPE</label>
                        <select
                          className="rounded-form-input select-arrow"
                          value={wellDietType}
                          onChange={(e) => setWellDietType(e.target.value)}
                        >
                          <option value="veg">veg</option>
                          <option value="non-veg">non-veg</option>
                          <option value="vegan">vegan</option>
                          <option value="eggetarian">eggetarian</option>
                        </select>
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">TOBACCO USE</label>
                        <select
                          className="rounded-form-input select-arrow"
                          value={wellTobaccoUse}
                          onChange={(e) => setWellTobaccoUse(e.target.value)}
                        >
                          <option value="no">no</option>
                          <option value="yes">yes</option>
                          <option value="former">former</option>
                        </select>
                      </div>

                      <div className="input-field-block">
                        <label className="uppercase-field-label">ALCOHOL CONSUMPTION</label>
                        <select
                          className="rounded-form-input select-arrow"
                          value={wellAlcoholConsumption}
                          onChange={(e) => setWellAlcoholConsumption(e.target.value)}
                        >
                          <option value="no">no</option>
                          <option value="moderate">moderate</option>
                          <option value="heavy">heavy</option>
                        </select>
                      </div>
                    </div>
                  </div>

                  {/* 5. OTHER OBSERVATIONS */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label" htmlFor="well-obs-input">OTHER OBSERVATIONS</label>
                    <textarea
                      id="well-obs-input"
                      className="rounded-form-input"
                      rows={2}
                      placeholder="Other Observations..."
                      value={wellOtherObs}
                      onChange={(e) => setWellOtherObs(e.target.value)}
                      style={{ width: '100%', resize: 'vertical' }}
                    />
                  </div>

                  {/* 6. PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH WELLNESS RECORD PDF / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </div>
              ) : activeHiType === 'InvoiceRecord' || activeHiType === 'Invoice' ? (
                /* INVOICE RECORD DYNAMIC FORM */
                <div className="ds-compact-form" style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                  {/* INVOICE NUMBER & INVOICE DATE */}
                  <div className="form-card-box grid-two-col">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="invoice-number-input">INVOICE NUMBER</label>
                      <input
                        id="invoice-number-input"
                        type="text"
                        className="rounded-form-input"
                        placeholder="Invoice Number (e.g. 34567IUYTRE)"
                        value={invoiceNumber}
                        onChange={(e) => setInvoiceNumber(e.target.value)}
                      />
                    </div>

                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="invoice-date-input">INVOICE DATE</label>
                      <div className="date-input-wrapper">
                        <input
                          id="invoice-date-input"
                          type="datetime-local"
                          className="rounded-form-input date-input-picker"
                          value={invoiceDate}
                          onChange={(e) => setInvoiceDate(e.target.value)}
                          onClick={(e) => (e.target as any).showPicker?.()}
                          style={{ cursor: 'pointer' }}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 1. PHARMACY INVOICE ITEMS (+ Add Medicine) */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">PHARMACY INVOICE ITEMS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={() => setInvoiceItemsList([...invoiceItemsList, {
                          id: String(Date.now()),
                          medicineName: '',
                          snomedCode: '',
                          form: 'tab',
                          hsn: '',
                          lotNumber: '',
                          expiryDate: '',
                          quantity: '1',
                          unit: 'Tab',
                          rate: '',
                          mrp: '',
                          discount: '0',
                          cgst: '0',
                          sgst: '0'
                        }])}
                      >
                        + Add Medicine
                      </button>
                    </div>

                    <div className="medicines-rows-container">
                      {invoiceItemsList.map((item) => (
                        <div key={item.id} className="medicine-row-card">
                          {/* Row 1: Medicine Name, SNOMED Code, Form, HSN */}
                          <div className="med-grid-top">
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">MEDICINE NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Medicine Name (e.g. doloo 500)"
                                value={item.medicineName}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, medicineName: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">SNOMED CODE</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="SNOMED Code"
                                value={item.snomedCode}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, snomedCode: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">FORM</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Form (e.g. tab, syrup)"
                                value={item.form}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, form: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">HSN</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="HSN (e.g. 3004)"
                                value={item.hsn}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, hsn: val } : i));
                                }}
                              />
                            </div>
                          </div>

                          {/* Row 2: Lot Number, Expiry, Quantity, Unit */}
                          <div className="med-grid-bottom">
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">LOT NUMBER</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Lot Number"
                                value={item.lotNumber}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, lotNumber: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">EXPIRY</label>
                              <input
                                type="date"
                                className="rounded-form-input date-input-picker"
                                value={item.expiryDate}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, expiryDate: val } : i));
                                }}
                                onClick={(e) => (e.target as any).showPicker?.()}
                                style={{ cursor: 'pointer' }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">QUANTITY</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="Qty"
                                value={item.quantity}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, quantity: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">UNIT</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Unit"
                                value={item.unit}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, unit: val } : i));
                                }}
                              />
                            </div>
                          </div>

                          {/* Row 3: Rate, MRP, Discount, CGST, SGST */}
                          <div className="med-grid-bottom" style={{ alignItems: 'center' }}>
                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">RATE</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="Rate"
                                value={item.rate}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, rate: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">MRP</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="MRP"
                                value={item.mrp}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, mrp: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">DISCOUNT</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="Discount"
                                value={item.discount}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, discount: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">CGST</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="CGST"
                                value={item.cgst}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, cgst: val } : i));
                                }}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">SGST</label>
                              <input
                                type="number"
                                className="rounded-form-input"
                                placeholder="SGST"
                                value={item.sgst}
                                onChange={(e) => {
                                  const val = e.target.value;
                                  setInvoiceItemsList(prev => prev.map(i => i.id === item.id ? { ...i, sgst: val } : i));
                                }}
                              />
                            </div>

                            {invoiceItemsList.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => setInvoiceItemsList(prev => prev.filter(i => i.id !== item.id))}
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* 2. INVOICE TOTAL */}
                  <div className="form-card-box">
                    <span className="uppercase-field-label no-margin" style={{ marginBottom: '8px' }}>INVOICE TOTAL</span>
                    <div className="med-grid-top" style={{ display: 'flex', gap: '12px' }}>
                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">TOTAL NET</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="Total Net (e.g. 700)"
                          value={invoiceTotalNet}
                          onChange={(e) => setInvoiceTotalNet(e.target.value)}
                        />
                      </div>

                      <div className="input-field-block flex-one">
                        <label className="uppercase-field-label">TOTAL GROSS</label>
                        <input
                          type="number"
                          className="rounded-form-input"
                          placeholder="Total Gross (e.g. 750)"
                          value={invoiceTotalGross}
                          onChange={(e) => setInvoiceTotalGross(e.target.value)}
                        />
                      </div>
                    </div>
                  </div>

                  {/* 3. PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH INVOICE PDF / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </div>
              ) : (
                /* PRESCRIPTION FORM */
                <>
                  {/* Box 2: ENCOUNTER TYPE & VISIT DATE Row */}
                  <div className="form-card-box grid-two-col">
                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="encounter-type-select">ENCOUNTER TYPE</label>
                      <select
                        id="encounter-type-select"
                        className="rounded-form-input select-arrow"
                        value={encounterType}
                        onChange={(e) => setEncounterType(e.target.value)}
                      >
                        <option value="Outpatient">Outpatient</option>
                        <option value="Inpatient">Inpatient</option>
                        <option value="Emergency">Emergency</option>
                        <option value="Ambulatory">Ambulatory</option>
                      </select>
                    </div>

                    <div className="input-field-block">
                      <label className="uppercase-field-label" htmlFor="visit-date-input">VISIT DATE</label>
                      <div className="date-input-wrapper">
                        <input
                          id="visit-date-input"
                          type="text"
                          className="rounded-form-input"
                          value={visitDate}
                          onChange={(e) => setVisitDate(e.target.value)}
                        />
                        <span className="calendar-icon">📅</span>
                      </div>
                    </div>
                  </div>

                  {/* Box 3: PRESCRIPTIONS / MEDICINES SECTION */}
                  <div className="form-card-box medicines-section-box">
                    <div className="section-header-row">
                      <span className="uppercase-field-label no-margin">PRESCRIPTIONS</span>
                      <button
                        type="button"
                        className="add-medicine-green-link"
                        onClick={handleAddMedicineRow}
                      >
                        + Add Medicine
                      </button>
                    </div>

                    {/* Medicine Rows */}
                    <div className="medicines-rows-container">
                      {medicines.map((med) => (
                        <div key={med.id} className="medicine-row-card">
                          {/* Row 1: Drug Name & Dosage */}
                          <div className="med-grid-top">
                            <div className="input-field-block flex-two">
                              <label className="uppercase-field-label">MEDICINE / DRUG NAME</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Medicine / Drug Name"
                                value={med.drugName}
                                onChange={(e) => handleUpdateMedicine(med.id, 'drugName', e.target.value)}
                              />
                            </div>

                            <div className="input-field-block flex-one">
                              <label className="uppercase-field-label">DOSAGE PATTERN</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={med.dosagePattern}
                                onChange={(e) => handleUpdateMedicine(med.id, 'dosagePattern', e.target.value)}
                              >
                                <option value="Morning Only (1-0-0)">Morning Only (1-0-0)</option>
                                <option value="Morning & Night (1-0-1)">Morning & Night (1-0-1)</option>
                                <option value="Thrice a Day (1-1-1)">Thrice a Day (1-1-1)</option>
                                <option value="Once a Day (0-0-1)">Once a Day (0-0-1)</option>
                                <option value="As Needed (PRN)">As Needed (PRN)</option>
                              </select>
                            </div>
                          </div>

                          {/* Row 2: Route, Method, Reason */}
                          <div className="med-grid-bottom">
                            <div className="input-field-block">
                              <label className="uppercase-field-label">ROUTE</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={med.route}
                                onChange={(e) => handleUpdateMedicine(med.id, 'route', e.target.value)}
                              >
                                <option value="Oral">Oral</option>
                                <option value="IV">IV</option>
                                <option value="IM">IM</option>
                                <option value="Topical">Topical</option>
                                <option value="Inhalation">Inhalation</option>
                                <option value="Sublingual">Sublingual</option>
                              </select>
                            </div>

                            <div className="input-field-block">
                              <label className="uppercase-field-label">METHOD</label>
                              <select
                                className="rounded-form-input select-arrow"
                                value={med.method}
                                onChange={(e) => handleUpdateMedicine(med.id, 'method', e.target.value)}
                              >
                                <option value="After Food">After Food</option>
                                <option value="Before Food">Before Food</option>
                                <option value="With Food">With Food</option>
                                <option value="Empty Stomach">Empty Stomach</option>
                              </select>
                            </div>

                            <div className="input-field-block flex-grow-reason">
                              <label className="uppercase-field-label">REASON</label>
                              <input
                                type="text"
                                className="rounded-form-input"
                                placeholder="Reason"
                                value={med.reason}
                                onChange={(e) => handleUpdateMedicine(med.id, 'reason', e.target.value)}
                              />
                            </div>

                            {medicines.length > 1 && (
                              <button
                                type="button"
                                className="remove-med-btn"
                                onClick={() => handleRemoveMedicine(med.id)}
                                title="Remove Medicine"
                              >
                                ✕
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* Box 4: PDF / DOCUMENT UPLOAD SECTION */}
                  <div className="form-card-box">
                    <label className="uppercase-field-label">ATTACH PRESCRIPTION / DOCUMENT (OPTIONAL)</label>
                    <input
                      type="file"
                      accept=".pdf,image/*"
                      onChange={handleFileChange}
                      className="rounded-form-input"
                      style={{ padding: '8px 12px' }}
                    />
                    {fileName && (
                      <div style={{ fontSize: '12.5px', color: '#059669', fontWeight: 600, marginTop: '6px', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <span>📄 File Attached:</span>
                        <strong>{fileName}</strong>
                        <span style={{ color: '#64748b', fontWeight: 400 }}>({Math.round(fileSize / 1024)} KB)</span>
                      </div>
                    )}
                  </div>
                </>
              )}

              {/* Modal Footer Actions Row */}
              <div className="prescription-modal-footer">
                <button
                  type="button"
                  className="footer-cancel-btn"
                  onClick={() => setIsAddModalOpen(false)}
                >
                  Cancel
                </button>

                <button
                  type="button"
                  className="footer-save-secondary-btn"
                  onClick={handleSaveOnly}
                  disabled={isSaving || isLinking}
                >
                  {isSaving ? 'Saving...' : `💾 Save ${activeHiType}`}
                </button>

                <button
                  type="submit"
                  className="footer-link-primary-btn"
                  disabled={isSaving || isLinking}
                >
                  {isLinking ? 'Linking to ABHA...' : `🔗 Save & Link ${activeHiType} to ABHA`}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default HealthRecordsPage;
