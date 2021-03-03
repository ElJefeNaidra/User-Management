using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.RequestFilters;
using SIDPSF.Features.Administration.GlobalAccessTracking;
using SIDPSF.Features.Administration.User.UserLogin;
using System.Data;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequestAuthorisationBasicFilter : ActionFilterAttribute
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserAgentService _userAgentService;

    public RequestAuthorisationBasicFilter(IServiceProvider serviceProvider, IUserAgentService userAgentService)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _userAgentService = userAgentService ?? throw new ArgumentNullException(nameof(userAgentService));
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
            return;
        else
        {
            #region Attach noCache headers to request
            // Set cache control headers to prevent caching
            context.HttpContext.Response.Headers["Cache-Control"] = "no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";
            #endregion

            var session = context.HttpContext.Session;

            #region Check if Session is still alive on server
            // Check if session values exist
            var userId = session.GetString("IdUser");
            var isLoggedIn = session.GetString("IsLoggedIn");
            var userFirstNameLastName = session.GetString("UserFirstNameLastName");
            var userName = session.GetString("UserName");
            var idOrganisation = session.GetString("IdOrganisation");

            // If any of these values are null, redirect to the login page
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(isLoggedIn) ||
                string.IsNullOrEmpty(userFirstNameLastName) ||
                string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(idOrganisation))
            {
                context.Result = new RedirectToActionResult("Login", "UserLogin", null);
                return;
            }

            #endregion

            #region Set GlobalAccessTrackingGUID
            // Generate a GUID for the request
            string requestGuid = Guid.NewGuid().ToString();
            session.SetString("GlobalAccessTrackingGUID", requestGuid);
            #endregion

            #region Get Routing data
            // Get controller/action data
            string? controllerName = context.RouteData.Values["controller"]?.ToString();
            string? actionName = context.RouteData.Values["action"]?.ToString();
            string ControllerAction = "/" + controllerName + "/" + actionName;

            // Capture the referrer URL
            var referrerUrl = context.HttpContext.Request.Headers["Referer"].ToString();

            // Capture the complete query string
            var queryString = context.HttpContext.Request.QueryString.Value;

            if (queryString is null || queryString == "")
            {
                queryString = null;
            }
            #endregion

            //Resolve DBContext
            var dbContext = _serviceProvider.GetService<DBContext>();

            #region Check if user status has been changed while the user was logged in
            UserStatusModel userStatusModel = new UserStatusModel();
            userStatusModel.IdUser = Convert.ToInt32(session.GetString("IdUser"));

            //Exec db context
            var (ResponseInfo, UserStatusData) = dbContext.ReadFilteredAsync<UserStatusModel>("[Administration].[USP_User_CheckStatus]", userStatusModel).GetAwaiter().GetResult();

            if (ResponseInfo.HasError || UserStatusData == null)
            {
                context.Result = new RedirectToActionResult("Login", "UserLogin", null);
                return;
            }
            else
            {
                userStatusModel = UserStatusData;
                if (userStatusModel.IsEnabled == false || userStatusModel.IsLockedOut == true || userStatusModel.IsLoggedIn == false)
                {
                    context.Result = new RedirectToActionResult("Login", "UserLogin", null);
                    return;
                }
            }
            #endregion

            #region Record the Global Access Tracking data
            // Insert data into GlobalAccessTracking
            UserAgentInfoModel userAgentInfoModel = new UserAgentInfoModel();
            userAgentInfoModel = _userAgentService.GetUserAgentInfo(context.HttpContext);

            GlobalAccessTrackingModel model = new()
            {
                DateOfRecord = DateTime.Now,
                GlobalAccessTrackingGUID = requestGuid.ToUpper(),

                IdUser = Convert.ToInt32(session.GetString("IdUser")),
                
                IdAuthorisation = Convert.ToInt32(1), // Administration
                SPExecuted = null,
                ControllerName = controllerName,
                ControllerAction = actionName,

                QueryString = queryString,

                // Get common User Agent and other stuff
                IPAddress = userAgentInfoModel.IPAddress,
                IsLocalIP = false,
                UserAgent = userAgentInfoModel.UserAgent,
                Browser = userAgentInfoModel.BrowserName,
                OperatingSystem = userAgentInfoModel.OperatingSystem.ToString(),
                Referrer = referrerUrl,
                Languages = userAgentInfoModel.Languages,
                IsHttps = userAgentInfoModel.IsHttps,
                Protocol = userAgentInfoModel.Protocol,
                SessionID = userAgentInfoModel.SessionID,
                SessionContent = userAgentInfoModel.SessionData,
                RequestInfo = null,
                _IdUserOperation = Convert.ToInt32(session.GetString("IdUser")),
                _IpAddress = userAgentInfoModel.IPAddress,
                _GlobalAccessTrackingGUID = requestGuid.ToUpper()
            };

            // Exec db context
            (ResponseInfo, var GlobalAccessTrackingData) = dbContext.InsertAsync<GlobalAccessTrackingModel>("[Administration].[USP_GlobalAccessTracking_Create]", model).GetAwaiter().GetResult();


            if (ResponseInfo.HasError || UserStatusData == null)
            {
                context.Result = new RedirectToActionResult("Login", "UserLogin", null);
                return;
            }
            #endregion
        }
    }
}
