using System.Net;
using System.Net.Sockets;
using UAParser;

namespace UserManagement.Common.RequestFilters
{
    public interface IUserAgentService
    {
        UserAgentInfoModel GetUserAgentInfo(HttpContext context);
        string GetPreferredLanguage(string acceptLanguages);
        string GetSessionData(ISession session);
    }

    public class UserAgentService : IUserAgentService
    {
        public UserAgentInfoModel GetUserAgentInfo(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            UserAgentInfoModel model = new UserAgentInfoModel
            {
                UserAgent = context.Request.Headers["User-Agent"].ToString()
            };

            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(model.UserAgent);

            model.BrowserName = clientInfo.UA.Family;
            model.BrowserVersion = $"{clientInfo.UA.Major}.{clientInfo.UA.Minor}";
            model.OperatingSystem = $"{clientInfo.OS.Family} {clientInfo.OS.Major}.{clientInfo.OS.Minor}";
            model.DeviceType = clientInfo.Device.Family.ToLowerInvariant().Contains("mobile") || clientInfo.Device.Family.ToLowerInvariant().Contains("tablet") ? "Mobile" : "Desktop";

            // Get IP address
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (!IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress) || parsedIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                ipAddress = "127.0.0.1"; // Return loopback address for invalid or IPv6 addresses
            }
            model.IPAddress = ipAddress;

            // Get session ID
            model.SessionID = context.Session.Id;

            // Additional details
            model.Referrer = context.Request.Headers["Referer"].ToString();
            model.IsHttps = context.Request.IsHttps;
            model.Protocol = model.IsHttps ? "https" : "http";

            var languages = context.Request.Headers["Accept-Language"].ToString();
            model.Languages = GetPreferredLanguage(languages);

            model.SessionData = GetSessionData(context.Session);

            return model;
        }

        public string GetPreferredLanguage(string acceptLanguages)
        {
            if (string.IsNullOrEmpty(acceptLanguages))
            {
                return "en"; // Default language if not specified
            }

            var languages = acceptLanguages.Split(',');
            foreach (var language in languages)
            {
                var parts = language.Split(';');
                var lang = parts[0]?.Trim();
                if (!string.IsNullOrEmpty(lang))
                {
                    return lang;
                }
            }

            return "en";
        }

        public string GetSessionData(ISession session)
        {
            var sessionVariables = session.Keys
                .Select(key => new KeyValuePair<string, string>(key, session.GetString(key)))
                .Where(pair => pair.Value != null)
                .Select(pair => $"{pair.Key}:{pair.Value}");

            return string.Join(" ", sessionVariables);
        }
    }

    public class UserAgentInfoModel
    {
        public string UserAgent { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string BrowserName { get; set; } = string.Empty;
        public string BrowserVersion { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string SessionID { get; set; } = string.Empty;
        public string Referrer { get; set; } = string.Empty;
        public bool IsHttps { get; set; } = false;
        public string Protocol { get; set; } = string.Empty;
        public string Languages { get; set; } = string.Empty;
        public string PreferredLanguage { get; set; } = string.Empty;
        public string SessionData { get; set; } = string.Empty;

    }
}

