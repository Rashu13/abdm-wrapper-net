using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ABDM.Models
{
    // --- Request Models -----------------------------------------------------------

    public class AbdmGenerateOtpRequest
    {
        public string LoginId   { get; set; }   // Mobile (10 digits) or Aadhaar (12 digits)
        public string LoginType { get; set; }   // "MOBILE" | "AADHAAR"

        // Nudge Consent Logging details
        public bool? Chk1 { get; set; }
        public bool? Chk2 { get; set; }
        public bool? Chk3 { get; set; }
        public bool? Chk4 { get; set; }
        public bool? Chk5 { get; set; }
        public bool? Chk6 { get; set; }
        public bool? Chk7 { get; set; }
        public string? OperatorName { get; set; }
        public string? BeneficiaryName { get; set; }
        public DateTime? ConsentTimestamp { get; set; }
    }

    public class AbdmVerifyOtpRequest
    {
        public string Otp           { get; set; }
        public string TransactionId { get; set; }
        public string? LoginType     { get; set; }   // "MOBILE" | "AADHAAR"
        public string? Mobile        { get; set; }   // required for Aadhaar flow
    }

    public class AbdmCreateAbhaAddressRequest
    {
        public string TransactionId { get; set; }
        public string AbhaAddress   { get; set; }
    }

    public class MobileUpdateOtpRequest
    {
        public string TransactionId { get; set; }
        public string Mobile    { get; set; }   // New 10-digit mobile number
        public string? UserToken { get; set; }   // X-Token (user's bearer token)
    }

    public class MobileUpdateVerifyRequest
    {
        public string Otp           { get; set; }
        public string TransactionId { get; set; }
        public string? UserToken     { get; set; }   // X-Token
    }

    public class EmailVerificationLinkRequest
    {
        public string Email     { get; set; }
        public string? UserToken { get; set; }
    }

    // --- Response Models ----------------------------------------------------------

    public class AbdmResponse<T>
    {
        public bool   Success { get; set; }
        public string Message { get; set; }
        public T      Data    { get; set; }
    }

    public class OtpTransactionResponse
    {
        public string TransactionId { get; set; }
        public string Message       { get; set; }
        public string MaskedMobile  { get; set; }   // e.g. ******6682
    }

    public class AbhaProfile
    {
        public string HealthIdNumber { get; set; }
        public string AbhaAddress    { get; set; }
        public string Name           { get; set; }
        public string FirstName      { get; set; }
        public string MiddleName     { get; set; }
        public string LastName       { get; set; }
        public string Mobile         { get; set; }
        public string Email          { get; set; }
        public string Gender         { get; set; }
        public string YearOfBirth    { get; set; }
        public string MonthOfBirth   { get; set; }
        public string DayOfBirth     { get; set; }
        public string Dob            { get; set; }
        public string Address        { get; set; }
        public string City           { get; set; }
        public string State          { get; set; }
        public string Photo          { get; set; }
        public string ProfilePhoto   { get; set; }
        public string FatherName     { get; set; }
        public string Token          { get; set; }
        public string RefreshToken   { get; set; }
        public string TxnId          { get; set; }
        public bool   IsNew          { get; set; }
        public string AbhaNumber { get; internal set; }
        public string AadharNo       { get; set; }

        public SavedSession ToSession(string token = null)
        {
            return new SavedSession
            {
                UserToken = token ?? this.Token ?? "",
                UserName = this.Name ?? "",
                FirstName = this.FirstName ?? "",
                MiddleName = this.MiddleName ?? "",
                LastName = this.LastName ?? "",
                HealthIdNumber = this.HealthIdNumber ?? "",
                AbhaAddress = this.AbhaAddress ?? "",
                Mobile = this.Mobile ?? "",
                Email = this.Email ?? "",
                Gender = this.Gender ?? "",
                YearOfBirth = this.YearOfBirth ?? "",
                MonthOfBirth = this.MonthOfBirth ?? "",
                DayOfBirth = this.DayOfBirth ?? "",
                Dob = this.Dob ?? "",
                Address = this.Address ?? "",
                Photo = this.Photo ?? "",
                ProfilePhoto = this.ProfilePhoto ?? "",
                TxnId = this.TxnId ?? "",
                SavedAtUtc = DateTime.UtcNow,
                AadharNo = this.AadharNo ?? "",
                City = this.City ?? "",
                State = this.State ?? "",
                FatherName = this.FatherName ?? ""
            };
        }
    }

    public class AbhaSuggestionResponse
    {
        public string TransactionId { get; set; }
        public List<string> AbhaAddressList { get; set; } = new List<string>();
    }

    public class CreateAbhaAddressResponse
    {
        public string AbhaAddress { get; set; }
        public string HealthIdNumber { get; set; }
        public string Status { get; set; }
    }

    public class AbdmLoginResponse
    {
        public bool              IsNew        { get; set; }
        public string            Token        { get; set; }
        public string            RefreshToken { get; set; }
        public List<AbhaProfile> Accounts     { get; set; } = new List<AbhaProfile>();
    }

    public class AbhaCardResponse
    {
        public string ContentType { get; set; }
        public string Content     { get; set; }  // Base64 PNG
    }

    // --- App Settings ---

    public class AbdmSettings
    {
        public string BaseUrl        { get; set; }
        public string AbhaServiceUrl { get; set; }
        public string ClientId       { get; set; }
        public string ClientSecret   { get; set; }
        public string HipId          { get; set; }
        public string HipName        { get; set; }
        public string CmId           { get; set; } = "sbx";
        public string Environment    { get; set; } = "Sandbox";
    }

    public class SavedSession
    {
        public string UserToken { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string HealthIdNumber { get; set; }
        public string AbhaAddress { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string YearOfBirth { get; set; }
        public string MonthOfBirth { get; set; }
        public string DayOfBirth { get; set; }
        public string Dob { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public string ProfilePhoto { get; set; }
        public string TxnId { get; set; }
        public DateTime SavedAtUtc { get; set; }
        public string AadharNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string FatherName { get; set; }

        public AbhaProfile ToProfile()
        {
            return new AbhaProfile
            {
                Name = UserName,
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                HealthIdNumber = HealthIdNumber,
                AbhaAddress = AbhaAddress,
                Mobile = Mobile,
                Email = Email,
                Gender = Gender,
                YearOfBirth = YearOfBirth,
                MonthOfBirth = MonthOfBirth,
                DayOfBirth = DayOfBirth,
                Dob = Dob,
                Address = Address,
                Photo = Photo,
                ProfilePhoto = ProfilePhoto,
                TxnId = TxnId,
                Token = UserToken,
                AadharNo = AadharNo,
                City = City,
                State = State,
                FatherName = FatherName
            };
        }
    }

    public static class SessionStore
    {
        private static string SessionFilePath
        {
            get
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ABDM");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return Path.Combine(dir, "session.dat");
            }
        }

        public static SavedSession Create(AbdmSettings settings, string userToken, AbhaProfile profile)
        {
            profile = profile ?? new AbhaProfile();
            return new SavedSession
            {
                UserToken = userToken ?? "",
                UserName = profile.Name ?? "",
                FirstName = profile.FirstName ?? "",
                MiddleName = profile.MiddleName ?? "",
                LastName = profile.LastName ?? "",
                HealthIdNumber = profile.HealthIdNumber ?? "",
                AbhaAddress = profile.AbhaAddress ?? "",
                Mobile = profile.Mobile ?? "",
                Email = profile.Email ?? "",
                Gender = profile.Gender ?? "",
                YearOfBirth = profile.YearOfBirth ?? "",
                MonthOfBirth = profile.MonthOfBirth ?? "",
                DayOfBirth = profile.DayOfBirth ?? "",
                Dob = profile.Dob ?? "",
                Address = profile.Address ?? "",
                Photo = profile.Photo ?? "",
                ProfilePhoto = profile.ProfilePhoto ?? "",
                TxnId = profile.TxnId ?? "",
                SavedAtUtc = DateTime.UtcNow,
                AadharNo = profile.AadharNo ?? "",
                City = profile.City ?? "",
                State = profile.State ?? "",
                FatherName = profile.FatherName ?? ""
            };
        }

        public static void Save(SavedSession session)
        {
            if (session == null || string.IsNullOrWhiteSpace(session.UserToken)) return;

            string plain = string.Join("\n", new[]
            {
                ToLine(session.UserToken),
                ToLine(session.UserName),
                ToLine(session.FirstName),
                ToLine(session.MiddleName),
                ToLine(session.LastName),
                ToLine(session.HealthIdNumber),
                ToLine(session.AbhaAddress),
                ToLine(session.Mobile),
                ToLine(session.Email),
                ToLine(session.Gender),
                ToLine(session.YearOfBirth),
                ToLine(session.MonthOfBirth),
                ToLine(session.DayOfBirth),
                ToLine(session.Dob),
                ToLine(session.Address),
                ToLine(session.Photo),
                ToLine(session.ProfilePhoto),
                ToLine(session.TxnId),
                ToLine(session.SavedAtUtc.ToString("o")),
                ToLine(session.AadharNo ?? ""),
                ToLine(session.City ?? ""),
                ToLine(session.State ?? ""),
                ToLine(session.FatherName ?? "")
            });

            File.WriteAllText(SessionFilePath, Convert.ToBase64String(Encoding.UTF8.GetBytes(plain)), Encoding.UTF8);
        }

        public static SavedSession Load()
        {
            try
            {
                if (!File.Exists(SessionFilePath)) return null;

                byte[] plainBytes = Convert.FromBase64String(File.ReadAllText(SessionFilePath, Encoding.UTF8));
                string[] lines = Encoding.UTF8.GetString(plainBytes).Split(new[] { "\n" }, StringSplitOptions.None);
                if (lines.Length < 19) return null;

                DateTime savedAtUtc;
                DateTime.TryParse(FromLine(lines[18]), null, System.Globalization.DateTimeStyles.RoundtripKind, out savedAtUtc);

                return new SavedSession
                {
                    UserToken = FromLine(lines[0]),
                    UserName = FromLine(lines[1]),
                    FirstName = FromLine(lines[2]),
                    MiddleName = FromLine(lines[3]),
                    LastName = FromLine(lines[4]),
                    HealthIdNumber = FromLine(lines[5]),
                    AbhaAddress = FromLine(lines[6]),
                    Mobile = FromLine(lines[7]),
                    Email = FromLine(lines[8]),
                    Gender = FromLine(lines[9]),
                    YearOfBirth = FromLine(lines[10]),
                    MonthOfBirth = FromLine(lines[11]),
                    DayOfBirth = FromLine(lines[12]),
                    Dob = FromLine(lines[13]),
                    Address = FromLine(lines[14]),
                    Photo = FromLine(lines[15]),
                    ProfilePhoto = FromLine(lines[16]),
                    TxnId = FromLine(lines[17]),
                    SavedAtUtc = savedAtUtc == DateTime.MinValue ? DateTime.UtcNow : savedAtUtc,
                    AadharNo = lines.Length >= 20 ? FromLine(lines[19]) : "",
                    City = lines.Length >= 21 ? FromLine(lines[20]) : "",
                    State = lines.Length >= 22 ? FromLine(lines[21]) : "",
                    FatherName = lines.Length >= 23 ? FromLine(lines[22]) : ""
                };
            }
            catch
            {
                return null;
            }
        }

        public static void Clear()
        {
            if (File.Exists(SessionFilePath))
                File.Delete(SessionFilePath);
        }

        private static string ToLine(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? ""));
        }

        private static string FromLine(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            try { return Encoding.UTF8.GetString(Convert.FromBase64String(value)); } catch { return ""; }
        }
    }
}
