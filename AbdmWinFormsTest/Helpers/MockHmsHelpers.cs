using System;
using System.Data;

namespace HMS.abdm
{
    public static class MyClass
    {
        public static DataTable GetDataTable(string query, CommandType cmdType, object? parameters)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("AbhaNumber", typeof(string));
            dt.Columns.Add("AbhaAddress", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("DOB", typeof(string));
            dt.Columns.Add("Mobile", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("State", typeof(string));
            dt.Columns.Add("FatherName", typeof(string));

            bool loadedFromApi = false;

            if (query != null && query.Contains("tblAbdmScanShare"))
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(3);
                        System.Net.Http.HttpResponseMessage? resp = null;
                        try
                        {
                            resp = client.GetAsync("http://localhost:8082/api/v3/m1/scan-share-requests").GetAwaiter().GetResult();
                        }
                        catch
                        {
                            try
                            {
                                resp = client.GetAsync("http://localhost:5155/api/v3/m1/scan-share-requests").GetAwaiter().GetResult();
                            }
                            catch { }
                        }

                        if (resp != null && resp.IsSuccessStatusCode)
                        {
                            var json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            using (var doc = System.Text.Json.JsonDocument.Parse(json))
                            {
                                int rowId = 1;
                                foreach (var item in doc.RootElement.EnumerateArray())
                                {
                                    string abhaAddress = "";
                                    if (item.TryGetProperty("abhaAddress", out var abhaAddrProp))
                                        abhaAddress = abhaAddrProp.GetString() ?? "";

                                    string detailsStr = "";
                                    if (item.TryGetProperty("details", out var detailsProp))
                                        detailsStr = detailsProp.GetString() ?? "{}";

                                    string name = "";
                                    string abhaNumber = "";
                                    string gender = "";
                                    string dob = "";
                                    string mobile = "";
                                    string addressLine = "";
                                    string city = "";
                                    string state = "";
                                    string fatherName = "";

                                    try
                                    {
                                        using (var detailsDoc = System.Text.Json.JsonDocument.Parse(detailsStr))
                                        {
                                            var root = detailsDoc.RootElement;
                                            if (root.TryGetProperty("shareProfileRequest", out var shareReq))
                                            {
                                                if (shareReq.TryGetProperty("profile", out var profile))
                                                {
                                                    if (profile.TryGetProperty("patient", out var patient))
                                                    {
                                                        if (patient.TryGetProperty("name", out var propName)) name = propName.GetString() ?? "";
                                                        if (patient.TryGetProperty("abhaNumber", out var propAbha)) abhaNumber = propAbha.GetString() ?? "";
                                                        if (patient.TryGetProperty("gender", out var propGender)) gender = propGender.GetString() ?? "";
                                                        if (patient.TryGetProperty("phoneNumber", out var propPhone)) mobile = propPhone.GetString() ?? "";

                                                        string yob = "";
                                                        string mob = "";
                                                        string dobVal = "";
                                                        if (patient.TryGetProperty("yearOfBirth", out var propYob)) yob = propYob.GetString() ?? "";
                                                        if (patient.TryGetProperty("monthOfBirth", out var propMob)) mob = propMob.GetString() ?? "";
                                                        if (patient.TryGetProperty("dayOfBirth", out var propDob)) dobVal = propDob.GetString() ?? "";

                                                        if (!string.IsNullOrEmpty(yob))
                                                        {
                                                            dob = yob;
                                                            if (!string.IsNullOrEmpty(mob) && !string.IsNullOrEmpty(dobVal))
                                                                dob = $"{dobVal}-{mob}-{yob}";
                                                        }

                                                        if (patient.TryGetProperty("address", out var addr))
                                                        {
                                                            if (addr.TryGetProperty("line", out var propLine)) addressLine = propLine.GetString() ?? "";
                                                            if (addr.TryGetProperty("district", out var propDist)) city = propDist.GetString() ?? "";
                                                            if (addr.TryGetProperty("state", out var propState)) state = propState.GetString() ?? "";
                                                            if (addr.TryGetProperty("pincode", out var propPin))
                                                            {
                                                                string pin = propPin.GetString() ?? "";
                                                                if (!string.IsNullOrEmpty(pin))
                                                                    addressLine += $" - {pin}";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch { }

                                    dt.Rows.Add(
                                        rowId++,
                                        "Pending",
                                        string.IsNullOrEmpty(abhaNumber) ? "N/A" : abhaNumber,
                                        abhaAddress,
                                        string.IsNullOrEmpty(name) ? "Unknown" : name,
                                        gender == "M" ? "Male" : (gender == "F" ? "Female" : gender),
                                        dob,
                                        mobile,
                                        addressLine,
                                        city,
                                        state,
                                        fatherName
                                    );
                                }
                            }
                            if (dt.Rows.Count > 0)
                            {
                                loadedFromApi = true;
                            }
                        }
                    }
                }
                catch { }
            }

            return dt;
        }

        public static void Execute(string query, CommandType cmdType, object? parameters)
        {
            // Mock Execute
        }
    }

    public static class SessionStore
    {
        public static object Create(object? arg1, string key, object value)
        {
            return new object();
        }

        public static void Save(object session)
        {
            // Mock Save
        }
    }
}
