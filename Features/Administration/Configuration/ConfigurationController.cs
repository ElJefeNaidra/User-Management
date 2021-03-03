using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.Razor;
using System.Globalization;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Features.Administration.ConfigurationProvider;

namespace SIDPSF.Features.Administration.Configuration
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class ConfigurationController : Controller
    {
        private const string ConfigurationViewPath = $"~/Features/Administration/Configuration/Configuration.cshtml";
        private const string ServicesViewPath = $"~/Features/Administration/Configuration/Services.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;
        private readonly SystemConfigService _configService;

        public ConfigurationController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor, SystemConfigService configService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _configService = configService;
        }

        [ChildAction("Administration.Configuration.Read", "Configuration", "Konfiguracioni", "Konfiguracija")]
        [IsMenuItem]
        [HttpGet]

        public async Task<IActionResult> Read()
        {
            var viewModel = new ConfigurationViewModel
            {
                DatabaseConfig = _configService.DatabaseConfig,
                SystemPathsConfig = _configService.SystemPathsConfig,
                SSRSConfig = _configService.SSRSConfig,
                AuthenticationConfig = _configService.AuthenticationConfig,
                DatabaseLogs = _configService.GetLogsFromDirectory(_configService.SystemPathsConfig.PathToConfigApplicationErrorLogs, "Database_Log.txt"),
                SSRSErrorLogs = _configService.GetLogsFromDirectory(_configService.SystemPathsConfig.PathToConfigSSRSErrorLogs, "SSRS_Log.txt"),
                ApplicationErrorLogs = _configService.GetLogsFromDirectory(_configService.SystemPathsConfig.PathToConfigApplicationErrorLogs, "Application_Log.txt")
            };

            string CrudOpForTitle = FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Read, _resourceRepo);
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Configuration");

            viewModel.WindowTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            viewModel.FormTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            viewModel.BreadCrumbRoot = TableNameDisplay;
            viewModel.BreadCrumbTitle = CrudOpForTitle.ToString();
            viewModel.ControllerName = "";
            viewModel.ActionName = "";

            return View(ConfigurationViewPath, viewModel);
        }

        [ChildAction("Administration.Configuration.Read", "Services", "Serviset", "Servisi")]
        [IsMenuItem]
        [HttpGet]
        public async Task<IActionResult> ReadServices()
        {
            var viewModel = new ConfigurationViewModel
            {
                ServiceConfigs = _configService.ServiceConfigs
            };

            string CrudOpForTitle = FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Read, _resourceRepo);
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Services");

            viewModel.WindowTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            viewModel.FormTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            viewModel.BreadCrumbRoot = TableNameDisplay;
            viewModel.BreadCrumbTitle = CrudOpForTitle.ToString();
            viewModel.ControllerName = "";
            viewModel.ActionName = "";


            foreach (var service in viewModel.ServiceConfigs)
            {
                var logFilePath = Path.Combine(_configService.SystemPathsConfig.PathToServicesLogs, service.ServiceLogName);
                var logEntries = new List<ConfigurationViewModel.LogEntry>();

                if (System.IO.File.Exists(logFilePath))
                {
                    // Assuming logs are stored one entry per line
                    var logLines = await System.IO.File.ReadAllLinesAsync(logFilePath);
                    foreach (var line in logLines)
                    {
                        // Parse the line into a LogEntry object
                        var logEntry = ParseLogLine(line); // Implement this method based on your log format
                        if (logEntry != null)
                        {
                            logEntries.Add(logEntry);
                        }
                    }
                }

                viewModel.ServiceLogEntries[service.ServiceCode] = logEntries;
            }


            return View(ServicesViewPath, viewModel);
        }

        private ConfigurationViewModel.LogEntry ParseLogLine(string line)
        {
            var parts = line.Split(new[] { " - " }, 2, StringSplitOptions.None);

            if (parts.Length == 2 && DateTime.TryParseExact(parts[0], "dd.MM.yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out var logDateTime))
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
}