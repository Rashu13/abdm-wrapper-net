using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ABDM.Models;

namespace ABDM.Api
{
    /// <summary>
    /// Lightweight HTTP wrapper that calls the ABDM V3 REST API.
    /// Uses only built-in .NET 4.8 assemblies.
    /// </summary>
    public class AbdmApiClient
    {
        private readonly HttpClient   _http;
        private readonly AbdmSettings _cfg;
        private string _cachedAccessToken = "";
        private DateTime _cachedAccessTokenUtc = DateTime.MinValue;
        private string _cachedPublicKey = "";

        /// <summary>Stores the last raw API response for debugging purposes.</summary>
        public string LastRawResponse { get; private set; } = "";

        public AbdmSettings Settings => _cfg;

        public AbdmApiClient() : this(LoadSettingsFromConfig()) { }

        public AbdmApiClient(AbdmSettings settings)
        {
            _cfg  = settings;

            // Ensure TLS 1.2 is enabled for .NET Framework 4.8
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls11 |
                System.Net.SecurityProtocolType.Tls;

            _http = new HttpClient();
        }

        private static AbdmSettings LoadSettingsFromConfig()
        {
            var settings = new AbdmSettings
            {
                BaseUrl = "https://dev.abdm.gov.in/api/hiecm/gateway",
                AbhaServiceUrl = "https://abhasbx.abdm.gov.in/abha/api/v3",
                Environment = "Sandbox",
                CmId = "sbx"
            };
            return settings;
        }

        // ??? JSON helpers using DataContractJsonSerializer ????????????????????

        private static string ToJson(object obj)
        {
            // Simple but effective: use DataContractJsonSerializer
            // For anonymous types we use a manual approach
            // We'll pass pre-serialized dictionaries
            throw new NotSupportedException("Use SerializeDict instead.");
        }

        private StringContent JsonContent(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static AbdmResponse<T> Ok<T>(T data, string msg = "")
        {
            return new AbdmResponse<T> { Success = true, Data = data, Message = msg };
        }

        private static AbdmResponse<T> Fail<T>(string msg)
        {
            return new AbdmResponse<T> { Success = false, Message = msg };
        }

        /// <summary>Serialize a Dictionary to JSON string manually (no external library needed).</summary>
        private static string DictToJson(object obj)
        {
            return SimpleJson.Serialize(obj);
        }

        private void LogResponse(string title, string body)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "abdm_response_log.txt";
                string logEntry = string.Format("--- {0:yyyy-MM-dd HH:mm:ss} | {1} ---\n{2}\n\n", DateTime.Now, title, body);
                System.IO.File.AppendAllText(path, logEntry);
            }
            catch { }
        }

        // ??? Headers ??????????????????????????????????????????????????????????

