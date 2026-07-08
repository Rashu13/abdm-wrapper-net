using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace HMS.abdm
{
    public class frmHealthRecordViewer : Form
    {
        private DataGridView gridRecords;
        private RichTextBox txtDetails;
        private FlowLayoutPanel pnlAttachments;
        private List<Dictionary<string, object>> _records;

        public frmHealthRecordViewer(List<object> records)
        {
            _records = new List<Dictionary<string, object>>();
            if (records != null)
            {
                foreach (var rec in records)
                {
                    if (rec is Dictionary<string, object> dRec)
                        _records.Add(dRec);
                }
            }
            InitializeComponent();
            PopulateGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Health Record Viewer";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 246, 250);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 450, 
                BackColor = Color.FromArgb(220, 224, 230),
                SplitterWidth = 4
            };

            gridRecords = new DataGridView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 230, 230),
                EnableHeadersVisualStyles = false,
                Cursor = Cursors.Hand
            };
            
            gridRecords.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            gridRecords.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridRecords.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            gridRecords.ColumnHeadersHeight = 45;
            gridRecords.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 232, 240);
            gridRecords.DefaultCellStyle.SelectionForeColor = Color.Black;
            gridRecords.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            gridRecords.RowTemplate.Height = 40;
            gridRecords.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            gridRecords.Columns.Add("Date", "Visit Date");
            gridRecords.Columns.Add("Context", "Care Context");
            gridRecords.Columns.Add("Items", "Contents");
            gridRecords.Columns[0].FillWeight = 30;
            gridRecords.Columns[1].FillWeight = 40;
            gridRecords.Columns[2].FillWeight = 30;

            gridRecords.SelectionChanged += GridRecords_SelectionChanged;

            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.White };

            txtDetails = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            pnlAttachments = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Visible = false,
                Padding = new Padding(0, 0, 0, 10)
            };

            rightPanel.Controls.Add(pnlAttachments);
            rightPanel.Controls.Add(txtDetails);

            splitContainer.Panel1.Controls.Add(gridRecords);
            splitContainer.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(splitContainer);
        }

        private void PopulateGrid()
        {
            gridRecords.Rows.Clear();

            foreach (var rec in _records)
            {
                string ccRef = GetDictString(rec, "careContextReference");
                if (string.IsNullOrEmpty(ccRef)) ccRef = "Unknown CC";
                
                string visitDate = "Unknown Date";
                int resourceCount = 0;
                
                var bundleObj = GetDictValue(rec, "fhirBundle");
                if (bundleObj is Dictionary<string, object> bundle)
                {
                    var entryObj = GetDictValue(bundle, "entry");
                    if (entryObj is List<object> entries)
                    {
                        resourceCount = entries.Count;
                        foreach (var eObj in entries)
                        {
                            if (eObj is Dictionary<string, object> entry)
                            {
                                var rObj = GetDictValue(entry, "resource");
                                if (rObj is Dictionary<string, object> res)
                                {
                                    string rType = GetDictString(res, "resourceType");
                                    if (rType.Equals("Encounter", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var periodObj = GetDictValue(res, "period");
                                        if (periodObj is Dictionary<string, object> pDict)
                                        {
                                            string start = GetDictString(pDict, "start");
                                            DateTime dt;
                                            if (DateTime.TryParse(start, out dt))
                                            {
                                                visitDate = dt.ToString("dd-MMM-yyyy");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                int rowIndex = gridRecords.Rows.Add(visitDate, ccRef, $"{resourceCount} resources");
                gridRecords.Rows[rowIndex].Tag = rec;
            }
            
            // Auto select first row if available
            if (gridRecords.Rows.Count > 0)
            {
                gridRecords.Rows[0].Selected = true;
            }
        }

        private void GridRecords_SelectionChanged(object sender, EventArgs e)
        {
            if (gridRecords.SelectedRows.Count == 0) return;
            
            var selectedRow = gridRecords.SelectedRows[0];
            if (selectedRow.Tag is Dictionary<string, object> rec)
            {
                ShowSummary(rec);
            }
        }

        private void ShowSummary(Dictionary<string, object> rec)
        {
            txtDetails.Clear();
            pnlAttachments.Controls.Clear();
            pnlAttachments.Visible = false;

            string ccRef = GetDictString(rec, "careContextReference");
            if (string.IsNullOrEmpty(ccRef)) ccRef = "N/A";
            
            string patientName = "N/A";
            string abhaAddress = "N/A";
            string gender = "N/A";
            string dob = "N/A";
            string visitDate = "N/A";
            List<string> medicines = new List<string>();
            List<string> diagnostics = new List<string>();
            List<string> observations = new List<string>();
            List<string> immunizations = new List<string>();
            List<string> sections = new List<string>();
            List<Dictionary<string, object>> attachments = new List<Dictionary<string, object>>();

            var fhirBundleObj = GetDictValue(rec, "fhirBundle");
            if (fhirBundleObj is Dictionary<string, object> bundle)
            {
                var entryObj = GetDictValue(bundle, "entry");
                if (entryObj is List<object> entries)
                {
                    foreach (var eObj in entries)
                    {
                        if (eObj is Dictionary<string, object> entry)
                        {
                            var rObj = GetDictValue(entry, "resource");
                            if (rObj is Dictionary<string, object> r)
                            {
                                string rType = GetDictString(r, "resourceType");
                                
                                if (rType.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                                {
                                    var nameObj = GetDictValue(r, "name");
                                    if (nameObj is List<object> nList && nList.Count > 0 && nList[0] is Dictionary<string, object> nDict)
                                    {
                                        patientName = GetDictString(nDict, "text");
                                    }
                                    
                                    string g = GetDictString(r, "gender");
                                    if (!string.IsNullOrEmpty(g)) 
                                    {
                                        gender = g.Substring(0, 1).ToUpper(); 
                                    }
                                    
                                    dob = GetDictString(r, "birthDate");
                                    
                                    var idObj = GetDictValue(r, "identifier");
                                    if (idObj is List<object> idList)
                                    {
                                        foreach (var idItem in idList)
                                        {
                                            if (idItem is Dictionary<string, object> idDict)
                                            {
                                                string val = GetDictString(idDict, "value");
                                                if (val.Contains("@")) abhaAddress = val;
                                            }
                                        }
                                    }
                                }
                                else if (rType.Equals("Encounter", StringComparison.OrdinalIgnoreCase))
                                {
                                    var periodObj = GetDictValue(r, "period");
                                    if (periodObj is Dictionary<string, object> pDict)
                                    {
                                        string start = GetDictString(pDict, "start");
                                        DateTime dt;
                                        if (DateTime.TryParse(start, out dt))
                                            visitDate = dt.ToString("dd-MMM-yyyy HH:mm");
                                    }
                                }
                                else if (rType.Equals("MedicationRequest", StringComparison.OrdinalIgnoreCase))
                                {
                                    string medName = "";
                                    
                                    // Try medicationCodeableConcept
                                    var mccObj = GetDictValue(r, "medicationCodeableConcept");
                                    if (mccObj is Dictionary<string, object> mccDict)
                                    {
                                        medName = GetDictString(mccDict, "text");
                                    }
                                    
                                    // Try medicationReference
                                    if (string.IsNullOrEmpty(medName))
                                    {
                                        var mrObj = GetDictValue(r, "medicationReference");
                                        if (mrObj is Dictionary<string, object> mrDict)
                                        {
                                            medName = GetDictString(mrDict, "display");
                                        }
                                    }

                                    // Fallback to "medication" string or property
                                    if (string.IsNullOrEmpty(medName))
                                    {
                                        var medDirect = GetDictValue(r, "medication");
                                        if (medDirect is string sMed) medName = sMed;
                                    }

                                    if (string.IsNullOrEmpty(medName)) medName = "Unknown Medicine";

                                    string dosage = "Not Specified";
                                    var diObj = GetDictValue(r, "dosageInstruction");
                                    if (diObj is List<object> dList && dList.Count > 0 && dList[0] is Dictionary<string, object> dDict)
                                    {
                                        dosage = GetDictString(dDict, "text");
                                    }
                                    
                                    medicines.Add($"- {medName} - Dosage: {dosage}");
                                }
                                else if (rType.Equals("DiagnosticReport", StringComparison.OrdinalIgnoreCase))
                                {
                                    string repName = "Diagnostic Report";
                                    var codeObj = GetDictValue(r, "code");
                                    if (codeObj is Dictionary<string, object> codeDict)
                                    {
                                        repName = GetDictString(codeDict, "text");
                                    }

                                    string conclusion = GetDictString(r, "conclusion");
                                    string status = GetDictString(r, "status");
                                    string diagStr = $"- {repName} (Status: {status})";
                                    if (!string.IsNullOrEmpty(conclusion))
                                        diagStr += $" - Conclusion: {conclusion}";
                                    diagnostics.Add(diagStr);
                                }
                                else if (rType.Equals("Observation", StringComparison.OrdinalIgnoreCase))
                                {
                                    string obsName = "Observation";
                                    var codeObj = GetDictValue(r, "code");
                                    if (codeObj is Dictionary<string, object> codeDict)
                                    {
                                        obsName = GetDictString(codeDict, "text");
                                    }

                                    string obsVal = "";
                                    var vqObj = GetDictValue(r, "valueQuantity");
                                    if (vqObj is Dictionary<string, object> vQ)
                                    {
                                        string val = GetDictString(vQ, "value");
                                        string unit = GetDictString(vQ, "unit");
                                        obsVal = $"{val} {unit}".Trim();
                                    }
                                    else
                                    {
                                        obsVal = GetDictString(r, "valueString");
                                        if (string.IsNullOrEmpty(obsVal))
                                        {
                                            var vcObj = GetDictValue(r, "valueCodeableConcept");
                                            if (vcObj is Dictionary<string, object> vC)
                                            {
                                                obsVal = GetDictString(vC, "text");
                                            }
                                        }
                                    }

                                    string status = GetDictString(r, "status");
                                    string obsStr = $"- {obsName}: {obsVal}";
                                    if (!string.IsNullOrEmpty(status))
                                        obsStr += $" ({status})";
                                    observations.Add(obsStr);
                                }
                                else if (rType.Equals("Immunization", StringComparison.OrdinalIgnoreCase))
                                {
                                    string vaccine = "Unknown Vaccine";
                                    var vcObj = GetDictValue(r, "vaccineCode");
                                    if (vcObj is Dictionary<string, object> vCode)
                                    {
                                        vaccine = GetDictString(vCode, "text");
                                    }

                                    string occDate = GetDictString(r, "occurrenceDateTime");
                                    string dose = "";
                                    var paObj = GetDictValue(r, "protocolApplied");
                                    if (paObj is List<object> pList && pList.Count > 0 && pList[0] is Dictionary<string, object> pDict)
                                    {
                                        string doseNum = GetDictString(pDict, "doseNumberPositiveInt");
                                        if (!string.IsNullOrEmpty(doseNum))
                                            dose = $" (Dose: {doseNum})";
                                    }

                                    string status = GetDictString(r, "status");
                                    string immStr = $"- {vaccine}{dose}";
                                    if (!string.IsNullOrEmpty(occDate))
                                        immStr += $" given on {occDate}";
                                    if (!string.IsNullOrEmpty(status))
                                        immStr += $" [Status: {status}]";
                                    immunizations.Add(immStr);
                                }
                                else if (rType.Equals("Composition", StringComparison.OrdinalIgnoreCase))
                                {
                                    string compTitle = GetDictString(r, "title");
                                    string compStatus = GetDictString(r, "status");
                                    if (!string.IsNullOrEmpty(compTitle))
                                    {
                                        sections.Add($"Document Type: {compTitle} (Status: {compStatus})");
                                    }
                                    
                                    var secObj = GetDictValue(r, "section");
                                    if (secObj is List<object> sList)
                                    {
                                        foreach (var sObj in sList)
                                        {
                                            if (sObj is Dictionary<string, object> sDict)
                                            {
                                                string sTitle = GetDictString(sDict, "title");
                                                if (string.IsNullOrEmpty(sTitle)) sTitle = "Section";
                                                
                                                // If sTitle is "Medications" or "Prescription", we skip it in general sections 
                                                // because it's already displayed in a separate, dedicated medicines block.
                                                if (sTitle.Equals("Medications", StringComparison.OrdinalIgnoreCase) || 
                                                    sTitle.Equals("Prescription", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    continue;
                                                }

                                                string sText = "";
                                                var textObj = GetDictValue(sDict, "text");
                                                if (textObj is Dictionary<string, object> tDict)
                                                {
                                                    sText = GetDictString(tDict, "div");
                                                    sText = System.Text.RegularExpressions.Regex.Replace(sText, "<.*?>", string.Empty).Trim();
                                                }

                                                if (!string.IsNullOrEmpty(sText))
                                                {
                                                    sections.Add($"* {sTitle}: {sText}");
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (rType.Equals("DocumentReference", StringComparison.OrdinalIgnoreCase))
                                {
                                    attachments.Add(r);
                                }
                            }
                        }
                    }
                }
            }

            // Build Beautiful Output
            txtDetails.SelectionFont = new Font("Segoe UI", 15F, FontStyle.Bold);
            txtDetails.SelectionColor = Color.FromArgb(41, 128, 185); // Bright Blue for name
            txtDetails.AppendText($"Patient Name: {patientName}\n");
            
            txtDetails.SelectionFont = new Font("Segoe UI", 11.5F, FontStyle.Regular);
            txtDetails.SelectionColor = Color.FromArgb(100, 100, 100);
            txtDetails.AppendText($"ABHA Address: {abhaAddress}\n\n");

            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtDetails.SelectionColor = Color.FromArgb(52, 73, 94);
            txtDetails.AppendText($"Gender: ");
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
            txtDetails.SelectionColor = Color.Black;
            txtDetails.AppendText($"{gender} | ");
            
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtDetails.SelectionColor = Color.FromArgb(52, 73, 94);
            txtDetails.AppendText($"DOB: ");
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
            txtDetails.SelectionColor = Color.Black;
            txtDetails.AppendText($"{dob}\n\n");
            
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtDetails.SelectionColor = Color.FromArgb(52, 73, 94);
            txtDetails.AppendText($"Care Context Ref: ");
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
            txtDetails.SelectionColor = Color.Black;
            txtDetails.AppendText($"{ccRef}\n\n");
            
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtDetails.SelectionColor = Color.FromArgb(52, 73, 94);
            txtDetails.AppendText($"Date: ");
            txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
            txtDetails.SelectionColor = Color.Black;
            txtDetails.AppendText($"{visitDate}\n\n");

            if (medicines.Count > 0)
            {
                txtDetails.SelectionFont = new Font("Segoe UI", 14F, FontStyle.Bold);
                txtDetails.SelectionColor = Color.FromArgb(41, 128, 185);
                txtDetails.AppendText($"Prescribed Medicines:\n");
                
                txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
                txtDetails.SelectionColor = Color.Black;
                foreach(var m in medicines)
                {
                    txtDetails.AppendText($"{m}\n");
                }
                txtDetails.AppendText("\n");
            }

            if (diagnostics.Count > 0)
            {
                txtDetails.SelectionFont = new Font("Segoe UI", 14F, FontStyle.Bold);
                txtDetails.SelectionColor = Color.FromArgb(230, 126, 34); // Orange for Diagnostic
                txtDetails.AppendText($"Diagnostic Reports:\n");
                
                txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
                txtDetails.SelectionColor = Color.Black;
                foreach(var d in diagnostics)
                {
                    txtDetails.AppendText($"{d}\n");
                }
                txtDetails.AppendText("\n");
            }

            if (observations.Count > 0)
            {
                txtDetails.SelectionFont = new Font("Segoe UI", 14F, FontStyle.Bold);
                txtDetails.SelectionColor = Color.FromArgb(46, 204, 113); // Green for Observations/Vital Signs
                txtDetails.AppendText($"Observations & Vitals:\n");
                
                txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
                txtDetails.SelectionColor = Color.Black;
                foreach(var o in observations)
                {
                    txtDetails.AppendText($"{o}\n");
                }
                txtDetails.AppendText("\n");
            }

            if (immunizations.Count > 0)
            {
                txtDetails.SelectionFont = new Font("Segoe UI", 14F, FontStyle.Bold);
                txtDetails.SelectionColor = Color.FromArgb(155, 89, 182); // Purple for Immunization
                txtDetails.AppendText($"Immunization Records:\n");
                
                txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
                txtDetails.SelectionColor = Color.Black;
                foreach(var imm in immunizations)
                {
                    txtDetails.AppendText($"{imm}\n");
                }
                txtDetails.AppendText("\n");
            }

            if (sections.Count > 0)
            {
                txtDetails.SelectionFont = new Font("Segoe UI", 14F, FontStyle.Bold);
                txtDetails.SelectionColor = Color.FromArgb(52, 73, 94); // Dark slate for Document Notes
                txtDetails.AppendText($"Clinical & Consultation Notes:\n");
                
                txtDetails.SelectionFont = new Font("Segoe UI", 12F, FontStyle.Regular);
                txtDetails.SelectionColor = Color.Black;
                foreach(var sec in sections)
                {
                    txtDetails.AppendText($"{sec}\n");
                }
                txtDetails.AppendText("\n");
            }

            // Create buttons for attachments
            if (attachments.Count > 0)
            {
                int attIndex = 1;
                foreach(var attRes in attachments)
                {
                    try
                    {
                        var contentObj = GetDictValue(attRes, "content");
                        if (contentObj is List<object> cList && cList.Count > 0)
                        {
                            if (cList[0] is Dictionary<string, object> cDict)
                            {
                                var attObj = GetDictValue(cDict, "attachment");
                                if (attObj is Dictionary<string, object> att)
                                {
                                    string base64 = GetDictString(att, "data");
                                    if (!string.IsNullOrEmpty(base64))
                                    {
                                        string ext = ".pdf";
                                        string contentType = GetDictString(att, "contentType").ToLower();
                                        if (contentType.Contains("jpeg") || contentType.Contains("jpg")) ext = ".jpg";
                                        else if (contentType.Contains("png")) ext = ".png";

                                        var btnOpen = new Button
                                        {
                                            Text = $"View Document {attIndex} ({ext})",
                                            Width = 180,
                                            Height = 40,
                                            BackColor = Color.FromArgb(46, 204, 113),
                                            ForeColor = Color.White,
                                            FlatStyle = FlatStyle.Flat,
                                            Cursor = Cursors.Hand,
                                            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                                            Margin = new Padding(0, 0, 10, 10)
                                        };
                                        btnOpen.FlatAppearance.BorderSize = 0;
                                        
                                        Action openPdfAction = () => 
                                        {
                                            try
                                            {
                                                byte[] fileBytes = Convert.FromBase64String(base64);
                                                string tempFile = Path.Combine(Path.GetTempPath(), $"ABDM_Doc_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}");
                                                File.WriteAllBytes(tempFile, fileBytes);
                                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                                {
                                                    FileName = tempFile,
                                                    UseShellExecute = true
                                                });
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show($"Failed to open: {ex.Message}");
                                            }
                                        };
                                        
                                        btnOpen.Click += (s, args) => openPdfAction();
                                        pnlAttachments.Controls.Add(btnOpen);
                                        
                                        // Auto open the very first attachment when they click the row
                                        if (attIndex == 1) openPdfAction();
                                    }
                                }
                            }
                        }
                    }
                    catch { /* ignore */ }
                    attIndex++;
                }
                
                if (pnlAttachments.Controls.Count > 0)
                {
                    pnlAttachments.Visible = true;
                }
            }
        }

        private static object GetDictValue(Dictionary<string, object> dict, string key)
        {
            if (dict == null) return null;
            foreach (var k in dict.Keys)
            {
                if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return dict[k];
            }
            return null;
        }

        private static string GetDictString(Dictionary<string, object> dict, string key)
        {
            var val = GetDictValue(dict, key);
            return val?.ToString() ?? "";
        }
    }
}
