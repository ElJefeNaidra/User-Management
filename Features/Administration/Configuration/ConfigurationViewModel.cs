using SIDPSF.Features.SharedMVC;
using static SIDPSF.Features.Administration.ConfigurationProvider;

namespace SIDPSF.Features.Administration.Configuration
{
    public class ConfigurationViewModel : BaseCrudModel
    {
        public DatabaseConfig? DatabaseConfig { get; set; }
        public SystemPathsConfig? SystemPathsConfig { get; set; }
        public SSRSConfig? SSRSConfig { get; set; }
        public AuthenticationConfig? AuthenticationConfig { get; set; }
        public List<LogEntry> DatabaseLogs { get; set; } = new List<LogEntry>();
        public List<LogEntry> SSRSErrorLogs { get; set; } = new List<LogEntry>();
        public List<LogEntry> ApplicationErrorLogs { get; set; } = new List<LogEntry>();
        public List<LogEntry> DbContextErrorLogs { get; set; } = new List<LogEntry>();


        public List<ServiceConfig>? ServiceConfigs { get; set; }

        public Dictionary<string, List<LogEntry>> ServiceLogEntries { get; set; } = new Dictionary<string, List<LogEntry>>();

        public class LogEntry
        {
            public DateTime LogDateTime { get; set; }
            public string? LogMessage { get; set; }
        }
    }
}