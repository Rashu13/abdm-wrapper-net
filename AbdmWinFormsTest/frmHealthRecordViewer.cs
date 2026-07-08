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
                string ccRef = rec.ContainsKey("careContextReference") ? rec["careContextReference"]?.ToString() ?? "Unknown CC" : "Unknown CC";
                string visitDate = "Unknown Date";
                int resourceCount = 0;
                
                if (rec.ContainsKey("fhirBundle") && rec["fhirBundle"] is Dictionary<string, object> bundle)
                {
                    if (bundle.ContainsKey("entry") && bundle["entry"] is List<object> entries)
                    {
                        resourceCount = entries.Count;
                        foreach (var eObj in entries)
                        {
                            if (eObj is Dictionary<string, object> entry && entry.ContainsKey("resource") && entry["resource"] is Dictionary<string, object> res)
                            {
                                string rType = res.ContainsKey("resourceType") ? res["resourceType"]?.ToString() ?? "" : "";
                                if (rType == "Encounter")
                                {
                                    if (res.ContainsKey("period") && res["period"] is Dictionary<string, object> pDict && pDict.ContainsKey("start"))
                                    {
                                        DateTime dt;
                                        if (DateTime.TryParse(pDict["start"].ToString(), out dt))
                                        {
                                            visitDate = dt.ToString("dd-MMM-yyyy");
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

            string ccRef = rec.ContainsKey("careContextReference") ? rec["careContextReference"]?.ToString() ?? "N/A" : "N/A";
            
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

            if (rec.ContainsKey("fhirBundle") && rec["fhirBundle"] is Dictionary<string, object> bundle)
            {
                if (bundle.ContainsKey("entry") && bundle["entry"] is List<object> entries)
                {
                    foreach (var eObj in entries)
                    {
                        if (eObj is Dictionary<string, object> entry && entry.ContainsKey("resource") && entry["resource"] is Dictionary<string, object> r)
                        {
                            string rType = r.ContainsKey("resourceType") ? r["resourceType"].ToString() : "";
                            
                            if (rType == "Patient")
                            {
                                if (r.ContainsKey("name") && r["name"] is List<object> nList && nList.Count > 0 && nList[0] is Dictionary<string, object> nDict && nDict.ContainsKey("text"))
                                    patientName = nDict["text"].ToString();
                                
                                if (r.ContainsKey("gender")) 
                                {
                                    string g = r["gender"].ToString();
                                    if (!string.IsNullOrEmpty(g)) gender = g.Substring(0, 1).ToUpper(); 
                                }
                                
                                if (r.ContainsKey("birthDate")) dob = r["birthDate"].ToString();
                                
                                if (r.ContainsKey("identifier") && r["identifier"] is List<object> idList)
                                {
                                    foreach (var idObj in idList)
                                    {
                                        if (idObj is Dictionary<string, object> idDict && idDict.ContainsKey("value"))
                                        {
                                            string val = idDict["value"].ToString();
                                            if (val.Contains("@")) abhaAddress = val;
                                        }
                                    }
                                }
                            }
                            else if (rType == "Encounter")
                            {
                                if (r.ContainsKey("period") && r["period"] is Dictionary<string, object> pDict && pDict.ContainsKey("start"))
                                {
                                    DateTime dt;
                                    if (DateTime.TryParse(pDict["start"].ToString(), out dt))
                                        visitDate = dt.ToString("dd-MMM-yyyy HH:mm");
                                }
                            }
                            else if (rType == "MedicationRequest")
                            {
                                string medName = "Unknown Medicine";
                                if (r.ContainsKey("medicationCodeableConcept") && r["medicationCodeableConcept"] is Dictionary<string, object> mDict && mDict.ContainsKey("text"))
                                    medName = mDict["text"].ToString();
                                else if (r.ContainsKey("medicationReference") && r["medicationReference"] is Dictionary<string, object> mrDict && mrDict.ContainsKey("display"))
                                    medName = mrDict["display"].ToString();

                                string dosage = "Not Specified";
                                if (r.ContainsKey("dosageInstruction") && r["dosageInstruction"] is List<object> dList && dList.Count > 0 && dList[0] is Dictionary<string, object> dDict && dDict.ContainsKey("text"))
                                {
                                    dosage = dDict["text"].ToString();
                                }
                                
                                medicines.Add($"- {medName} - Dosage: {dosage}");
                            }
                            else if (rType == "DiagnosticReport")
                            {
                                string repName = "Diagnostic Report";
                                if (r.ContainsKey("code") && r["code"] is Dictionary<string, object> cDict && cDict.ContainsKey("text"))
                                    repName = cDict["text"].ToString();

                                string conclusion = "";
                                if (r.ContainsKey("conclusion"))
                                    conclusion = r["conclusion"].ToString();

                                string status = r.ContainsKey("status") ? r["status"].ToString() : "";
                                string diagStr = $"- {repName} (Status: {status})";
                                if (!string.IsNullOrEmpty(conclusion))
                                    diagStr += $" - Conclusion: {conclusion}";
                                diagnostics.Add(diagStr);
                            }
                            else if (rType == "Observation")
                            {
                                string obsName = "Observation";
                                if (r.ContainsKey("code") && r["code"] is Dictionary<string, object> cDict && cDict.ContainsKey("text"))
                                    obsName = cDict["text"].ToString();

                                string obsVal = "";
                                if (r.ContainsKey("valueQuantity") && r["valueQuantity"] is Dictionary<string, object> vQ)
                                {
                                    string val = vQ.ContainsKey("value") ? vQ["value"].ToString() : "";
                                    string unit = vQ.ContainsKey("unit") ? vQ["unit"].ToString() : "";
                                    obsVal = $"{val} {unit}".Trim();
                                }
                                else if (r.ContainsKey("valueString"))
                                {
                                    obsVal = r["valueString"].ToString();
                                }
                                else if (r.ContainsKey("valueCodeableConcept") && r["valueCodeableConcept"] is Dictionary<string, object> vC && vC.ContainsKey("text"))
                                {
                                    obsVal = vC["text"].ToString();
                                }

                                string status = r.ContainsKey("status") ? r["status"].ToString() : "";
                                string obsStr = $"- {obsName}: {obsVal}";
                                if (!string.IsNullOrEmpty(status))
                                    obsStr += $" ({status})";
                                observations.Add(obsStr);
                            }
                            else if (rType == "Immunization")
                            {
                                string vaccine = "Unknown Vaccine";
                                if (r.ContainsKey("vaccineCode") && r["vaccineCode"] is Dictionary<string, object> vCode && vCode.ContainsKey("text"))
                                    vaccine = vCode["text"].ToString();

                                string occDate = "";
                                if (r.ContainsKey("occurrenceDateTime"))
                                    occDate = r["occurrenceDateTime"].ToString();

                                string dose = "";
                                if (r.ContainsKey("protocolApplied") && r["protocolApplied"] is List<object> pList && pList.Count > 0)
                                {
                                    if (pList[0] is Dictionary<string, object> pDict && pDict.ContainsKey("doseNumberPositiveInt"))
                                        dose = $" (Dose: {pDict["doseNumberPositiveInt"]})";
                                }

                                string status = r.ContainsKey("status") ? r["status"].ToString() : "";
                                string immStr = $"- {vaccine}{dose}";
                                if (!string.IsNullOrEmpty(occDate))
                                    immStr += $" given on {occDate}";
                                if (!string.IsNullOrEmpty(status))
                                    immStr += $" [Status: {status}]";
                                immunizations.Add(immStr);
                            }
                            else if (rType == "Composition")
                            {
                                if (r.ContainsKey("title"))
                                {
                                    string compTitle = r["title"].ToString();
                                    string compStatus = r.ContainsKey("status") ? r["status"].ToString() : "";
                                    sections.Add($"Document Type: {compTitle} (Status: {compStatus})");
                                }
                                if (r.ContainsKey("section") && r["section"] is List<object> sList)
                                {
                                    foreach (var sObj in sList)
                                    {
                                        if (sObj is Dictionary<string, object> sDict)
                                        {
                                            string sTitle = sDict.ContainsKey("title") ? sDict["title"].ToString() : "Section";
                                            string sText = "";
                                            if (sDict.ContainsKey("text") && sDict["text"] is Dictionary<string, object> tDict && tDict.ContainsKey("div"))
                                            {
                                                sText = tDict["div"].ToString();
                                                sText = System.Text.RegularExpressions.Regex.Replace(sText, "<.*?>", string.Empty).Trim();
                                            }
                                            sections.Add($"* {sTitle}: {sText}");
                                        }
                                    }
                                }
                            }
                            else if (rType == "DocumentReference")
                            {
                                attachments.Add(r);
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
                        if (attRes.ContainsKey("content") && attRes["content"] is List<object> cList && cList.Count > 0)
                        {
                            if (cList[0] is Dictionary<string, object> cDict && cDict.ContainsKey("attachment") && cDict["attachment"] is Dictionary<string, object> att)
                            {
                                if (att.ContainsKey("data") && att["data"] != null)
                                {
                                    string base64 = att["data"].ToString();
                                    string ext = ".pdf";
                                    if (att.ContainsKey("contentType") && att["contentType"] != null)
                                    {
                                        string cType = att["contentType"].ToString().ToLower();
                                        if (cType.Contains("jpeg") || cType.Contains("jpg")) ext = ".jpg";
                                        else if (cType.Contains("png")) ext = ".png";
                                    }

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
                    catch { /* ignore */ }
                    attIndex++;
                }
                
                if (pnlAttachments.Controls.Count > 0)
                {
                    pnlAttachments.Visible = true;
                }
            }
        }
    }
}
