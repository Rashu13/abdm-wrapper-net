using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace HMS.abdm
{
    public class frmHealthRecordViewer : Form
    {
        private TreeView treeView;
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
            PopulateTree();
        }

        private void InitializeComponent()
        {
            this.Text = "Health Record Viewer";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 300
            };

            treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };
            treeView.AfterSelect += TreeView_AfterSelect;

            var rightPanel = new Panel { Dock = DockStyle.Fill };

            txtDetails = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                ReadOnly = true,
                BackColor = Color.White
            };

            pnlAttachments = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.LightGray,
                Visible = false,
                Padding = new Padding(5)
            };

            rightPanel.Controls.Add(txtDetails);
            rightPanel.Controls.Add(pnlAttachments);

            splitContainer.Panel1.Controls.Add(treeView);
            splitContainer.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(splitContainer);
        }

        private void PopulateTree()
        {
            treeView.Nodes.Clear();

            foreach (var rec in _records)
            {
                string ccRef = rec.ContainsKey("careContextReference") ? rec["careContextReference"]?.ToString() ?? "Unknown CC" : "Unknown CC";
                var ccNode = new TreeNode($"Care Context: {ccRef}") { Tag = rec };

                if (rec.ContainsKey("fhirBundle") && rec["fhirBundle"] is Dictionary<string, object> bundle)
                {
                    if (bundle.ContainsKey("entry") && bundle["entry"] is List<object> entries)
                    {
                        foreach (var eObj in entries)
                        {
                            if (eObj is Dictionary<string, object> entry && entry.ContainsKey("resource") && entry["resource"] is Dictionary<string, object> res)
                            {
                                string rType = res.ContainsKey("resourceType") ? res["resourceType"]?.ToString() ?? "" : "";
                                
                                string nodeText = rType;
                                if (rType == "Composition") nodeText = "Clinical Document";
                                else if (rType == "Practitioner") nodeText = "Doctor Details";
                                else if (rType == "MedicationRequest") nodeText = "Prescription";
                                else if (rType == "Condition") nodeText = "Condition/Symptom";
                                else if (rType == "DocumentReference") nodeText = "Attachment";

                                var resNode = new TreeNode(nodeText) { Tag = res };
                                ccNode.Nodes.Add(resNode);
                            }
                        }
                    }
                }
                
                treeView.Nodes.Add(ccNode);
            }

            treeView.ExpandAll();
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            txtDetails.Clear();
            pnlAttachments.Controls.Clear();
            pnlAttachments.Visible = false;

            if (e.Node.Tag is Dictionary<string, object> tagData)
            {
                // Check if it's a full record (Care Context level)
                if (tagData.ContainsKey("careContextReference"))
                {
                    txtDetails.AppendText($"Select a specific resource under this Care Context to view details.");
                }
                // Check if it's a resource level
                else if (tagData.ContainsKey("resourceType"))
                {
                    string rType = tagData["resourceType"].ToString();
                    txtDetails.SelectionFont = new Font(txtDetails.Font, FontStyle.Bold);
                    txtDetails.AppendText($"Resource Type: {rType}\n\n");
                    txtDetails.SelectionFont = new Font(txtDetails.Font, FontStyle.Regular);

                    if (rType == "Composition")
                    {
                        if (tagData.ContainsKey("type") && tagData["type"] is Dictionary<string, object> tDict && tDict.ContainsKey("text"))
                        {
                            txtDetails.AppendText($"Type: {tDict["text"]}\n");
                        }
                        if (tagData.ContainsKey("status")) txtDetails.AppendText($"Status: {tagData["status"]}\n");
                    }
                    else if (rType == "Practitioner")
                    {
                        if (tagData.ContainsKey("name") && tagData["name"] is List<object> nList && nList.Count > 0 && nList[0] is Dictionary<string, object> nDict && nDict.ContainsKey("text"))
                        {
                            txtDetails.AppendText($"Name: {nDict["text"]}\n");
                        }
                    }
                    else if (rType == "MedicationRequest")
                    {
                        if (tagData.ContainsKey("medicationCodeableConcept") && tagData["medicationCodeableConcept"] is Dictionary<string, object> mDict && mDict.ContainsKey("text"))
                        {
                            txtDetails.AppendText($"Medication: {mDict["text"]}\n");
                        }
                    }
                    else if (rType == "Condition")
                    {
                        if (tagData.ContainsKey("code") && tagData["code"] is Dictionary<string, object> cDict && cDict.ContainsKey("text"))
                        {
                            txtDetails.AppendText($"Condition: {cDict["text"]}\n");
                        }
                    }
                    else if (rType == "DocumentReference")
                    {
                        txtDetails.AppendText("This resource contains an attachment. Click the button above to view it.");
                        
                        // Show attachment button
                        try
                        {
                            if (tagData.ContainsKey("content") && tagData["content"] is List<object> cList && cList.Count > 0)
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
                                            Text = $"Open Attachment ({ext})",
                                            Width = 200,
                                            Height = 35,
                                            BackColor = Color.CornflowerBlue,
                                            ForeColor = Color.White,
                                            FlatStyle = FlatStyle.Flat
                                        };
                                        btnOpen.Click += (s, args) => 
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
                                        
                                        pnlAttachments.Controls.Add(btnOpen);
                                        pnlAttachments.Visible = true;
                                    }
                                }
                            }
                        }
                        catch { /* ignore */ }
                    }

                    // Dump raw json of just this resource minus base64 data
                    var rawJson = System.Text.Json.JsonSerializer.Serialize(tagData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    rawJson = System.Text.RegularExpressions.Regex.Replace(rawJson, "\"data\"\\s*:\\s*\"[A-Za-z0-9+/=]{100,}\"", "\"data\": \"[BASE64_DATA_HIDDEN]\"");
                    
                    txtDetails.AppendText("\n\n--- Raw Details ---\n");
                    txtDetails.AppendText(rawJson);
                }
            }
        }
    }
}
