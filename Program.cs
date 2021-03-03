using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using UserManagement.Common.CustomDateTimeModelBinding.Core;
using UserManagement.Common.DataAccess;
using UserManagement.Common.RequestFilters;
using UserManagement.Common.StringLocalisation;
using System.Dynamic;
using System.Globalization;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static UserManagement.Features.Administration.ConfigurationProvider;

var builder = WebApplication.CreateBuilder(args);

// Access the connection string
var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddSingleton(connectionString);

builder.Configuration.AddJsonFile("system.config.json", optional: true, reloadOnChange: true);

// Configure and add SystemConfigService to the services
var systemConfigService = new SystemConfigService(builder.Configuration, connectionString);
builder.Services.AddSingleton<SystemConfigService>(systemConfigService);

// Check if the database exists
ApplicationState.DatabaseExists = systemConfigService.CheckDatabaseExists();

if (!ApplicationState.DatabaseExists)
{
    var logPath = systemConfigService.SystemPathsConfig.PathToConfigApplicationErrorLogs;
    var logFile = "Critical_Database_Log.txt";
    systemConfigService.LogToFile("Critical Error: Database does not exist or cannot be connected to.", logPath, logFile);
}
else
{
    InsertConfigurationSettings(builder.Configuration, connectionString);
}


// Now ensure all necessary directories exist
var pathsConfig = systemConfigService.SystemPathsConfig;
systemConfigService.EnsureDirectoryExists(pathsConfig.RepositoryFolder);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathToDocumentationExport);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathToConfigApplicationErrorLogs);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathToConfigDbContextErrorLogs);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathToConfigSSRSErrorLogs);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathUpload);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathToManuals);
systemConfigService.EnsureDirectoryExists(pathsConfig.PathToServicesLogs);


// Check if SSRS is running or installed; if not, log the error
systemConfigService.CheckSsrsServerAvailability();

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "SessionID-X0001";
    options.IdleTimeout = TimeSpan.FromMinutes(systemConfigService.AuthenticationConfig.LoggedInUserNoActivityPeriodMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<DBContext>();
builder.Services.Configure<SessionOptions>(builder.Configuration.GetSection("SessionOptions"));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(culture: "en-GB", uiCulture: "en-GB");
    options.SupportedUICultures = new List<CultureInfo>
    {
        new CultureInfo("en-GB"),
        new CultureInfo("sq-AL"),
        new CultureInfo("sr-Latn-RS")
    };
    options.FallBackToParentUICultures = false;

    // Remove the default AcceptLanguageHeaderRequestCultureProvider
    var providerToRemove = options.RequestCultureProviders
        .FirstOrDefault(p => p is AcceptLanguageHeaderRequestCultureProvider);
    if (providerToRemove != null)
    {
        options.RequestCultureProviders.Remove(providerToRemove);
    }

    // Add your custom culture provider
    options.RequestCultureProviders.Add(new UserManagement.Common.RequestFilters.CustomRequestCultureProvider());
});

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");

// Add MemoryCache service
builder.Services.AddMemoryCache();


// Register your service with a factory method
builder.Services.AddSingleton(sp =>
    new ResourceRepository(connectionString, sp.GetRequiredService<IMemoryCache>()));

builder.Services.AddTransient<DBContext>(sp =>
    new DBContext(
        connectionString,
        sp.GetRequiredService<IMemoryCache>(),
        sp.GetRequiredService<IHttpContextAccessor>() // Add this line
    ));


// Now that DBContext is registered, set the error log path
DBContext.SetDbContextErrorLogsPath(systemConfigService.SystemPathsConfig.PathToConfigDbContextErrorLogs);

builder.Services.AddControllersWithViews(options =>
// add our custom binder to beginning of collection
{
    options.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());
}
).AddJsonOptions(jsonOptions =>
{

    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
})
.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


builder.Services.AddKendo();

builder.Services.AddScoped<IUserAgentService, UserAgentService>();