        private void AddCommonHeaders(string accessToken = null)
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(accessToken))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

            var reqId = Guid.NewGuid().ToString();
            var ts    = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            _http.DefaultRequestHeaders.TryAddWithoutValidation("REQUEST-ID",   reqId);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("TIMESTAMP",    ts);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-REQUEST-ID", reqId);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-TIMESTAMP",  ts);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-CM-ID",      _cfg.CmId ?? "sbx");
            _http.DefaultRequestHeaders.TryAddWithoutValidation("guid",         reqId);

            if (!string.IsNullOrWhiteSpace(_cfg.HipId))
            {
                _http.DefaultRequestHeaders.TryAddWithoutValidation("X-HIP-ID", _cfg.HipId);
            }
        }

        private void AddProfileApiHeaders(string accessToken, string userToken)
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string cleanAccessToken = (accessToken ?? "").Trim();
            if (cleanAccessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                cleanAccessToken = cleanAccessToken.Substring(7).Trim();

            string cleanUserToken = (userToken ?? "").Trim();
            if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                cleanUserToken = cleanUserToken.Substring(7).Trim();

            var reqId = Guid.NewGuid().ToString();
            var ts    = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            _http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + cleanAccessToken);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-Token", "Bearer " + cleanUserToken);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("REQUEST-ID",   reqId);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("TIMESTAMP",    ts);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-REQUEST-ID", reqId);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-TIMESTAMP",  ts);
            _http.DefaultRequestHeaders.TryAddWithoutValidation("X-CM-ID",      _cfg.CmId ?? "sbx");

            if (!string.IsNullOrWhiteSpace(_cfg.HipId))
            {
                _http.DefaultRequestHeaders.TryAddWithoutValidation("X-HIP-ID", _cfg.HipId);
            }
        }

        // ??? Step 0 : Access Token ?????????????????????????????????????????????

        // Gateway base URL ∩┐╜ BaseUrl itself IS the gateway base
        // BaseUrl = "https://dev.abdm.gov.in/api/hiecm/gateway"
        // Token endpoint = BaseUrl + "/v3/sessions"
        private string GwBase
        {
            get { return _cfg.BaseUrl.TrimEnd('/'); }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                return "ConnectedToWrapper";
            }

            if (!string.IsNullOrWhiteSpace(_cachedAccessToken) &&
                _cachedAccessTokenUtc > DateTime.UtcNow.AddMinutes(1))
            {
                return _cachedAccessToken;
            }

            AddCommonHeaders();
            string body = SimpleJson.Serialize(new Dictionary<string, object>
            {
                ["clientId"]     = _cfg.ClientId,
                ["clientSecret"] = _cfg.ClientSecret,
                ["grantType"]    = "client_credentials"
            });

            var resp = await _http.PostAsync(
                $"{GwBase}/v3/sessions", JsonContent(body));

            var respBody = await resp.Content.ReadAsStringAsync();
            LastRawResponse = respBody;
            if (!resp.IsSuccessStatusCode)
            {
                if (respBody.IndexOf("SUSPENDED", StringComparison.OrdinalIgnoreCase) >= 0)
                    throw new Exception("Token Error: ABDM client/endpoint is suspended for the current credentials. " +
                        "Please use active sandbox/production credentials.\n" +
                        $"ClientId used: {_cfg.ClientId}\n" +
                        $"URL: {GwBase}/v3/sessions\n" +
                        $"Response: {respBody}");

                throw new Exception($"Token Error [{resp.StatusCode}]: {respBody}\n" +
                    $"ClientId used: {_cfg.ClientId}\n" +
                    $"URL: {GwBase}/v3/sessions");
            }

            var dict = SimpleJson.Deserialize(respBody);
            _cachedAccessToken = dict.ContainsKey("accessToken") ? dict["accessToken"]?.ToString() ?? "" : "";
            if (string.IsNullOrWhiteSpace(_cachedAccessToken))
                throw new Exception("Token Error: accessToken not found in ABDM session response.");

            int expiresInSeconds = 300;
            if (dict.ContainsKey("expiresIn") && int.TryParse(dict["expiresIn"]?.ToString(), out int parsedExpiresIn) && parsedExpiresIn > 0)
                expiresInSeconds = parsedExpiresIn;
            else if (dict.ContainsKey("expires_in") && int.TryParse(dict["expires_in"]?.ToString(), out parsedExpiresIn) && parsedExpiresIn > 0)
                expiresInSeconds = parsedExpiresIn;

            _cachedAccessTokenUtc = DateTime.UtcNow.AddSeconds(expiresInSeconds);
            return _cachedAccessToken;
        }

        // ??? Step 0b : Public Key ??????????????????????????????????????????????

        public async Task<string> GetPublicKeyAsync(string accessToken)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                return "ConnectedToWrapperPublicKey";
            }

            if (!string.IsNullOrEmpty(_cachedPublicKey))
                return _cachedPublicKey;

            // Try V3 endpoint first, fallback to V1 if 404
            var endpoints = new[]
            {
                $"{_cfg.AbhaServiceUrl}/profile/public/certificate",
                $"{_cfg.BaseUrl.Replace("/api/hiecm/gateway", "")}/abha/api/v1/profile/public/certificate"
            };

            Exception lastEx = null;
            foreach (var endpoint in endpoints)
            {
                try
                {
                    AddCommonHeaders(accessToken);
                    var resp = await _http.GetAsync(endpoint);
                    var respBody = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode) { lastEx = new Exception($"PublicKey Error [{resp.StatusCode}]: {respBody}"); continue; }

                    // Plain PEM string
                    if (respBody.TrimStart().StartsWith("-----BEGIN"))
                        return respBody.Trim();

                    // JSON response ∩┐╜ try common key names
                    if (respBody.TrimStart().StartsWith("{"))
                    {
                        var dict = SimpleJson.Deserialize(respBody);
                        foreach (var key in new[] { "publicKey", "cert", "certificate", "public_key" })
                            if (dict.ContainsKey(key) && !string.IsNullOrEmpty(dict[key]?.ToString()))
                                return dict[key].ToString();
                    }
                    // Fallback: return raw body as-is (might be plain base64 or PEM)
                    _cachedPublicKey = respBody.Trim();
                    return _cachedPublicKey;
                }
                catch (Exception ex) { lastEx = ex; }
            }
            throw lastEx ?? new Exception("PublicKey Error: All endpoints failed.");
        }

        // ??? RSA Encrypt (OAEP SHA-1 ∩┐╜ built-in .NET 4.8) ????????????????????

        public string Encrypt(string data, string publicKeyPem)
        {
            string cleanPublicKey = publicKeyPem
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----",   "")
                .Replace("\n", "").Replace("\r", "").Replace(" ", "");

            byte[] keyBytes = Convert.FromBase64String(cleanPublicKey);
            var keyParts = ParseSpkiDer(keyBytes);
            byte[] modulus = keyParts.Modulus;
            byte[] exponent = keyParts.Exponent;

            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider())
            {
                var rsaParams = new System.Security.Cryptography.RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                rsa.ImportParameters(rsaParams);

                byte[] encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(data), true); // OAEP SHA-1
                return Convert.ToBase64String(encrypted);
            }
        }

        private struct RsaKeyParts
        {
            public byte[] Modulus;
            public byte[] Exponent;
        }

        private static RsaKeyParts ParseSpkiDer(byte[] der)
        {
            RsaKeyParts parts = new RsaKeyParts();
            int idx = 0;
            ReadSeqLen(der, ref idx);          // outer SEQUENCE
            int algLen = ReadSeqLen(der, ref idx); // algorithm SEQUENCE
            idx += algLen;                         // skip algorithm contents

            if (der[idx++] != 0x03) throw new Exception("Expected BIT STRING");
            ReadLen(der, ref idx);
            idx++;                                 // skip unused-bits byte

            if (der[idx++] != 0x30) throw new Exception("Expected inner SEQUENCE");
            ReadLen(der, ref idx);

            if (der[idx++] != 0x02) throw new Exception("Expected INTEGER (modulus)");
            int modLen = ReadLen(der, ref idx);
            if (der[idx] == 0x00) { idx++; modLen--; }
            byte[] modulus = new byte[modLen];
            Array.Copy(der, idx, modulus, 0, modLen);
            idx += modLen;

            if (der[idx++] != 0x02) throw new Exception("Expected INTEGER (exponent)");
            int expLen = ReadLen(der, ref idx);
            byte[] exponent = new byte[expLen];
            Array.Copy(der, idx, exponent, 0, expLen);

            parts.Modulus = modulus;
            parts.Exponent = exponent;
            return parts;
        }

        private static int ReadSeqLen(byte[] d, ref int i)
        {
            if (d[i++] != 0x30) throw new Exception("Expected SEQUENCE");
            return ReadLen(d, ref i);
        }
        private static int ReadLen(byte[] d, ref int i)
        {
            if (d[i] < 0x80) return d[i++];
            int bytes = d[i++] & 0x7F;
            int len   = 0;
            for (int b = 0; b < bytes; b++) len = (len << 8) | d[i++];
            return len;
        }

        // --- Redirection to Wrapper Helpers ---

        private string BaseUrl => _cfg.BaseUrl.TrimEnd('/');

        private async Task<string> PostToWrapperAsync(string path, string jsonPayload, string? userToken = null)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}{path}"))
            {
                requestMessage.Content = JsonContent(jsonPayload);
                if (!string.IsNullOrEmpty(userToken))
                {
                    string cleanUserToken = userToken.Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    requestMessage.Headers.TryAddWithoutValidation("X-Token", "Bearer " + cleanUserToken);
                }
                var resp = await _http.SendAsync(requestMessage);
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Wrapper Error [{resp.StatusCode}]: {body}");
                }
                return body;
            }
        }

        private async Task<string> PutToWrapperAsync(string path, string jsonPayload, string? userToken = null)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}{path}"))
            {
                requestMessage.Content = JsonContent(jsonPayload);
                if (!string.IsNullOrEmpty(userToken))
                {
                    string cleanUserToken = userToken.Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    requestMessage.Headers.TryAddWithoutValidation("X-Token", "Bearer " + cleanUserToken);
                }
                var resp = await _http.SendAsync(requestMessage);
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Wrapper Error [{resp.StatusCode}]: {body}");
                }
                return body;
            }
        }

        private async Task<string> GetFromWrapperAsync(string path, string? userToken = null)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}"))
            {
                if (!string.IsNullOrEmpty(userToken))
                {
                    string cleanUserToken = userToken.Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    requestMessage.Headers.TryAddWithoutValidation("X-Token", "Bearer " + cleanUserToken);
                }
                var resp = await _http.SendAsync(requestMessage);
                if (resp.Content.Headers.ContentType?.MediaType == "image/png" || path.Contains("/card"))
                {
                    var bytes = await resp.Content.ReadAsByteArrayAsync();
                    if (!resp.IsSuccessStatusCode)
                    {
                        var text = Encoding.UTF8.GetString(bytes);
                        throw new Exception($"Wrapper Error [{resp.StatusCode}]: {text}");
                    }
                    return Convert.ToBase64String(bytes);
                }
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Wrapper Error [{resp.StatusCode}]: {body}");
                }
                return body;
            }
        }

        private static AbhaProfile ParseProfileDict(Dictionary<string, object> acc)
        {
            var profile = new AbhaProfile
            {
                HealthIdNumber = acc.ContainsKey("ABHANumber") ? acc["ABHANumber"]?.ToString() : 
                                 acc.ContainsKey("healthIdNumber") ? acc["healthIdNumber"]?.ToString() : "",
                AbhaAddress    = acc.ContainsKey("preferredAbhaAddress") ? acc["preferredAbhaAddress"]?.ToString() : 
                                 acc.ContainsKey("abhaAddress") ? acc["abhaAddress"]?.ToString() : "",
                Name           = acc.ContainsKey("name") ? acc["name"]?.ToString() : "",
                Gender         = acc.ContainsKey("gender") ? acc["gender"]?.ToString() : "",
                Mobile         = acc.ContainsKey("mobile") ? acc["mobile"]?.ToString() : "",
                Address        = acc.ContainsKey("address") ? acc["address"]?.ToString() : "",
                YearOfBirth    = acc.ContainsKey("yearOfBirth") ? acc["yearOfBirth"]?.ToString() : 
                                 acc.ContainsKey("dob") ? acc["dob"]?.ToString() : "",
                ProfilePhoto   = acc.ContainsKey("profilePhoto") ? acc["profilePhoto"]?.ToString() : ""
            };

            if (acc.ContainsKey("tokens") && acc["tokens"] is Dictionary<string, object> tok)
            {
                profile.Token = tok.ContainsKey("token") ? tok["token"]?.ToString() : "";
                profile.RefreshToken = tok.ContainsKey("refreshToken") ? tok["refreshToken"]?.ToString() : "";
            }
            else
            {
                profile.Token = acc.ContainsKey("token") ? acc["token"]?.ToString() : "";
                profile.RefreshToken = acc.ContainsKey("refreshToken") ? acc["refreshToken"]?.ToString() : "";
            }

            return profile;
        }

        // ??? Generate OTP ???????????????????????????????????????????????????????

        public async Task<AbdmResponse<OtpTransactionResponse>> GenerateOtpAsync(
            AbdmGenerateOtpRequest request)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string payload = SimpleJson.Serialize(request);
                    string responseBody = await PostToWrapperAsync("/api/v3/m1/generate-otp", payload);
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<OtpTransactionResponse>("Invalid response format from wrapper");
                    
                    var data = new OtpTransactionResponse
                    {
                        TransactionId = dataDict.ContainsKey("transactionId") ? dataDict["transactionId"]?.ToString() : "",
                        Message       = dataDict.ContainsKey("message") ? dataDict["message"]?.ToString() : "",
                        MaskedMobile  = dataDict.ContainsKey("maskedMobile") ? dataDict["maskedMobile"]?.ToString() : ""
                    };
                    return AbdmApiClient.Ok(data, "OTP Sent.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token     = await GetAccessTokenAsync();
                    var publicKey = await GetPublicKeyAsync(token);
                    var encrypted = Encrypt(request.LoginId.Trim(), publicKey);

                    long dummy;
                    bool isAadhaar = request.LoginType?.ToUpper() == "AADHAAR"
                                  || (request.LoginId.Trim().Length == 12
                                      && long.TryParse(request.LoginId.Trim(), out dummy));

                    AddCommonHeaders(token);
                    string endpoint;
                    Dictionary<string, object> payloadDict;

                    if (isAadhaar)
                    {
                        // Aadhaar flow: enrollment/request/otp
                        endpoint = $"{_cfg.AbhaServiceUrl}/enrollment/request/otp";
                        payloadDict = new Dictionary<string, object>
                        {
                            ["clientId"]  = _cfg.ClientId,
                            ["txnId"]     = "",
                            ["scope"]     = new[] { "abha-enrol" },
                            ["loginHint"] = "aadhaar",
                            ["loginId"]   = encrypted,
                            ["otpSystem"] = "aadhaar"
                        };
                    }
                    else
                    {
                        // Mobile flow: profile/login/request/otp (existing ABHA login)
                        endpoint = $"{_cfg.AbhaServiceUrl}/profile/login/request/otp";
                        payloadDict = new Dictionary<string, object>
                        {
                            ["scope"]     = new[] { "abha-login", "mobile-verify" },
                            ["loginHint"] = "mobile",
                            ["loginId"]   = encrypted,
                            ["otpSystem"] = "abdm"
                        };
                    }

                    var payload = SimpleJson.Serialize(payloadDict);
                    var resp    = await _http.PostAsync(endpoint, JsonContent(payload));
                    var body    = await resp.Content.ReadAsStringAsync();
                    LogResponse("GenerateOtpAsync", body);

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<OtpTransactionResponse>($"OTP Error [{resp.StatusCode}]: {body}");

                    var d    = SimpleJson.Deserialize(body);
                    var data = new OtpTransactionResponse
                    {
                        TransactionId = d.ContainsKey("txnId")   ? d["txnId"]?.ToString()   : "",
                        Message       = d.ContainsKey("message") ? d["message"]?.ToString() : "",
                        MaskedMobile  = d.ContainsKey("mobileNumber") ? d["mobileNumber"]?.ToString()
                                      : d.ContainsKey("maskedMobile") ? d["maskedMobile"]?.ToString()
                                      : d.ContainsKey("mobile") ? d["mobile"]?.ToString()
                                      : ""
                    };
                    return AbdmApiClient.Ok(data, isAadhaar ? "Aadhaar OTP Sent." : "Mobile OTP Sent.");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message); }
            }
        }

        // ??? Verify OTP ?????????????????????????????????????????????????????????

        public async Task<AbdmResponse<AbhaProfile>> VerifyOtpAsync(
            AbdmVerifyOtpRequest request)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string payload = SimpleJson.Serialize(request);
                    string responseBody = await PostToWrapperAsync("/api/v3/m1/verify-otp", payload);
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<AbhaProfile>("Invalid response format from wrapper");

                    var profile = ParseProfileDict(dataDict);
                    return AbdmApiClient.Ok(profile, "OTP Verified.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<AbhaProfile>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token     = await GetAccessTokenAsync();
                    var publicKey = await GetPublicKeyAsync(token);
                    var encOtp    = Encrypt(request.Otp, publicKey);
                    bool isAadhaar = request.LoginType?.ToUpper() == "AADHAAR";
                    var ts = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    AddCommonHeaders(token);
                    string url;
                    string payloadJson;

                    if (isAadhaar)
                    {
                        // Aadhaar enrollment verify
                        url = $"{_cfg.AbhaServiceUrl}/enrollment/enrol/byAadhaar";
                        payloadJson = SimpleJson.Serialize(new Dictionary<string, object>
                        {
                            ["authData"] = new Dictionary<string, object>
                            {
                                ["authMethods"] = new[] { "otp" },
                                ["otp"] = new Dictionary<string, object>
                                {
                                    ["timeStamp"] = ts,
                                    ["txnId"]     = request.TransactionId,
                                    ["otpValue"]  = encOtp,
                                    ["mobile"]    = request.Mobile
                                }
                            },
                            ["consent"] = new Dictionary<string, object>
                            {
                                ["code"]    = "abha-enrollment",
                                ["version"] = "1.4"
                            }
                        });
                    }
                    else
                    {
                        // Mobile login verify ? profile/login/verify
                        url = $"{_cfg.AbhaServiceUrl}/profile/login/verify";
                        payloadJson = SimpleJson.Serialize(new Dictionary<string, object>
                        {
                            ["scope"] = new[] { "abha-login", "mobile-verify" },
                            ["authData"] = new Dictionary<string, object>
                            {
                                ["authMethods"] = new[] { "otp" },
                                ["otp"] = new Dictionary<string, object>
                                {
                                    ["timeStamp"] = ts,
                                    ["txnId"]     = request.TransactionId,
                                    ["otpValue"]  = encOtp
                                }
                            }
                        });
                    }

                    var resp = await _http.PostAsync(url, JsonContent(payloadJson));
                    var body = await resp.Content.ReadAsStringAsync();
                    LastRawResponse = body;
                    LogResponse("VerifyOtpAsync", body);

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<AbhaProfile>($"Verify Error [{resp.StatusCode}]: {body}");

                    var profile = ParseAbhaProfile(body);
                    return AbdmApiClient.Ok(profile, "OTP Verified Successfully.");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<AbhaProfile>(ex.Message); }
            }
        }

        // ??? Login Request OTP ?????????????????????????????????????????????????

        public async Task<AbdmResponse<OtpTransactionResponse>> LoginRequestOtpAsync(
            AbdmGenerateOtpRequest request)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string payload = SimpleJson.Serialize(request);
                    string responseBody = await PostToWrapperAsync("/api/v3/m1/login-otp", payload);
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<OtpTransactionResponse>("Invalid response format from wrapper");

                    var data = new OtpTransactionResponse
                    {
                        TransactionId = dataDict.ContainsKey("transactionId") ? dataDict["transactionId"]?.ToString() : "",
                        Message       = dataDict.ContainsKey("message") ? dataDict["message"]?.ToString() : ""
                    };
                    return AbdmApiClient.Ok(data, "Login OTP Sent.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token     = await GetAccessTokenAsync();
                    var publicKey = await GetPublicKeyAsync(token);
                    var encrypted = Encrypt(request.LoginId.Trim(), publicKey);

                    string loginIdClean = request.LoginId?.Replace("-", "").Trim() ?? "";
                    string loginHint = "mobile";
                    string otpSystem = "abdm";
                    string[] scope = new[] { "abha-login", "mobile-verify" };

                    if (request.LoginType?.ToUpper() == "AADHAAR" || (loginIdClean.Length == 12 && System.Text.RegularExpressions.Regex.IsMatch(loginIdClean, "^[0-9]+$")))
                    {
                        loginHint = "aadhaar";
                        otpSystem = "aadhaar";
                        scope = new[] { "abha-login", "aadhaar-verify" };
                    }
                    else if (loginIdClean.Length == 14 && System.Text.RegularExpressions.Regex.IsMatch(loginIdClean, "^[0-9]+$"))
                    {
                        loginHint = "abha-number";
                        if (request.LoginType?.ToUpper() == "AADHAAR")
                        {
                            otpSystem = "aadhaar";
                            scope = new[] { "abha-login", "aadhaar-verify" };
                        }
                        else
                        {
                            otpSystem = "abdm";
                            scope = new[] { "abha-login", "mobile-verify" };
                        }
                    }
                    else if (loginIdClean.Contains("@"))
                    {
                        loginHint = "abha-address";
                        otpSystem = "abdm";
                        scope = new[] { "abha-login", "mobile-verify" };
                    }

                    AddCommonHeaders(token);
                    var payload = SimpleJson.Serialize(new Dictionary<string, object>
                    {
                        ["scope"]     = scope,
                        ["loginHint"] = loginHint,
                        ["loginId"]   = encrypted,
                        ["otpSystem"] = otpSystem
                    });

                    var resp = await _http.PostAsync(
                        $"{_cfg.AbhaServiceUrl}/profile/login/request/otp", JsonContent(payload));
                    var body = await resp.Content.ReadAsStringAsync();
                    LogResponse("LoginRequestOtpAsync", body);

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<OtpTransactionResponse>($"Login OTP Error [{resp.StatusCode}]: {body}");

                    var d    = SimpleJson.Deserialize(body);
                    var data = new OtpTransactionResponse
                    {
                        TransactionId = d.ContainsKey("txnId")   ? d["txnId"]?.ToString()   : "",
                        Message       = d.ContainsKey("message") ? d["message"]?.ToString() : ""
                    };
                    return AbdmApiClient.Ok(data, "Login OTP Sent.");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message); }
            }
        }

        public async Task<AbdmResponse<AbhaSuggestionResponse>> GetAbhaSuggestionsAsync(string txnId)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string responseBody = await GetFromWrapperAsync($"/api/v3/m1/suggestions/{txnId}");
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<AbhaSuggestionResponse>("Invalid response format from wrapper");

                    var result = new AbhaSuggestionResponse
                    {
                        TransactionId = dataDict.ContainsKey("transactionId") ? dataDict["transactionId"]?.ToString() : "",
                        AbhaAddressList = new List<string>()
                    };

                    if (dataDict.ContainsKey("abhaAddressList") && dataDict["abhaAddressList"] is List<object>)
                    {
                        var list = dataDict["abhaAddressList"] as List<object>;
                        if (list != null)
                        {
                            foreach (var item in list)
                                result.AbhaAddressList.Add(item?.ToString() ?? "");
                        }
                    }
                    return AbdmApiClient.Ok(result);
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<AbhaSuggestionResponse>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token = await GetAccessTokenAsync();
                    AddCommonHeaders(token);

                    string url = $"{_cfg.AbhaServiceUrl}/enrollment/enrol/suggestion";
                    string payload = SimpleJson.Serialize(new Dictionary<string, object>
                    {
                        ["txnId"] = txnId
                    });

                    var resp = await _http.PostAsync(url, JsonContent(payload));
                    var body = await resp.Content.ReadAsStringAsync();
                    LastRawResponse = body;

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<AbhaSuggestionResponse>($"Suggestion Error [{resp.StatusCode}]: {body}");

                    var dict = SimpleJson.Deserialize(body);
                    var result = new AbhaSuggestionResponse
                    {
                        TransactionId = dict.ContainsKey("txnId") ? dict["txnId"]?.ToString() : "",
                        AbhaAddressList = new List<string>()
                    };

                    if (dict.ContainsKey("abhaAddressList") && dict["abhaAddressList"] is List<object>)
                    {
                        var list = dict["abhaAddressList"] as List<object>;
                        if (list != null)
                        {
                            foreach (var item in list)
                                result.AbhaAddressList.Add(item?.ToString() ?? "");
                        }
                    }

                    return AbdmApiClient.Ok(result);
                }
                catch (Exception ex) { return AbdmApiClient.Fail<AbhaSuggestionResponse>(ex.Message); }
            }
        }

        // ??? Login Verify OTP ??????????????????????????????????????????????????

        public async Task<AbdmResponse<AbdmLoginResponse>> LoginVerifyOtpAsync(
            AbdmVerifyOtpRequest request)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string payload = SimpleJson.Serialize(request);
                    string responseBody = await PostToWrapperAsync("/api/v3/m1/login-verify", payload);
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<AbdmLoginResponse>("Invalid response format from wrapper");

                    var loginResp = new AbdmLoginResponse
                    {
                        IsNew        = dataDict.ContainsKey("isNew") && dataDict["isNew"] != null && dataDict["isNew"].ToString().ToLower() == "true",
                        Token        = dataDict.ContainsKey("token")        ? dataDict["token"]?.ToString()        : "",
                        RefreshToken = dataDict.ContainsKey("refreshToken") ? dataDict["refreshToken"]?.ToString() : "",
                        Accounts     = new List<AbhaProfile>()
                    };

                    var accts = dataDict.ContainsKey("accounts") ? dataDict["accounts"] as List<object> : null;
                    if (accts != null)
                    {
                        foreach (var a in accts)
                        {
                            var acc = a as Dictionary<string, object>;
                            if (acc != null)
                            {
                                loginResp.Accounts.Add(ParseProfileDict(acc));
                            }
                        }
                    }
                    return AbdmApiClient.Ok(loginResp, "Login Successful.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<AbdmLoginResponse>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token     = await GetAccessTokenAsync();
                    var publicKey = await GetPublicKeyAsync(token);
                    var encOtp    = Encrypt(request.Otp, publicKey);

                    string[] scope = new[] { "abha-login", "mobile-verify" };
                    if (request.LoginType?.ToUpper() == "AADHAAR")
                    {
                        scope = new[] { "abha-login", "aadhaar-verify" };
                    }

                    AddCommonHeaders(token);
                    var payload = SimpleJson.Serialize(new Dictionary<string, object>
                    {
                        ["scope"] = scope,
                        ["authData"] = new Dictionary<string, object>
                        {
                            ["authMethods"] = new[] { "otp" },
                            ["otp"] = new Dictionary<string, object>
                            {
                                ["txnId"]    = request.TransactionId,
                                ["otpValue"] = encOtp
                            }
                        }
                    });

                    var resp = await _http.PostAsync(
                        $"{_cfg.AbhaServiceUrl}/profile/login/verify", JsonContent(payload));
                    var body = await resp.Content.ReadAsStringAsync();
                    LogResponse("LoginVerifyOtpAsync", body);

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<AbdmLoginResponse>($"Login Verify Error [{resp.StatusCode}]: {body}");

                    var root = SimpleJson.Deserialize(body);
                    var loginResp = new AbdmLoginResponse
                    {
                        IsNew        = root.ContainsKey("isNew") && root["isNew"] != null && root["isNew"].ToString().ToLower() == "true",
                        Token        = root.ContainsKey("token")        ? root["token"]?.ToString()        : "",
                        RefreshToken = root.ContainsKey("refreshToken") ? root["refreshToken"]?.ToString() : "",
                        Accounts     = new List<AbhaProfile>()
                    };

                    var accts = root.ContainsKey("accounts") ? root["accounts"] as List<object> : null;
                    if (accts != null)
                    {
                        foreach (var a in accts)
                        {
                            var acc = a as Dictionary<string, object>;
                            if (acc != null)
                            {
                                loginResp.Accounts.Add(new AbhaProfile
                                {
                                    HealthIdNumber = acc.ContainsKey("ABHANumber")            ? acc["ABHANumber"]?.ToString()            : "",
                                    Name           = acc.ContainsKey("name")                  ? acc["name"]?.ToString()                  : "",
                                    AbhaAddress    = acc.ContainsKey("preferredAbhaAddress")  ? acc["preferredAbhaAddress"]?.ToString()  : "",
                                    ProfilePhoto   = acc.ContainsKey("profilePhoto")          ? acc["profilePhoto"]?.ToString()          : ""
                                });
                            }
                        }
                    }

                    // Exchanging temporary T-token for final X-token
                    if (!string.IsNullOrEmpty(loginResp.Token) && loginResp.Accounts.Count > 0)
                    {
                        try
                        {
                            var firstAccount = loginResp.Accounts[0];
                            string abhaNum = firstAccount.HealthIdNumber;
                            string verifyUserTxnId = root.ContainsKey("txnId") ? root["txnId"]?.ToString() : request.TransactionId;

                            // Call /profile/login/verify/user
                            AddCommonHeaders(token);
                            _http.DefaultRequestHeaders.Remove("T-token");
                            _http.DefaultRequestHeaders.TryAddWithoutValidation("T-token", $"Bearer {loginResp.Token}");

                            var verifyUserPayload = SimpleJson.Serialize(new Dictionary<string, object>
                            {
                                ["ABHANumber"] = abhaNum,
                                ["txnId"]      = verifyUserTxnId
                            });

                            var verifyUserResp = await _http.PostAsync(
                                $"{_cfg.AbhaServiceUrl}/profile/login/verify/user", JsonContent(verifyUserPayload));
                            
                            var verifyUserBody = await verifyUserResp.Content.ReadAsStringAsync();
                            LogResponse("LoginVerifyUser", verifyUserBody);

                            if (verifyUserResp.IsSuccessStatusCode)
                            {
                                var verifyUserDict = SimpleJson.Deserialize(verifyUserBody);
                                if (verifyUserDict.ContainsKey("token"))
                                {
                                    loginResp.Token = verifyUserDict["token"]?.ToString() ?? loginResp.Token;
                                    if (verifyUserDict.ContainsKey("refreshToken"))
                                    {
                                        loginResp.RefreshToken = verifyUserDict["refreshToken"]?.ToString() ?? loginResp.RefreshToken;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error exchanging T-Token: " + ex.Message);
                        }
                        finally
                        {
                            _http.DefaultRequestHeaders.Remove("T-token");
                        }
                    }

                    return AbdmApiClient.Ok(loginResp, "Login Successful.");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<AbdmLoginResponse>(ex.Message); }
            }
        }

        // ??? Get Profile ???????????????????????????????????????????????????????

        public async Task<AbdmResponse<AbhaProfile>> GetAbhaProfileAsync(string userToken)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string responseBody = await GetFromWrapperAsync("/api/v3/m1/profile", userToken);
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<AbhaProfile>("Invalid response format from wrapper");

                    var profile = ParseProfileDict(dataDict);
                    profile.IsNew = false;
                    return AbdmApiClient.Ok(profile, "Profile fetched successfully.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<AbhaProfile>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token = await GetAccessTokenAsync();
                    AddCommonHeaders(token);

                    string cleanUserToken = (userToken ?? "").Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();

                    string endpoint = "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account";
                    if (!string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                        _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        endpoint = "https://abha.abdm.gov.in/abha/api/v3/profile/account";
                    }

                    _http.DefaultRequestHeaders.Remove("X-Token");
                    _http.DefaultRequestHeaders.Remove("X-token");
                    _http.DefaultRequestHeaders.TryAddWithoutValidation(
                        "X-Token", $"Bearer {cleanUserToken}");

                    var resp = await _http.GetAsync(endpoint);
                    var body = await resp.Content.ReadAsStringAsync();
                    LastRawResponse = body;
                    LogResponse("GetAbhaProfileAsync", body);

                    if (!resp.IsSuccessStatusCode)
                    {
                        if (body.IndexOf("invalid x-token", StringComparison.OrdinalIgnoreCase) >= 0)
                            return AbdmApiClient.Fail<AbhaProfile>("Session expired ya invalid ho gayi hai. Dobara login karein.\nEndpoint: " + endpoint);

                        return AbdmApiClient.Fail<AbhaProfile>($"Profile Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");
                    }

                    var profile = ParseAbhaProfile(body);
                    profile.IsNew = false;
                    return AbdmApiClient.Ok(profile, "Profile fetched successfully (v3).");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<AbhaProfile>(ex.Message); }
            }
        }

        // ??? Create ABHA Address ???????????????????????????????????????????

        public async Task<AbdmResponse<CreateAbhaAddressResponse>> CreateAbhaAddressAsync(
            string txnId, string abhaAddress, string userToken = null)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string localPart = abhaAddress;
                    if (localPart.Contains("@"))
                        localPart = localPart.Substring(0, localPart.IndexOf('@'));

                    string path = $"/api/v3/m1/create-abha?txnId={txnId}&abhaAddress={Uri.EscapeDataString(localPart)}";
                    string responseBody = await PostToWrapperAsync(path, "{}", userToken);
                    var dict = SimpleJson.Deserialize(responseBody);
                    var dataDict = dict.ContainsKey("data") ? dict["data"] as Dictionary<string, object> : null;
                    if (dataDict == null) return AbdmApiClient.Fail<CreateAbhaAddressResponse>("Invalid response format from wrapper");

                    var result = new CreateAbhaAddressResponse
                    {
                        AbhaAddress = dataDict.ContainsKey("abhaAddress") ? dataDict["abhaAddress"]?.ToString() : abhaAddress,
                        HealthIdNumber = dataDict.ContainsKey("healthIdNumber") ? dataDict["healthIdNumber"]?.ToString() : "",
                        Status = dataDict.ContainsKey("status") ? dataDict["status"]?.ToString() : "ACTIVE"
                    };
                    return AbdmApiClient.Ok(result, "ABHA Address created successfully.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<CreateAbhaAddressResponse>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token = await GetAccessTokenAsync();
                    AddCommonHeaders(token);

                    string endpoint = "https://abhasbx.abdm.gov.in/abha/api/v3/enrollment/enrol/abha-address";
                    if (!string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                        _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        endpoint = "https://abha.abdm.gov.in/abha/api/v3/enrollment/enrol/abha-address";
                    }

                    // If we have a user-token (from enrollment verify), pass it as X-Token
                    if (!string.IsNullOrEmpty(userToken))
                    {
                        string cleanUserToken = userToken.Trim();
                        if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            cleanUserToken = cleanUserToken.Substring(7).Trim();
                        _http.DefaultRequestHeaders.TryAddWithoutValidation(
                            "X-Token", $"Bearer {cleanUserToken}");
                    }

                    // Strip @abdm suffix - API expects just the local part
                    string localPart = abhaAddress;
                    if (localPart.Contains("@"))
                        localPart = localPart.Substring(0, localPart.IndexOf('@'));

                    var payload = SimpleJson.Serialize(new Dictionary<string, object>
                    {
                        ["txnId"]       = txnId,
                        ["abhaAddress"] = localPart,
                        ["preferred"]   = 1
                    });

                    var resp = await _http.PostAsync(endpoint, JsonContent(payload));
                    var body = await resp.Content.ReadAsStringAsync();
                    LastRawResponse = body;

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<CreateAbhaAddressResponse>($"ABHA Address Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");

                    var d = SimpleJson.Deserialize(body);
                    var result = new CreateAbhaAddressResponse
                    {
                        AbhaAddress = d.ContainsKey("ABHAAddress") ? d["ABHAAddress"]?.ToString() 
                                    : d.ContainsKey("abhaAddress") ? d["abhaAddress"]?.ToString() 
                                    : abhaAddress,
                        HealthIdNumber = d.ContainsKey("healthIdNumber") ? d["healthIdNumber"]?.ToString() : "",
                        Status = d.ContainsKey("status") ? d["status"]?.ToString() : "ACTIVE"
                    };

                    return AbdmApiClient.Ok(result, "ABHA Address created successfully.");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<CreateAbhaAddressResponse>(ex.Message); }
            }
        }

        // ??? Get ABHA Card (PNG) ??????????????????????????????????????????????

        public async Task<AbdmResponse<AbhaCardResponse>> GetAbhaCardAsync(string userToken)
        {
            if (!_cfg.BaseUrl.Contains("abdm.gov.in"))
            {
                try
                {
                    string base64Content = await GetFromWrapperAsync("/api/v3/m1/card", userToken);
                    return AbdmApiClient.Ok(new AbhaCardResponse
                    {
                        ContentType = "image/png",
                        Content     = base64Content
                    }, "Card fetched.");
                }
                catch (Exception ex)
                {
                    return AbdmApiClient.Fail<AbhaCardResponse>(ex.Message);
                }
            }
            else
            {
                try
                {
                    var token = await GetAccessTokenAsync();
                    AddCommonHeaders(token);
                    string cleanUserToken = (userToken ?? "").Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    _http.DefaultRequestHeaders.Remove("X-Token");
                    _http.DefaultRequestHeaders.Remove("X-token");
                    _http.DefaultRequestHeaders.TryAddWithoutValidation(
                        "X-Token", $"Bearer {cleanUserToken}");
                    _http.DefaultRequestHeaders.Accept.Clear();
                    _http.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("image/png"));
                    _http.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    var resp = await _http.GetAsync($"{_cfg.AbhaServiceUrl}/profile/account/abha-card");
                    var bytes = await resp.Content.ReadAsByteArrayAsync();

                    if (!resp.IsSuccessStatusCode)
                        return AbdmApiClient.Fail<AbhaCardResponse>($"Card Error [{resp.StatusCode}]: {System.Text.Encoding.UTF8.GetString(bytes)}");

                    var contentType = resp.Content.Headers.ContentType?.MediaType ?? "image/png";
                    return AbdmApiClient.Ok(new AbhaCardResponse
                    {
                        ContentType = contentType,
                        Content     = Convert.ToBase64String(bytes)
                    }, "Card fetched.");
                }
                catch (Exception ex) { return AbdmApiClient.Fail<AbhaCardResponse>(ex.Message); }
            }
        }

        // ?? Mobile Update : Send OTP ?????????????????????????????????????????
        // POST /abha/api/v3/enrollment/request/otp
        // body: { txnId, scope:["abha-enrol","mobile-verify"], loginHint:"mobile", loginId, otpSystem:"abdm" }

        public async Task<AbdmResponse<OtpTransactionResponse>> MobileUpdateSendOtpAsync(
            MobileUpdateOtpRequest request)
        {
            try
            {
                var token     = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(token);
                var encrypted = Encrypt(request.Mobile.Trim(), publicKey);

                AddCommonHeaders(token);
                if (!string.IsNullOrEmpty(request.UserToken))
                {
                    string cleanUserToken = request.UserToken.Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    _http.DefaultRequestHeaders.TryAddWithoutValidation(
                        "X-Token", $"Bearer {cleanUserToken}");
                }

                var payloadDict = new Dictionary<string, object>
                {
                    ["clientId"]  = _cfg.ClientId,
                    ["txnId"]     = request.TransactionId ?? "",
                    ["scope"]     = new[] { "abha-enrol", "mobile-verify" },
                    ["loginHint"] = "mobile",
                    ["loginId"]   = encrypted,
                    ["otpSystem"] = "abdm"
                };

                var payload = SimpleJson.Serialize(payloadDict);
                var resp = await _http.PostAsync(
                    $"{_cfg.AbhaServiceUrl}/enrollment/request/otp",
                    JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                {
                    if (body.IndexOf("Invalid X-token", StringComparison.OrdinalIgnoreCase) >= 0)
                        return AbdmApiClient.Fail<OtpTransactionResponse>("Mobile OTP Error: user session token invalid hai. Dobara login/enroll karke profile reopen karein. Response: " + body);

                    return AbdmApiClient.Fail<OtpTransactionResponse>($"Mobile OTP Error [{resp.StatusCode}]: {body}");
                }

                var d = SimpleJson.Deserialize(body);
                return AbdmApiClient.Ok(new OtpTransactionResponse
                {
                    TransactionId = d.ContainsKey("txnId")   ? d["txnId"]?.ToString()   : "",
                    Message       = d.ContainsKey("message") ? d["message"]?.ToString() : "OTP sent."
                }, "Mobile update OTP sent.");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message); }
        }

        // ?? Mobile Update : Verify OTP ???????????????????????????????????????
        // POST /abha/api/v3/enrollment/auth/byAbdm
        // scope: ["abha-enrol", "mobile-verify"]

        public async Task<AbdmResponse<string>> MobileUpdateVerifyOtpAsync(
            MobileUpdateVerifyRequest request)
        {
            try
            {
                var token     = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(token);
                var encOtp    = Encrypt(request.Otp, publicKey);
                var ts        = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                AddCommonHeaders(token);
                if (!string.IsNullOrEmpty(request.UserToken))
                {
                    string cleanUserToken = request.UserToken.Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    _http.DefaultRequestHeaders.TryAddWithoutValidation(
                        "X-Token", $"Bearer {cleanUserToken}");
                }

                var payload = SimpleJson.Serialize(new Dictionary<string, object>
                {
                    ["scope"] = new[] { "abha-enrol", "mobile-verify" },
                    ["authData"] = new Dictionary<string, object>
                    {
                        ["authMethods"] = new[] { "otp" },
                        ["otp"] = new Dictionary<string, object>
                        {
                            ["timeStamp"] = ts,
                            ["txnId"]     = request.TransactionId,
                            ["otpValue"]  = encOtp
                        }
                    }
                });

                var resp = await _http.PostAsync(
                    $"{_cfg.AbhaServiceUrl}/enrollment/auth/byAbdm",
                    JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                {
                    if (body.IndexOf("Invalid X-token", StringComparison.OrdinalIgnoreCase) >= 0)
                        return AbdmApiClient.Fail<string>("Mobile Verify Error: user session token invalid hai. Dobara login/enroll karke profile reopen karein. Response: " + body);

                    return AbdmApiClient.Fail<string>($"Mobile Verify Error [{resp.StatusCode}]: {body}");
                }

                var d = SimpleJson.Deserialize(body);
                var msg = d.ContainsKey("message") ? d["message"]?.ToString() : "Mobile updated.";
                return AbdmApiClient.Ok(msg, "Mobile number updated successfully.");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<string>(ex.Message); }
        }

        // ?? Email Verification Link ?????????????????????????????????????????
        // POST /abha/api/v3/profile/account/request/emailVerificationLink

        public async Task<AbdmResponse<string>> RequestEmailVerificationLinkAsync(
            EmailVerificationLinkRequest request)
        {
            try
            {
                var token     = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(token);
                var encrypted = Encrypt((request.Email ?? "").Trim(), publicKey);

                AddCommonHeaders(token);
                if (!string.IsNullOrEmpty(request.UserToken))
                {
                    string cleanUserToken = request.UserToken.Trim();
                    if (cleanUserToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        cleanUserToken = cleanUserToken.Substring(7).Trim();
                    _http.DefaultRequestHeaders.TryAddWithoutValidation(
                        "X-Token", $"Bearer {cleanUserToken}");
                }

                string endpoint = "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account/request/emailVerificationLink";
                if (!string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                    _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    endpoint = "https://abha.abdm.gov.in/abha/api/v3/profile/account/request/emailVerificationLink";
                }

                var payload = SimpleJson.Serialize(new Dictionary<string, object>
                {
                    ["scope"]     = new[] { "abha-profile", "email-link-verify" },
                    ["loginHint"] = "email",
                    ["loginId"]   = encrypted,
                    ["otpSystem"] = "abdm"
                });

                var resp = await _http.PostAsync(endpoint, JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                {
                    if (body.IndexOf("Invalid X-token", StringComparison.OrdinalIgnoreCase) >= 0)
                        return AbdmApiClient.Fail<string>("Email Verification Error: user session token invalid hai. Dobara login/enroll karke profile reopen karein.\n" +
                            "Endpoint: " + endpoint + "\n" +
                            "Response: " + body);

                    return AbdmApiClient.Fail<string>($"Email Verification Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");
                }

                var d = SimpleJson.Deserialize(body);
                var msg = d.ContainsKey("message") ? d["message"]?.ToString() : "";
                var txnId = d.ContainsKey("txnId") ? d["txnId"]?.ToString() : "";

                if (string.IsNullOrWhiteSpace(msg) && string.IsNullOrWhiteSpace(txnId))
                    return AbdmApiClient.Fail<string>("Email verification API returned success status but no confirmation payload.\n" +
                        "Endpoint: " + endpoint + "\n" +
                        "Response: " + body);

                return AbdmApiClient.Ok(body,
                    (string.IsNullOrWhiteSpace(msg) ? "Verification link requested." : msg) +
                    "\nEndpoint: " + endpoint);
            }
            catch (Exception ex) { return AbdmApiClient.Fail<string>(ex.Message); }
        }

        public async Task<AbdmResponse<OtpTransactionResponse>> RequestPasswordChangeOtpAsync(string userToken, string password, string otpSystem)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(accessToken);
                string encryptedPassword = Encrypt((password ?? "").Trim(), publicKey);

                string endpoint = !string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                                  _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "https://abha.abdm.gov.in/abha/api/v3/profile/account/request/otp"
                    : "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account/request/otp";

                AddProfileApiHeaders(accessToken, userToken);
                var payload = SimpleJson.Serialize(new Dictionary<string, object>
                {
                    ["scope"] = new[] { "abha-profile", "change-password" },
                    ["loginHint"] = "password",
                    ["loginId"] = encryptedPassword,
                    ["otpSystem"] = (otpSystem ?? "aadhaar").Trim().ToLowerInvariant()
                });

                var resp = await _http.PostAsync(endpoint, JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                    return AbdmApiClient.Fail<OtpTransactionResponse>($"Set Password OTP Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");

                var d = SimpleJson.Deserialize(body);
                return AbdmApiClient.Ok(new OtpTransactionResponse
                {
                    TransactionId = d.ContainsKey("txnId") ? d["txnId"]?.ToString() : "",
                    Message = d.ContainsKey("message") ? d["message"]?.ToString() : "OTP sent.",
                    MaskedMobile = d.ContainsKey("mobileNumber") ? d["mobileNumber"]?.ToString()
                                : d.ContainsKey("maskedMobile") ? d["maskedMobile"]?.ToString()
                                : ""
                }, "Set password OTP sent.");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message); }
        }

        public async Task<AbdmResponse<string>> VerifyPasswordChangeOtpAsync(string userToken, string transactionId, string otp)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(accessToken);
                string encOtp = Encrypt((otp ?? "").Trim(), publicKey);

                string endpoint = !string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                                  _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "https://abha.abdm.gov.in/abha/api/v3/profile/account/verify"
                    : "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account/verify";

                AddProfileApiHeaders(accessToken, userToken);
                var payload = SimpleJson.Serialize(new Dictionary<string, object>
                {
                    ["scope"] = new[] { "abha-profile", "change-password" },
                    ["authData"] = new Dictionary<string, object>
                    {
                        ["authMethods"] = new[] { "otp" },
                        ["otp"] = new Dictionary<string, object>
                        {
                            ["txnId"] = transactionId ?? "",
                            ["otpValue"] = encOtp
                        }
                    }
                });

                var resp = await _http.PostAsync(endpoint, JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                    return AbdmApiClient.Fail<string>($"Set Password Verify Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");

                var d = SimpleJson.Deserialize(body);
                return AbdmApiClient.Ok(d.ContainsKey("message") ? d["message"]?.ToString() : "Password set successfully.", "Password verified and updated.");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<string>(ex.Message); }
        }

        public async Task<AbdmResponse<string>> ChangePasswordWithOldPasswordAsync(string userToken, string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(accessToken);

                string endpoint = !string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                                  _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "https://abha.abdm.gov.in/abha/api/v3/profile/account/verify"
                    : "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account/verify";

                AddProfileApiHeaders(accessToken, userToken);
                var passwordDict = new Dictionary<string, object>
                {
                    ["newPassword"] = Encrypt((newPassword ?? "").Trim(), publicKey),
                    ["oldPassword"] = Encrypt((oldPassword ?? "").Trim(), publicKey)
                };
                if (!string.IsNullOrWhiteSpace(confirmPassword))
                    passwordDict["confirmPassword"] = Encrypt(confirmPassword.Trim(), publicKey);

                var payload = SimpleJson.Serialize(new Dictionary<string, object>
                {
                    ["scope"] = new[] { "abha-profile", "change-password" },
                    ["authData"] = new Dictionary<string, object>
                    {
                        ["authMethods"] = new[] { "password" },
                        ["password"] = passwordDict
                    }
                });

                var resp = await _http.PostAsync(endpoint, JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                    return AbdmApiClient.Fail<string>($"Change Password Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");

                var d = SimpleJson.Deserialize(body);
                return AbdmApiClient.Ok(d.ContainsKey("message") ? d["message"]?.ToString() : "Password updated successfully.", "Password updated successfully.");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<string>(ex.Message); }
        }

        public async Task<AbdmResponse<OtpTransactionResponse>> RequestReKycOtpAsync(string userToken, string abhaNumber, string abhaAddress = null)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(accessToken);

                string cleanAbhaNumber = (abhaNumber ?? "").Replace("-", "").Replace(" ", "").Trim();
                string rawAbhaNumber = (abhaNumber ?? "").Trim();
                string cleanAbhaAddress = (abhaAddress ?? "").Trim();

                string endpoint = "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account/request/otp";
                if (!string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                    _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    endpoint = "https://abha.abdm.gov.in/abha/api/v3/profile/account/request/otp";
                }

                var attempts = new List<Dictionary<string, object>>();
                if (!string.IsNullOrWhiteSpace(cleanAbhaNumber))
                {
                    attempts.Add(new Dictionary<string, object>
                    {
                        ["scope"] = new[] { "abha-profile", "re-kyc" },
                        ["loginHint"] = "abha-number",
                        ["loginId"] = Encrypt(cleanAbhaNumber, publicKey),
                        ["otpSystem"] = "aadhaar"
                    });
                }
                if (!string.IsNullOrWhiteSpace(rawAbhaNumber) && !string.Equals(rawAbhaNumber, cleanAbhaNumber, StringComparison.Ordinal))
                {
                    attempts.Add(new Dictionary<string, object>
                    {
                        ["scope"] = new[] { "abha-profile", "re-kyc" },
                        ["loginHint"] = "abha-number",
                        ["loginId"] = Encrypt(rawAbhaNumber, publicKey),
                        ["otpSystem"] = "aadhaar"
                    });
                }
                if (!string.IsNullOrWhiteSpace(cleanAbhaAddress))
                {
                    attempts.Add(new Dictionary<string, object>
                    {
                        ["scope"] = new[] { "abha-profile", "re-kyc" },
                        ["loginHint"] = "abha-address",
                        ["loginId"] = Encrypt(cleanAbhaAddress, publicKey),
                        ["otpSystem"] = "aadhaar"
                    });
                }

                HttpResponseMessage resp = null;
                string body = "";
                foreach (var attempt in attempts)
                {
                    AddProfileApiHeaders(accessToken, userToken);
                    var payload = SimpleJson.Serialize(attempt);
                    resp = await _http.PostAsync(endpoint, JsonContent(payload));
                    body = await resp.Content.ReadAsStringAsync();
                    LastRawResponse = body;

                    if (resp.IsSuccessStatusCode)
                        break;

                    bool tryNext = body.IndexOf("Invalid LoginId", StringComparison.OrdinalIgnoreCase) >= 0
                                || body.IndexOf("Invalid LoginHint", StringComparison.OrdinalIgnoreCase) >= 0
                                || body.IndexOf("Access Denied", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!tryNext)
                        break;
                }

                if (!resp.IsSuccessStatusCode)
                {
                    if (body.IndexOf("invalid x-token", StringComparison.OrdinalIgnoreCase) >= 0)
                        return AbdmApiClient.Fail<OtpTransactionResponse>("Re-KYC session invalid hai. Dobara login karein.");

                    if (body.IndexOf("Missing Credentials", StringComparison.OrdinalIgnoreCase) >= 0
                        || body.IndexOf("Invalid Credentials", StringComparison.OrdinalIgnoreCase) >= 0)
                        return AbdmApiClient.Fail<OtpTransactionResponse>("Re-KYC access token issue. Pehle settings/token verify karein, phir dobara try karein.\nEndpoint: " + endpoint + "\nResponse: " + body);

                    return AbdmApiClient.Fail<OtpTransactionResponse>($"Re-KYC OTP Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");
                }

                var d = SimpleJson.Deserialize(body);
                return AbdmApiClient.Ok(new OtpTransactionResponse
                {
                    TransactionId = d.ContainsKey("txnId") ? d["txnId"]?.ToString() : "",
                    Message = d.ContainsKey("message") ? d["message"]?.ToString() : "OTP sent.",
                    MaskedMobile = d.ContainsKey("mobileNumber") ? d["mobileNumber"]?.ToString()
                                : d.ContainsKey("maskedMobile") ? d["maskedMobile"]?.ToString()
                                : ""
                }, "Re-KYC OTP sent.");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<OtpTransactionResponse>(ex.Message); }
        }

        public async Task<AbdmResponse<string>> VerifyReKycOtpAsync(string userToken, string transactionId, string otp)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var publicKey = await GetPublicKeyAsync(accessToken);
                var encOtp = Encrypt((otp ?? "").Trim(), publicKey);

                string endpoint = !string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                                  _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0
                    ? "https://abha.abdm.gov.in/abha/api/v3/profile/account/verify"
                    : "https://abhasbx.abdm.gov.in/abha/api/v3/profile/account/verify";

                var payload = SimpleJson.Serialize(new Dictionary<string, object>
                {
                    ["scope"] = new[] { "abha-profile", "re-kyc" },
                    ["authData"] = new Dictionary<string, object>
                    {
                        ["authMethods"] = new[] { "otp" },
                        ["otp"] = new Dictionary<string, object>
                        {
                            ["txnId"] = transactionId,
                            ["otpValue"] = encOtp
                        }
                    }
                });

                AddProfileApiHeaders(accessToken, userToken);

                var resp = await _http.PostAsync(endpoint, JsonContent(payload));
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (resp.IsSuccessStatusCode)
                {
                    var d = SimpleJson.Deserialize(body);
                    var msg = d.ContainsKey("message") ? d["message"]?.ToString() : "Re-KYC verified successfully.";
                    return AbdmApiClient.Ok(msg, "Re-KYC verified.\nEndpoint: " + endpoint);
                }

                if (body.IndexOf("invalid x-token", StringComparison.OrdinalIgnoreCase) >= 0)
                    return AbdmApiClient.Fail<string>("Re-KYC session invalid hai. Dobara login karein.");

                if (body.IndexOf("Missing Credentials", StringComparison.OrdinalIgnoreCase) >= 0
                    || body.IndexOf("Invalid Credentials", StringComparison.OrdinalIgnoreCase) >= 0)
                    return AbdmApiClient.Fail<string>("Re-KYC access token issue. Pehle settings/token verify karein, phir dobara try karein.\nEndpoint: " + endpoint + "\nResponse: " + body);

                return AbdmApiClient.Fail<string>($"Re-KYC Verify Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<string>(ex.Message); }
        }

        // ?? ABHA Address Suggestion ????????????????????????????????????????
        // GET /abha/api/v3/enrollment/enrol/suggestion

        public async Task<AbdmResponse<List<string>>> GetAbhaAddressSuggestionsAsync(string transactionId = null)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                AddCommonHeaders(token);

                _http.DefaultRequestHeaders.TryAddWithoutValidation("Transaction_Id", string.IsNullOrWhiteSpace(transactionId) ? Guid.NewGuid().ToString() : transactionId);

                string endpoint = "https://abhasbx.abdm.gov.in/abha/api/v3/enrollment/enrol/suggestion";
                if (!string.IsNullOrWhiteSpace(_cfg.AbhaServiceUrl) &&
                    _cfg.AbhaServiceUrl.IndexOf("abha.abdm.gov.in", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    endpoint = "https://abha.abdm.gov.in/abha/api/v3/enrollment/enrol/suggestion";
                }

                var resp = await _http.GetAsync(endpoint);
                var body = await resp.Content.ReadAsStringAsync();
                LastRawResponse = body;

                if (!resp.IsSuccessStatusCode)
                    return AbdmApiClient.Fail<List<string>>($"Suggestion Error [{resp.StatusCode}]\nEndpoint: {endpoint}\nResponse: {body}");

                var suggestions = new List<string>();
                var root = SimpleJson.Deserialize(body);

                foreach (var key in new[] { "suggestions", "abhaAddressSuggestions", "abhaAddressSuggestion", "data" })
                {
                    if (root.ContainsKey(key) && root[key] is List<object>)
                    {
                        var arr = root[key] as List<object>;
                        if (arr != null)
                        {
                            foreach (var item in arr)
                            {
                                var val = item == null ? "" : item.ToString();
                                if (!string.IsNullOrWhiteSpace(val) && !suggestions.Contains(val))
                                    suggestions.Add(val);
                            }
                        }
                    }
                }

                if (suggestions.Count == 0)
                {
                    foreach (var kv in root)
                    {
                        if (kv.Value is List<object>)
                        {
                            var arr = kv.Value as List<object>;
                            if (arr != null)
                            {
                                foreach (var item in arr)
                                {
                                    var val = item == null ? "" : item.ToString();
                                    if (!string.IsNullOrWhiteSpace(val) && !suggestions.Contains(val))
                                        suggestions.Add(val);
                                }
                            }
                        }
                    }
                }

                return AbdmApiClient.Ok(suggestions, $"Suggestions fetched successfully.\nEndpoint: {endpoint}");
            }
            catch (Exception ex) { return AbdmApiClient.Fail<List<string>>(ex.Message); }
        }

        // ?? Parse ABHA Profile from raw JSON ???????????????????????????????
        private AbhaProfile ParseAbhaProfile(string json)
        {
            var profile = new AbhaProfile();
            try
            {
                var root = SimpleJson.Deserialize(json);
                Dictionary<string, object> ap = null;

                if (root.ContainsKey("ABHAProfile") && root["ABHAProfile"] is Dictionary<string, object>)
                {
                    ap = root["ABHAProfile"] as Dictionary<string, object>;
                }
                else if (root.ContainsKey("firstName") || root.ContainsKey("name") || root.ContainsKey("ABHANumber"))
                {
                    // It's a flat structure (direct account response)
                    ap = root;
                }

                if (ap != null)
                {
                    
                    // Build full name
                    profile.FirstName  = Get(ap, "firstName");
                    profile.MiddleName = Get(ap, "middleName");
                    profile.LastName   = Get(ap, "lastName");

                    var nameParts = new List<string>();
                    foreach (var part in new[] { profile.FirstName, profile.MiddleName, profile.LastName })
                        if (!string.IsNullOrWhiteSpace(part)) nameParts.Add(part.Trim());
                    profile.Name = string.Join(" ", nameParts);
                    if (string.IsNullOrEmpty(profile.Name)) profile.Name = Get(ap, "name");

                    profile.Gender         = Get(ap, "gender");
                    profile.Mobile         = Get(ap, "mobile");
                    profile.Email          = GetFirst(ap, "email", "emailId", "emailAddress");
                    profile.HealthIdNumber = Get(ap, "ABHANumber");
                    if (string.IsNullOrEmpty(profile.HealthIdNumber)) profile.HealthIdNumber = Get(ap, "healthIdNumber");
                    // For compatibility
                    profile.AbhaAddress = Get(ap, "preferredAbhaAddress");
                    if (string.IsNullOrEmpty(profile.AbhaAddress)) profile.AbhaAddress = Get(ap, "healthId");

                    // DOB
                    profile.DayOfBirth   = Get(ap, "dayOfBirth");
                    profile.MonthOfBirth = Get(ap, "monthOfBirth");
                    profile.YearOfBirth  = Get(ap, "yearOfBirth");

                    var dob = Get(ap, "dob");
                    if (string.IsNullOrEmpty(dob) && !string.IsNullOrEmpty(profile.YearOfBirth))
                    {
                        if (!string.IsNullOrEmpty(profile.DayOfBirth) && !string.IsNullOrEmpty(profile.MonthOfBirth))
                        {
                            dob = $"{profile.DayOfBirth}-{profile.MonthOfBirth}-{profile.YearOfBirth}";
                        }
                        else
                        {
                            dob = profile.YearOfBirth;
                        }
                    }
                    profile.Dob = dob;

                    // Address
                    var addr = Get(ap, "address");
                    if (string.IsNullOrEmpty(addr))
                    {
                        var stateName   = Get(ap, "stateName");
                        var distName    = Get(ap, "districtName");
                        var subdistName = Get(ap, "subDistrictName");
                        var village     = Get(ap, "villageName");
                        var town        = Get(ap, "townName");
                        var ward        = Get(ap, "wardName");
                        var pincode     = Get(ap, "pincode");
                        var addrParts   = new List<string>();
                        foreach (var p in new[] { ward, village, town, subdistName, distName, stateName, pincode })
                            if (!string.IsNullOrWhiteSpace(p)) addrParts.Add(p.Trim());
                        addr = string.Join(", ", addrParts);
                    }
                    profile.Address = addr;
                    profile.City = Get(ap, "districtName");
                    profile.State = Get(ap, "stateName");

                    // Photo
                    var photo = GetFirst(ap, "photo", "profilePhoto");
                    profile.Photo = photo;
                    profile.ProfilePhoto = photo;
                    profile.FatherName = GetFirst(ap, "fatherName", "guardianName", "careOf");

                    // ABHA address
                    if (ap.ContainsKey("phrAddress") && ap["phrAddress"] is List<object>)
                    {
                        var phrArr = ap["phrAddress"] as List<object>;
                        if (phrArr != null && phrArr.Count > 0)
                            profile.AbhaAddress = phrArr[0]?.ToString() ?? "";
                    }
                    if (string.IsNullOrEmpty(profile.AbhaAddress))
                        profile.AbhaAddress = Get(ap, "preferredAbhaAddress");

                    if (root.ContainsKey("isNew") && root["isNew"] is bool)
                        profile.IsNew = (bool)root["isNew"];
                }
                else if (root.ContainsKey("accounts") && root["accounts"] is List<object>)
                {
                    var accs = root["accounts"] as List<object>;
                    if (accs != null && accs.Count > 0)
                    {
                        var acc = accs[0] as Dictionary<string, object>;
                        if (acc != null)
                        {
                            profile.HealthIdNumber = Get(acc, "ABHANumber");
                            profile.Name           = Get(acc, "name");
                            profile.AbhaAddress    = Get(acc, "preferredAbhaAddress");
                            profile.Email          = GetFirst(acc, "email", "emailId", "emailAddress");
                            profile.Photo          = GetFirst(acc, "photo", "profilePhoto");
                            profile.ProfilePhoto   = profile.Photo;
                            profile.IsNew          = false;
                        }
                    }
                }

                else
                {
                    // Case 3: Root fields directly (v3)
                    PopulateProfileFromDict(profile, root);
                }

                if (string.IsNullOrEmpty(profile.TxnId))
                    profile.TxnId = Get(root, "txnId");

                if (root.ContainsKey("tokens") && root["tokens"] is Dictionary<string, object>)
                {
                    var tok = root["tokens"] as Dictionary<string, object>;
                    profile.Token        = Get(tok, "token");
                    profile.RefreshToken = Get(tok, "refreshToken");
                    if (string.IsNullOrEmpty(profile.TxnId))
                        profile.TxnId = Get(tok, "txnId");
                }
                else if (root.ContainsKey("token"))
                {
                    profile.Token        = Get(root, "token");
                    profile.RefreshToken = Get(root, "refreshToken");
                }
            }
            catch { }
            return profile;
        }

        private void PopulateProfileFromDict(AbhaProfile profile, Dictionary<string, object> dict)
        {
            // Build full name
            var fullName = Get(dict, "name");
            if (string.IsNullOrEmpty(fullName))
            {
                var nameParts = new List<string>();
                foreach (var part in new[] { Get(dict, "firstName"), Get(dict, "middleName"), Get(dict, "lastName") })
                    if (!string.IsNullOrWhiteSpace(part)) nameParts.Add(part.Trim());
                fullName = string.Join(" ", nameParts);
            }
            profile.Name = fullName;

            profile.Gender         = Get(dict, "gender");
            profile.Mobile         = Get(dict, "mobile");
            profile.Email          = GetFirst(dict, "email", "emailId", "emailAddress");
            profile.HealthIdNumber = GetFirst(dict, "ABHANumber", "healthIdNumber");

            // DOB
            var dob = Get(dict, "dob");
            if (string.IsNullOrEmpty(dob))
            {
                var d2 = Get(dict, "dayOfBirth");
                var m2 = Get(dict, "monthOfBirth");
                var y2 = Get(dict, "yearOfBirth");
                if (!string.IsNullOrEmpty(y2))
                    dob = !string.IsNullOrEmpty(d2) ? $"{d2}-{m2}-{y2}" : y2;
            }
            profile.YearOfBirth = dob;
            profile.Dob = dob;

            // Address
            var addr = Get(dict, "address");
            if (string.IsNullOrEmpty(addr))
            {
                var stateName   = Get(dict, "stateName");
                var distName    = Get(dict, "districtName");
                var subdistName = Get(dict, "subDistrictName");
                var village     = Get(dict, "villageName");
                var town        = Get(dict, "townName");
                var ward        = Get(dict, "wardName");
                var pincode     = Get(dict, "pincode");
                var addrParts   = new List<string>();
                foreach (var p in new[] { ward, village, town, subdistName, distName, stateName, pincode })
                    if (!string.IsNullOrWhiteSpace(p)) addrParts.Add(p.Trim());
                addr = string.Join(", ", addrParts);
            }
            profile.Address = addr;
            profile.City = GetFirst(dict, "districtName", "district");
            profile.State = GetFirst(dict, "stateName", "state");

            // Photo
            profile.Photo = GetFirst(dict, "photo", "profilePhoto");
            profile.ProfilePhoto = profile.Photo;
            profile.FatherName = GetFirst(dict, "fatherName", "guardianName", "careOf");

            // ABHA address
            if (dict.ContainsKey("phrAddress") && dict["phrAddress"] is List<object>)
            {
                var phrArr = dict["phrAddress"] as List<object>;
                if (phrArr != null && phrArr.Count > 0)
                    profile.AbhaAddress = phrArr[0]?.ToString() ?? "";
            }
            if (string.IsNullOrEmpty(profile.AbhaAddress))
                profile.AbhaAddress = Get(dict, "preferredAbhaAddress");
        }

        private static string Get(Dictionary<string, object> d, string key)
        {
            return d != null && d.ContainsKey(key) ? d[key]?.ToString() ?? "" : "";
        }

        private static string GetFirst(Dictionary<string, object> d, params string[] keys)
        {
            if (d == null || keys == null) return "";
            foreach (var key in keys)
            {
                var value = Get(d, key);
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
            return "";
        }

        // ==========================================
        // === MILESTONE 2 (M2) API CLIENT METHODS ===
        // ==========================================

        /// <summary>
        /// Registers patients and their care contexts in the wrapper database (HIP).
        /// </summary>
        public async Task<AbdmResponse<string>> AddPatientsToWrapperAsync(string abhaAddress, string name, string gender, string dob, string patientRef, string patientMobile, List<Dictionary<string, object>> careContexts)
        {
            try
            {
                var patient = new Dictionary<string, object>
                {
                    ["abhaAddress"] = abhaAddress,
                    ["name"] = name,
                    ["gender"] = gender,
                    ["dateOfBirth"] = dob,
                    ["patientReference"] = patientRef,
                    ["patientDisplay"] = name,
                    ["patientMobile"] = patientMobile,
                    ["hipId"] = _cfg.HipId ?? "IN0610090658",
                    ["careContexts"] = careContexts
                };

                var payload = SimpleJson.Serialize(new List<object> { patient });
                string responseBody = await PutToWrapperAsync("/v3/add-patients", payload);
                return Ok(responseBody, "Patient registered successfully in wrapper database.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Initiates HIP-initiated care context linking (HIP).
        /// </summary>
        public async Task<AbdmResponse<string>> LinkCareContextsAsync(string abhaAddress, string requesterId, List<Dictionary<string, object>> careContexts)
        {
            try
            {
                var request = new Dictionary<string, object>
                {
                    ["requestId"] = Guid.NewGuid().ToString(),
                    ["requesterId"] = requesterId,
                    ["abhaAddress"] = abhaAddress,
                    ["careContexts"] = careContexts
                };

                var payload = SimpleJson.Serialize(request);
                string responseBody = await PostToWrapperAsync("/v3/link-carecontexts", payload);
                return Ok(responseBody, "Linking initiated. Check status using request ID.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Checks the status of a care context linking request (HIP).
        /// </summary>
        public async Task<AbdmResponse<string>> GetLinkStatusAsync(string requestId)
        {
            try
            {
                string responseBody = await GetFromWrapperAsync($"/v3/link-status/{requestId}");
                return Ok(responseBody, "Fetch status succeeded.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Sends deep linking notification SMS to patient (HIP).
        /// </summary>
        public async Task<AbdmResponse<string>> SendDeepLinkingSmsAsync(string phoneNo, string hipId, string hipName)
        {
            try
            {
                var request = new Dictionary<string, object>
                {
                    ["requestId"] = Guid.NewGuid().ToString(),
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ["notification"] = new Dictionary<string, object>
                    {
                        ["phoneNo"] = phoneNo,
                        ["hip"] = new Dictionary<string, object>
                        {
                            ["id"] = hipId,
                            ["name"] = hipName
                        }
                    }
                };

                var payload = SimpleJson.Serialize(request);
                string responseBody = await PostToWrapperAsync("/v3/sms/notify", payload);
                return Ok(responseBody, "SMS notification sent.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Initiates a consent request (HIU).
        /// </summary>
        public async Task<AbdmResponse<string>> InitiateConsentRequestAsync(string patientAbhaAddress, string purposeCode, List<string> hiTypes, string dateFrom, string dateTo, string eraseAt)
        {
            try
            {
                var consent = new Dictionary<string, object>
                {
                    ["purpose"] = new Dictionary<string, object>
                    {
                        ["text"] = "Referral",
                        ["code"] = purposeCode,
                        ["refUri"] = ""
                    },
                    ["patient"] = new Dictionary<string, object>
                    {
                        ["id"] = patientAbhaAddress
                    },
                    ["hiu"] = new Dictionary<string, object>
                    {
                        ["id"] = _cfg.HipId ?? "IN0610090658",
                        ["name"] = _cfg.HipName ?? "MIDHA HOSPITAL"
                    },
                    ["requester"] = new Dictionary<string, object>
                    {
                        ["name"] = "Dr. Midha",
                        ["identifier"] = new Dictionary<string, object>
                        {
                            ["type"] = "REGNO",
                            ["value"] = "MCI-12345",
                            ["system"] = "https://mciindia.org"
                        }
                    },
                    ["hiTypes"] = hiTypes,
                    ["permission"] = new Dictionary<string, object>
                    {
                        ["accessMode"] = "VIEW",
                        ["dateRange"] = new Dictionary<string, object>
                        {
                            ["from"] = dateFrom,
                            ["to"] = dateTo
                        },
                        ["dataEraseAt"] = eraseAt,
                        ["frequency"] = new Dictionary<string, object>
                        {
                            ["unit"] = "HOUR",
                            ["value"] = 1,
                            ["repeats"] = 0
                        }
                    }
                };

                var request = new Dictionary<string, object>
                {
                    ["requestId"] = Guid.NewGuid().ToString(),
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ["consent"] = consent
                };

                var payload = SimpleJson.Serialize(request);
                string responseBody = await PostToWrapperAsync("/v3/consent-init", payload);
                return Ok(responseBody, "Consent request initiated. Check status using request ID.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Gets the status of a consent request (HIU).
        /// </summary>
        public async Task<AbdmResponse<string>> GetConsentStatusAsync(string requestId)
        {
            try
            {
                string responseBody = await GetFromWrapperAsync($"/v3/consent-status/{requestId}");
                return Ok(responseBody, "Fetch consent status succeeded.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Requests encrypted health records from HIP via ABDM gateway (HIU).
        /// </summary>
        public async Task<AbdmResponse<string>> FetchHealthInformationAsync(string consentId, string dateFrom, string dateTo)
        {
            try
            {
                var request = new Dictionary<string, object>
                {
                    ["requestId"] = Guid.NewGuid().ToString(),
                    ["consentId"] = consentId,
                    ["dateRange"] = new Dictionary<string, object>
                    {
                        ["from"] = dateFrom,
                        ["to"] = dateTo
                    }
                };

                var payload = SimpleJson.Serialize(request);
                string responseBody = await PostToWrapperAsync("/v3/health-information/fetch-records", payload);
                return Ok(responseBody, "Health information fetch request initiated.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Gets the status and decrypted FHIR bundles for a health data request (HIU).
        /// </summary>
        public async Task<AbdmResponse<string>> GetHealthInformationStatusAsync(string requestId)
        {
            try
            {
                string responseBody = await GetFromWrapperAsync($"/v3/health-information/status/{requestId}");
                return Ok(responseBody, "Fetch health records succeeded.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Initiates a health information subscription request (HIU) - M3.
        /// </summary>
        public async Task<AbdmResponse<string>> InitiateSubscriptionRequestAsync(string patientAbhaAddress, string purposeCode, List<string> categories, string dateFrom, string dateTo)
        {
            try
            {
                var subscription = new Dictionary<string, object>
                {
                    ["purpose"] = new Dictionary<string, object>
                    {
                        ["text"] = "Referral",
                        ["code"] = purposeCode,
                        ["refUri"] = ""
                    },
                    ["patient"] = new Dictionary<string, object>
                    {
                        ["id"] = patientAbhaAddress
                    },
                    ["hiu"] = new Dictionary<string, object>
                    {
                        ["id"] = _cfg.HipId ?? "IN0610090658",
                        ["name"] = _cfg.HipName ?? "MIDHA HOSPITAL"
                    },
                    ["categories"] = categories,
                    ["period"] = new Dictionary<string, object>
                    {
                        ["from"] = dateFrom,
                        ["to"] = dateTo
                    }
                };

                var request = new Dictionary<string, object>
                {
                    ["requestId"] = Guid.NewGuid().ToString(),
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ["subscription"] = subscription
                };

                var payload = SimpleJson.Serialize(request);
                string responseBody = await PostToWrapperAsync("/v3/subscription-init", payload);
                return Ok(responseBody, "Subscription request initiated. Check status using request ID.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

        /// <summary>
        /// Gets the status of a subscription request (HIU) - M3.
        /// </summary>
        public async Task<AbdmResponse<string>> GetSubscriptionStatusAsync(string requestId)
        {
            try
            {
                string responseBody = await GetFromWrapperAsync($"/v3/subscription-status/{requestId}");
                return Ok(responseBody, "Fetch subscription status succeeded.");
            }
            catch (Exception ex)
            {
                return Fail<string>(ex.Message);
            }
        }

    }

    // ??? Minimal JSON Serializer/Deserializer (no external libs) ?????????????
    internal static class SimpleJson
    {
        // ?? Serialize ?????????????????????????????????????????????????????????
        public static string Serialize(object obj)
        {
            if (obj == null) return "null";
            if (obj is bool) return (bool)obj ? "true" : "false";
            if (obj is string) return EscapeString((string)obj);
            if (obj is int || obj is long || obj is double || obj is float || obj is decimal)
                return obj.ToString();
            if (obj is DateTime) return EscapeString(((DateTime)obj).ToString("o"));
            
            if (obj is Dictionary<string, object>) return SerializeDict((Dictionary<string, object>)obj);

            if (obj is System.Collections.IEnumerable && !(obj is string))
            {
                var sb = new StringBuilder("[");
                bool first = true;
                foreach (var item in (System.Collections.IEnumerable)obj)
                {
                    if (!first) sb.Append(",");
                    sb.Append(Serialize(item));
                    first = false;
                }
                sb.Append("]");
                return sb.ToString();
            }

            var type = obj.GetType();
            if (type.IsClass)
            {
                var sb = new StringBuilder("{");
                var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                bool first = true;
                foreach (var prop in props)
                {
                    var val = prop.GetValue(obj);
                    if (val == null) continue;
                    if (!first) sb.Append(",");
                    sb.Append(EscapeString(prop.Name));
                    sb.Append(":");
                    sb.Append(Serialize(val));
                    first = false;
                }
                sb.Append("}");
                return sb.ToString();
            }
            
            return EscapeString(obj.ToString());
        }

        private static string EscapeString(string s) =>
            "\"" + s.Replace("\\","\\\\").Replace("\"","\\\"")
                    .Replace("\n","\\n").Replace("\r","\\r").Replace("\t","\\t") + "\"";

        private static string SerializeStringArray(IEnumerable<string> arr)
        {
            var sb = new StringBuilder("[");
            bool first = true;
            foreach (var item in arr)
            {
                if (!first) sb.Append(",");
                sb.Append(EscapeString(item ?? ""));
                first = false;
            }
            return sb.Append("]").ToString();
        }

        private static string SerializeArray(object[] arr)
        {
            var sb = new StringBuilder("[");
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(Serialize(arr[i]));
            }
            return sb.Append("]").ToString();
        }

        private static string SerializeList(List<object> list)
        {
            var sb = new StringBuilder("[");
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(Serialize(list[i]));
            }
            return sb.Append("]").ToString();
        }

        private static string SerializeDict(Dictionary<string, object> d)
        {
            var sb = new StringBuilder("{");
            bool first = true;
            foreach (var kv in d)
            {
                if (!first) sb.Append(",");
                sb.Append(EscapeString(kv.Key));
                sb.Append(":");
                sb.Append(Serialize(kv.Value));
                first = false;
            }
            return sb.Append("}").ToString();
        }

        // ?? Deserialize ???????????????????????????????????????????????????????
        public static Dictionary<string, object> Deserialize(string json)
        {
            int pos = 0;
            SkipWs(json, ref pos);
            var result = ParseObject(json, ref pos);
            return result as Dictionary<string, object>
                   ?? new Dictionary<string, object>();
        }

        private static object ParseValue(string json, ref int pos)
        {
            SkipWs(json, ref pos);
            if (pos >= json.Length) return null;
            char c = json[pos];
            if (c == '"')  return ParseString(json, ref pos);
            if (c == '{')  return ParseObject(json, ref pos);
            if (c == '[')  return ParseArray(json, ref pos);
            if (c == 't')  { pos += 4; return true; }
            if (c == 'f')  { pos += 5; return false; }
            if (c == 'n')  { pos += 4; return null; }
            return ParseNumber(json, ref pos);
        }

        private static object ParseObject(string json, ref int pos)
        {
            var dict = new Dictionary<string, object>();
            pos++; // skip '{'
            SkipWs(json, ref pos);
            if (pos < json.Length && json[pos] == '}') { pos++; return dict; }
            while (pos < json.Length)
            {
                SkipWs(json, ref pos);
                var key = ParseString(json, ref pos);
                SkipWs(json, ref pos);
                pos++; // skip ':'
                SkipWs(json, ref pos);
                dict[key] = ParseValue(json, ref pos);
                SkipWs(json, ref pos);
                if (pos < json.Length && json[pos] == ',') { pos++; continue; }
                break;
            }
            if (pos < json.Length) pos++; // skip '}'
            return dict;
        }

        private static object ParseArray(string json, ref int pos)
        {
            var list = new List<object>();
            pos++; // skip '['
            SkipWs(json, ref pos);
            if (pos < json.Length && json[pos] == ']') { pos++; return list; }
            while (pos < json.Length)
            {
                SkipWs(json, ref pos);
                list.Add(ParseValue(json, ref pos));
                SkipWs(json, ref pos);
                if (pos < json.Length && json[pos] == ',') { pos++; continue; }
                break;
            }
            if (pos < json.Length) pos++; // skip ']'
            return list;
        }

        private static string ParseString(string json, ref int pos)
        {
            pos++; // skip '"'
            var sb = new StringBuilder();
            while (pos < json.Length)
            {
                char c = json[pos++];
                if (c == '"') break;
                if (c == '\\' && pos < json.Length)
                {
                    char e = json[pos++];
                    switch (e)
                    {
                        case '"':  sb.Append('"');  break;
                        case '\\': sb.Append('\\'); break;
                        case 'n':  sb.Append('\n'); break;
                        case 'r':  sb.Append('\r'); break;
                        case 't':  sb.Append('\t'); break;
                        default:   sb.Append(e);    break;
                    }
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static object ParseNumber(string json, ref int pos)
        {
            int start = pos;
            while (pos < json.Length && "0123456789.-+eE".IndexOf(json[pos]) >= 0) pos++;
            string num = json.Substring(start, pos - start);
            
            long l;
            if (long.TryParse(num, out l)) return l;
            
            double d;
            if (double.TryParse(num, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out d)) return d;
            
            return num;
        }

        private static void SkipWs(string json, ref int pos)
        {
            while (pos < json.Length && (json[pos] == ' ' || json[pos] == '\t' ||
                                          json[pos] == '\r' || json[pos] == '\n'))
                pos++;
        }
    }
}
