using Microsoft.Data.SqlClient;
using UserManagement.Common.StringLocalisation;
using UserManagement.Features.Administration.Configuration;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UserManagement.Features.Administration
{
    public class ConfigurationProvider
    {
        public class SystemConfigService
        {
            public List<ServiceConfig> ServiceConfigs { get; } = new List<ServiceConfig>();
            public DatabaseConfig DatabaseConfig { get; }
            public SystemPathsConfig SystemPathsConfig { get; }
            public SSRSConfig SSRSConfig { get; }
            public AuthenticationConfig AuthenticationConfig { get; }

            private readonly string _connectionString;

            public SystemConfigService(IConfiguration configuration, string connectionString)
            {
                DatabaseConfig = new DatabaseConfig();
                SystemPathsConfig = new SystemPathsConfig();
                SSRSConfig = new SSRSConfig();
                AuthenticationConfig = new AuthenticationConfig();
                _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

                configuration.GetSection("DatabaseConfig").Bind(DatabaseConfig);
                configuration.GetSection("SystemPathsConfig").Bind(SystemPathsConfig);
                configuration.GetSection("SSRSConfig").Bind(SSRSConfig);
                configuration.GetSection("AuthenticationConfig").Bind(AuthenticationConfig);
                configuration.GetSection("ServiceConfigs").Bind(ServiceConfigs);
            }

            public void EnsureDirectoryExists(string path)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    DirectoryInfo directoryInfo = null;

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(path))
                    {
                        directoryInfo = Directory.CreateDirectory(path);
                    }
                    else
                    {
                        directoryInfo = new DirectoryInfo(path);
                    }

                    // Get the current access control settings for the directory
                    DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

                    // Define the "ALL APPLICATION PACKAGES" security identifier
                    SecurityIdentifier allApplicationPackagesSid = new SecurityIdentifier("S-1-15-2-1");

                    // Define a new full control access rule for "ALL APPLICATION PACKAGES"
                    FileSystemAccessRule accessRule = new FileSystemAccessRule(
                        allApplicationPackagesSid,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, // Apply to all subfolders and files
                        PropagationFlags.None,
                        AccessControlType.Allow);

                    // Check if the access rule is not already present
                    bool ruleExists = false;
                    foreach (FileSystemAccessRule rule in directorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
                    {
                        if (rule.IdentityReference == allApplicationPackagesSid && rule.FileSystemRights == FileSystemRights.FullControl)
                        {
                            ruleExists = true;
                            break;
                        }
                    }

                    // Add the new rule and set the updated access control settings if the rule does not exist
                    if (!ruleExists)
                    {
                        directorySecurity.AddAccessRule(accessRule);
                        directoryInfo.SetAccessControl(directorySecurity);
                    }
                }
            }
            public bool CheckDatabaseExists()
            {
                try
                {
                    // Retrieve the connection string from the configuration
                    string connectionString = _connectionString;

                    // Attempt to open a connection to the database
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();  // If this succeeds, the database exists and can be logged into

                        // Optionally, you can execute a simple query like "SELECT 1" to further verify the connection
                        using (var command = new SqlCommand("SELECT 1", connection))
                        {
                            command.ExecuteScalar();
                        }
                    }

                    return true;  // The database exists and is accessible
                }
                catch (Exception ex)
                {
                    // Log the exception details
                    LogToFile($"Critical Error: Could not connect to the database. Exception: {ex.Message}", SystemPathsConfig.PathToConfigApplicationErrorLogs, "Database_Log.txt");

                    return false;  // The database does not exist or is not accessible
                }
            }

            public void CheckSsrsServerAvailability()
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(SSRSConfig.SSRSUrl);
                    request.Timeout = 5000;
                    request.Method = "HEAD";

                    using var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        LogToFile("Warning: SSRS server is not available.", SystemPathsConfig.PathToConfigSSRSErrorLogs, "SSRS_Log.txt");
                    }
                    else
                    {
                        using var connection = new SqlConnection(_connectionString);
                        connection.Open();

                        // Check if ReportServer db exists
                        if (!DatabaseExists(connection, "ReportServer"))
                        {
                            LogToFile("Warning: ReportServer database does not exist.", SystemPathsConfig.PathToConfigSSRSErrorLogs, "SSRS_Log.txt");
                        }

                        // Check if ReportServerTempDB db exists
                        if (!DatabaseExists(connection, "ReportServerTempDB"))
                        {
                            LogToFile("Warning: ReportServerTempDB database does not exist.", SystemPathsConfig.PathToConfigSSRSErrorLogs, "SSRS_Log.txt");
                        }

                        // Additional checks can be implemented similar to the above
                    }
                }
                catch (Exception ex)
                {
                    LogToFile($"Warning: Could not connect to the SSRS server at {SSRSConfig.SSRSUrl}. Exception: {ex.Message}", SystemPathsConfig.PathToConfigSSRSErrorLogs, "SSRS_Log.txt");
                }
            }

            private bool DatabaseExists(SqlConnection connection, string databaseName)
            {
                using var command = new SqlCommand($"SELECT db_id(@name)", connection);
                command.Parameters.AddWithValue("@name", databaseName);
                return (command.ExecuteScalar() != DBNull.Value);
            }

            public void LogToFile(string message, string logDirectory, string filename)
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFilePath = Path.Combine(logDirectory, filename);

                // Include the current datetime in the log message with the specified format
                string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string logMessage = $"{timestamp}: {message}{Environment.NewLine}";

                File.AppendAllText(logFilePath, logMessage);
            }


            public List<ConfigurationViewModel.LogEntry> GetLogsFromDirectory(string directoryPath, string logFileName)
            {
                var logEntries = new List<ConfigurationViewModel.LogEntry>();
                string logFilePath = Path.Combine(directoryPath, logFileName);

                if (File.Exists(logFilePath))
                {
                    var logLines = File.ReadAllLines(logFilePath);
                    foreach (var line in logLines)
                    {
                        var entry = ParseLogLine(line);
                        if (entry != null)
                        {
                            logEntries.Add(entry);
                        }
                    }
                }

                return logEntries;
            }

            private ConfigurationViewModel.LogEntry ParseLogLine(string line)
            {
                // Assuming log format: "dd/MM/yyyy HH:mm:ss: Message"
                var parts = line.Split(new[] { ": " }, 2, StringSplitOptions.None);
                if (parts.Length == 2 && DateTime.TryParseExact(parts[0], "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var logDateTime))
                {
                    return new ConfigurationViewModel.LogEntry
                    {
                        LogDateTime = logDateTime,
                        LogMessage = parts[1]
                    };
                }

                return null;
            }

        }

        #region Models
        public class DatabaseConfig
        {
            [DbDisplayName("ServerName")]
            [Description("ServerNameDescription")]
            public string? ServerName { get; set; }

            [DbDisplayName("DatabaseName")]
            [Description("DatabaseNameDescription")]
            public string? DatabaseName { get; set; }
        }

        public class SystemPathsConfig
        {
            [DbDisplayName("RepositoryFolder")]
            [Description("RepositoryFolderDescription")]
            public string RepositoryFolder { get; set; } = "C:\\SIDPSF_Repository\\";

            [DbDisplayName("PathToDocumentationExport")]
            [Description("PathToDocumentationExportDescription")]
            public string PathToDocumentationExport { get; set; } = "C:\\SIDPSF_Repository\\Documentation\\";

            [DbDisplayName("PathToConfigApplicationErrorLogs")]
            [Description("PathToConfigApplicationErrorLogsDescription")]
            public string PathToConfigApplicationErrorLogs { get; set; } = "C:\\SIDPSF_Repository\\ErrorLogs\\Application\\";

            [DbDisplayName("PathToConfigDbContextErrorLogs")]
            [Description("PathToConfigDbContextErrorLogsDescription")]
            public string PathToConfigDbContextErrorLogs { get; set; } = "C:\\SIDPSF_Repository\\ErrorLogs\\DBContext\\";

            [DbDisplayName("PathToConfigSSRSErrorLogs")]
            [Description("PathToConfigSSRSErrorLogsDescription")]
            public string PathToConfigSSRSErrorLogs { get; set; } = "C:\\SIDPSF_Repository\\ErrorLogs\\SSRS\\";

            [DbDisplayName("PathToSchemeFilesExport")]
            [Description("PathToSchemeFilesExportDescription")]
            public string PathToSchemeFilesExport { get; set; } = "C:\\SIDPSF_Repository\\Schemes\\";

            [DbDisplayName("PathToSchemeChildrensBenefitFilesExport")]
            [Description("PathToSchemeChildrensBenefitFilesExportDescription")]
            public string PathToSchemeChildrensBenefitFilesExport { get; set; } = "C:\\SIDPSF_Repository\\Schemes\\Childrens Benefit\\";

            [DbDisplayName("PathToSchemeMaternityBenefitFilesExport")]
            [Description("PathToSchemeMaternityBenefitFilesExportDescription")]
            public string PathToSchemeMaternityBenefitFilesExport { get; set; } = "C:\\SIDPSF_Repository\\Schemes\\Maternity Benefit\\";

            [DbDisplayName("PathUpload")]
            [Description("PathUploadDescription")]
            public string PathUpload { get; set; } = "C:\\SIDPSF_Repository\\Upload\\";

            [DbDisplayName("PathToManuals")]
            [Description("PathToManualsDescription")]
            public string PathToManuals { get; set; } = "C:\\SIDPSF_Repository\\Manuals\\";

            [DbDisplayName("PathToServicesLogs")]
            [Description("PathToServicesLogsDescription")]
            public string PathToServicesLogs { get; set; } = "C:\\SIDPSF_Repository\\ServiceLogs\\";
        }

        public class SSRSConfig
        {
            [DbDisplayName("SSRSUrl")]
            [Description("SSRSUrlDescription")]
            public string SSRSUrl { get; set; } = "DefaultSSRSUrl";

            [DbDisplayName("SSRSUsername")]
            [Description("SSRSUsernameDescription")]
            public string SSRSUsername { get; set; } = "DefaultSSRSUsername";

            [DbDisplayName("SSRSPassword")]
            [Description("SSRSPasswordDescription")]
            public string SSRSPassword { get; set; } = "DefaultSSRSPassword";
        }

        public class AuthenticationConfig
        {
            [DbDisplayName("MaxNumberOfFailedLoginsBeforeLockout")]
            [Description("MaxNumberOfFailedLoginsBeforeLockoutDescription")]
            public int MaxNumberOfFailedLoginsBeforeLockout { get; set; } = 5;

            [DbDisplayName("MaxNumberOfFailedLoginsBeforeLockoutPeriodBetweenTriesMinutes")]
            [Description("MaxNumberOfFailedLoginsBeforeLockoutPeriodBetweenTriesMinutesDescription")]
            public int MaxNumberOfFailedLoginsBeforeLockoutPeriodBetweenTriesMinutes { get; set; } = 5;

            [DbDisplayName("UnlockLockoutStatusAutomaticallyAfterMinutes")]
            [Description("UnlockLockoutStatusAutomaticallyAfterMinutesDescription")]
            public int UnlockLockoutStatusAutomaticallyAfterMinutes { get; set; } = 5;

            [DbDisplayName("LoggedInUserNoActivityPeriodMinutes")]
            [Description("LoggedInUserNoActivityPeriodMinutesDescription")]
            public int LoggedInUserNoActivityPeriodMinutes { get; set; } = 10;

            [DbDisplayName("PreventCreationOfDuplicatePersonalNoUser")]
            [Description("PreventCreationOfDuplicatePersonalNoUserDescription")]
            public bool PreventCreationOfDuplicatePersonalNoUser { get; set; } = true;

            [DbDisplayName("PreventCreationOfDuplicateEmailUser")]
            [Description("PreventCreationOfDuplicateEmailUserDescription")]
            public bool PreventCreationOfDuplicateEmailUser { get; set; } = true;

            [DbDisplayName("PasswordRequiresUppercase")]
            [Description("PasswordRequiresUppercaseDescription")]
            public bool PasswordRequiresUppercase { get; set; } = true;

            [DbDisplayName("PasswordRequiresSpecialChar")]
            [Description("PasswordRequiresSpecialCharDescription")]
            public bool PasswordRequiresSpecialChar { get; set; } = true;

            [DbDisplayName("PasswordRequiresDigit")]
            [Description("PasswordRequiresDigitDescription")]
            public bool PasswordRequiresDigit { get; set; } = true;

            private int _passwordMinimumLength = 6;

            [DbDisplayName("PasswordMinimumLength")]
            [Description("PasswordMinimumLengthDescription")]
            public int PasswordMinimumLength
            {
                get => _passwordMinimumLength;
                set => _passwordMinimumLength = value < 6 ? 6 : value;
            }
        }

        public class ServiceConfig
        {
            [DbDisplayName("ServiceName")]
            [Description("ServiceNameDescription")]
            public string ServiceName { get; set; }

            [DbDisplayName("ServiceDescription")]
            [Description("ServiceDescriptionDescription")]
            public string ServiceDescription { get; set; }

            [DbDisplayName("ServiceCode")]
            [Description("ServiceCodeDescription")]
            public string ServiceCode { get; set; }

            [DbDisplayName("ServiceSleepDelayInMinutes")]
            [Description("ServiceSleepDelayInMinutesDescription")]
            public int ServiceSleepDelayInMinutes { get; set; }

            [DbDisplayName("ServiceDateOfMonthToExecuteTask")]
            [Description("ServiceDateOfMonthToExecuteTaskDescription")]
            public int? ServiceDateOfMonthToExecuteTask { get; set; }

            [DbDisplayName("ServiceDateOfMonthToEndExecuteTaskExecution")]
            [Description("ServiceDateOfMonthToEndExecuteTaskExecutionDescription")]
            public int? ServiceDateOfMonthToEndExecuteTaskExecution { get; set; }

            [DbDisplayName("ServiceLogName")]
            [Description("ServiceLogNameDescription")]
            public string ServiceLogName { get; set; }
        }

        #endregion

    }
}