// Add hosted services
//builder.Services.AddHostedService<CommunicationServices_Send>(provider => new CommunicationServices_Send(connectionString, systemConfigService));
builder.Services.AddHostedService<UserServices_Maintenance>(provider => new UserServices_Maintenance(connectionString, systemConfigService));
var app = builder.Build();

ServiceLocator.SetCurrent(app.Services);

// Early check for database availability and show error if needed
app.Use(async (context, next) =>
{
    if (!ApplicationState.DatabaseExists)
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<h2>Critical Error: Cannot connect to the database specified in appsettings.json. Check the server logs for more details.</h2>");
        return;
    }
    await next();
});


if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseRequestLocalization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=UserLogin}/{action=Login}/{id?}");
});

app.Use(async (context, next) =>
{
    // A local function to simplify session variable initialization
    void InitializeSessionVariable(string key, string defaultValue)
    {
        if (context.Session.GetString(key) == null)
        {
            context.Session.SetString(key, defaultValue);
        }
    }

    // Initialize session variables
    InitializeSessionVariable("IdUser", ""); // Currently Logged in User ID
    InitializeSessionVariable("IdUserRowGUID", ""); // Row GUID of logged in User
    InitializeSessionVariable("IsLoggedIn", "0"); // Is User Logged In?
    InitializeSessionVariable("IdLanguage", "1"); // Interface Language
    InitializeSessionVariable("MenuStructure", ""); // Storage for Menu HTML
    InitializeSessionVariable("Authorisations", ""); // Storage for Authorisations JSON
    InitializeSessionVariable("GlobalAccessTrackingGUID", ""); // Global Access Tracking GUID
    InitializeSessionVariable("IpAddress", ""); // IP Address tracking
    InitializeSessionVariable("UserFirstNameLastName", ""); // First and Last Name
    InitializeSessionVariable("UserName", ""); // Username
    InitializeSessionVariable("IdOrganisation", ""); // IdOrganisation
    await next();
});

app.Run();


void InsertConfigurationSettings(IConfiguration configuration, string connectionString)
{
    var configSections = new List<string> { "DatabaseConfig", "SystemPathsConfig", "SSRSConfig", "AuthenticationConfig" };

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();

        // Check if the table exists, if not, create it
        var checkTableQuery = @"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'Administration' AND TABLE_NAME = 'SystemConfig')
            BEGIN
                CREATE TABLE Administration.SystemConfig(
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    [Key] NVARCHAR(MAX),
                    [Value] NVARCHAR(MAX)
                )
            END";

        using (var command = new SqlCommand(checkTableQuery, connection))
        {
            command.ExecuteNonQuery();
        }

        // Clear existing settings
        var deleteQuery = "DELETE FROM Administration.SystemConfig DBCC CHECKIDENT('Administration.SystemConfig', RESEED, 0)";
        using (var deleteCommand = new SqlCommand(deleteQuery, connection))
        {
            deleteCommand.ExecuteNonQuery();
        }

        // Insert new settings
        foreach (var section in configSections)
        {
            var configSection = configuration.GetSection(section);
            var expandoDict = new ExpandoObject() as IDictionary<string, Object>;

            foreach (var childItem in configSection.GetChildren())
            {
                expandoDict.Add(childItem.Key, childItem.Value);
            }

            foreach (var item in expandoDict)
            {
                var key = item.Key;
                var value = item.Value.ToString();

                // Check if the value is a boolean and convert it to 0 or 1
                if (bool.TryParse(value, out bool boolValue))
                {
                    value = boolValue ? "1" : "0";
                }

                var insertQuery = "INSERT INTO Administration.SystemConfig ([Key], [Value]) VALUES (@key, @value)";
                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@key", key);
                    insertCommand.Parameters.AddWithValue("@value", value);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }
    }
}


public static class ApplicationState
{
    public static bool DatabaseExists { get; set; }
}
