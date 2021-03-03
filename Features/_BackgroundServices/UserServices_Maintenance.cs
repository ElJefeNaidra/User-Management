using Microsoft.Data.SqlClient;
using System.Data;
using static UserManagement.Features.Administration.ConfigurationProvider;

/// <summary>
/// Executes the User Maintenance tasks in the background
/// </summary>
public class UserServices_Maintenance : BackgroundService
{
    private readonly string _filePath;
    private readonly string _connectionString;
    private readonly ServiceConfig _serviceConfig;

    public UserServices_Maintenance(string connectionString, SystemConfigService configService)
    {
        _connectionString = connectionString;
        _serviceConfig = configService.ServiceConfigs.FirstOrDefault(c => c.ServiceCode == "UserMaintenance");
        if (_serviceConfig != null)
        {
            _filePath = Path.Combine(configService.SystemPathsConfig.PathToServicesLogs, _serviceConfig.ServiceLogName);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TrimLog();
        // Handle application shutdown gracefully.
        stoppingToken.Register(() => LogMessage($"Background service {_serviceConfig.ServiceCode} is stopping due to a cancellation request.", stoppingToken));

        while (!stoppingToken.IsCancellationRequested)
        {
            await LogMessage("Service is running.", stoppingToken);

            // Execute the stored procedure
            await ExecuteStoredProcedure(stoppingToken);

            // Calculate wake up time
            var wakeUpTime = DateTime.Now.AddMinutes(_serviceConfig.ServiceSleepDelayInMinutes);

            // Log going to sleep and when it's expected to wake up
            await LogMessage($"Service is going to sleep for {_serviceConfig.ServiceSleepDelayInMinutes} minutes. Expected to wake up at {wakeUpTime:dd/MM/yyyy HH:mm:ss}.", stoppingToken);

            // Delay for the specified minutes. If a cancellation is requested, the delay is canceled.
            await Task.Delay(TimeSpan.FromMinutes(_serviceConfig.ServiceSleepDelayInMinutes), stoppingToken);

            // Log wake up message
            await LogMessage("Service is waking up.", stoppingToken);
        }
    }

    private async Task ExecuteStoredProcedure(CancellationToken stoppingToken)
    {
        string SPName = "[Administration].[USP_User_BackgroundService_Maintenance]";
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(stoppingToken);
                using (var command = new SqlCommand(SPName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync(stoppingToken);
                }
            }

            await LogMessage($"Stored procedure {SPName} executed.", stoppingToken);
        }
        catch (Exception ex)
        {
            await LogMessage($"Error executing {SPName} stored procedure: {ex.Message}", stoppingToken);
        }
    }

    // A helper method to log messages to the file.
    private async Task LogMessage(string message, CancellationToken stoppingToken)
    {
        // Ensure the log file exists; create it if it doesn't.
        if (!File.Exists(_filePath))
        {
            using (var stream = File.Create(_filePath)) { }
        }

        // Format the current datetime with milliseconds and append the log message
        string formattedMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} - {message}{Environment.NewLine}";
        await File.AppendAllTextAsync(_filePath, formattedMessage, stoppingToken);
    }

    private void TrimLog()
    {
        const int maxLogEntries = 300;
        const int entriesToRemove = 100;

        try
        {
            var logEntries = new List<string>();

            if (File.Exists(_filePath))
            {
                logEntries = File.ReadAllLines(_filePath).ToList();

                if (logEntries.Count > maxLogEntries)
                {
                    logEntries.RemoveRange(0, entriesToRemove);
                    File.WriteAllLines(_filePath, logEntries);
                }
            }
        }
        catch (Exception ex)
        {
            // Enhanced exception logging
            Console.WriteLine($"Exception in TrimLog: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }
    }
}
