using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.DataAccess;
using UserManagement.Common.RequestFilters;
using UserManagement.Common.Security;
using UserManagement.Features.Administration.NonMatchingLogin;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static UserManagement.Features.Administration.ConfigurationProvider;

namespace UserManagement.Features.Administration.User.UserLogin
{
    [RequestAuthorisationNoCheckFilter]
    public partial class UserLoginController : Controller
    {
        private const string ViewPath = $"~/Features/Administration/User/UserLogin/UserLogin.cshtml";
        private const string UserLoginSP = "[Administration].[USP_User_Login]";
        private const string UserUpdateLoginInfoSP = "[Administration].[USP_User_UpdateLoginInfo]";
        private const string UserGetAuthorisationSP = "[Administration].[USP_User_GetAuthorisation]";
        private const string NonMatchingLoginCreateSP = "[Administration].[USP_NonMatchingLogin_Create]";
        private const string UserGetMenuSP = "[Administration].[USP_User_GetMenu]";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserAgentService _userAgentService;
        private readonly SystemConfigService _configService;

        public UserLoginController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor, IUserAgentService userAgentService, SystemConfigService configService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _userAgentService = userAgentService ?? throw new ArgumentNullException(nameof(userAgentService));
            _configService = configService ?? throw new ArgumentNullException(nameof(userAgentService));
        }

        [HttpGet]
        public IActionResult Login()
        {
            UserLoginModel model = new UserLoginModel();
            return View(ViewPath, model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("LoginMsgInvalidInput");
                return RedirectToAction("Login");
            }

            // Capture SQLI common chars
            if(Utilities.PasswordIsSQLI(model.Password) == true)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }

            model.MaxNumberOfFailedLoginsBeforeLockout = _configService.AuthenticationConfig.MaxNumberOfFailedLoginsBeforeLockout;
            model.MaxNumberOfFailedLoginsBeforeLockoutPeriodBetweenTriesMinutes = _configService.AuthenticationConfig.MaxNumberOfFailedLoginsBeforeLockoutPeriodBetweenTriesMinutes;
            model.UnlockLockoutStatusAutomaticallyAfterMinutes = _configService.AuthenticationConfig.UnlockLockoutStatusAutomaticallyAfterMinutes;
            model.LoggedInUserNoActivityPeriodMinutes = _configService.AuthenticationConfig.LoggedInUserNoActivityPeriodMinutes;

            UserModel userModel = await AuthenticateUser(model);
            if (userModel != null)
            {
                return await HandleSuccessfulLogin(userModel);
            }

            await LogFailedLoginAttempt(model);
            TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("LoginMsgLoginFailed");
            return RedirectToAction("Login");
        }

        private async Task<UserModel> AuthenticateUser(UserLoginModel model)
        {
            model.Password = EncryptionDecryptionRoutines.Encrypt(model.Password, "");
            var (ResponseInfo, UserData) = await _dbContext.ReadDifferentFilterModelAsync<UserLoginModel, UserModel>(UserLoginSP, model);
            return UserData;
        }

        private async Task<IActionResult> HandleSuccessfulLogin(UserModel userModel)
        {
            if (userModel.IsLockedOut == true)
            {
                TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("LoginMsgUserLockedOut");
                return RedirectToAction("Login");
            }

            if (userModel.IsEnabled == false)
            {
                TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("LoginMsgUserIsDisabled");
                return RedirectToAction("Login");
            }

            if (userModel.IsSystemProceess == true)
            {
                TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("LoginMsgUserSystemProccessIsNotUser");
                return RedirectToAction("Login");
            }

            if (userModel.IsLoggedIn == true)
            {
                TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("LoginMsgUserIsLoggedIn");
                return RedirectToAction("Login");
            }

            await UpdateUserLoginInfo(userModel);
            await SetSessionVariables(userModel);
            return RedirectToAction(userModel.PasswordNeedsChange ? "ChangePassword" : "GetView", userModel.PasswordNeedsChange ? "UserChangePassword" : "LandingView");
        }

        private async Task UpdateUserLoginInfo(UserModel userModel)
        {
            UserAgentInfoModel userAgentInfoModel = _userAgentService.GetUserAgentInfo(_accessor.HttpContext);

            UserModel modelUpdate = new UserModel
            {
                IdUser = userModel.IdUser,
                CurrentSessionID = userAgentInfoModel.SessionID,
                _IdUserOperation = userModel.IdUser,
                _IpAddress = userAgentInfoModel.IPAddress,
                _GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID")
            };

            await _dbContext.UpdateAsync(UserUpdateLoginInfoSP, modelUpdate);
        }

        private async Task SetSessionVariables(UserModel userModel)
        {
            _accessor.HttpContext.Session.SetString("IdUser", userModel.IdUser.ToString());
            _accessor.HttpContext.Session.SetString("IdUserRowGUID", userModel._RowGuid.ToString());
            _accessor.HttpContext.Session.SetString("IsLoggedIn", "1");
            _accessor.HttpContext.Session.SetString("IdLanguage", userModel.IdLanguageOfUser.ToString());
            _accessor.HttpContext.Session.SetString("UserFirstNameLastName", userModel.FirstName + " " + userModel.LastName);
            _accessor.HttpContext.Session.SetString("UserName", userModel.Username);
            _accessor.HttpContext.Session.SetString("IdOrganisation", userModel.IdOrganisation.ToString());


            // Load Authorisations/Permissions
            var filterParameters = new List<KeyValuePair<string, object>>()
                    {
                        // Add parameters to filter the stored procedure
                        new KeyValuePair<string, object>("IdUser", userModel.IdUser),
                      };

            // Get authorisation list as datatable
            var DatatableResult = await _dbContext.QueryResultToDatatable(UserGetAuthorisationSP, filterParameters);

            // Storage for Authorisations JSON
            _accessor.HttpContext.Session.SetObjectAsJson("Authorisations", DatatableResult);


            // Get menu only if the PasswordNeedsChange = false because we will be redirecting to Login after successful PW change
            if (userModel.PasswordNeedsChange == false)
            {
                filterParameters.Add(new KeyValuePair<string, object>("IdLanguage", userModel.IdLanguageOfUser));

                DatatableResult = await _dbContext.QueryResultToDatatable(UserGetMenuSP, filterParameters);
                // Check if the DataTable has any rows
                if (DatatableResult.Rows.Count > 0)
                {
                    // Assuming the column contains string data and you want to store the first row's value
                    string menuStructure = DatatableResult.Rows[0]["MenuStructure"].ToString();
                    HttpContext.Session.SetString("MenuStructure", menuStructure);
                }
            }
            else
            {
                _accessor.HttpContext.Session.SetString("MenuStructure", "");
            }
        }

        private async Task LogFailedLoginAttempt(UserLoginModel model)
        {
            UserAgentInfoModel userAgentInfoModel = _userAgentService.GetUserAgentInfo(_accessor.HttpContext);

            NonMatchingLoginModel nonMatchingLoginModel = new()
            {
                DateOfAttempt = DateTime.Now,
                Username = model.Username,
                Password = EncryptionDecryptionRoutines.Decrypt(model.Password, ""), // Consider the implications of logging plaintext passwords
                SessionID = userAgentInfoModel.SessionID,
                IpAddress = userAgentInfoModel.IPAddress,
                DeviceType = userAgentInfoModel.DeviceType,
                BrowserName = userAgentInfoModel.BrowserName,
                BrowserVersion = userAgentInfoModel.BrowserVersion,
                OperatingSystem = userAgentInfoModel.OperatingSystem,
                _IdUserOperation = 1,
                _IpAddress = userAgentInfoModel.IPAddress,
                _GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID")
            };

            await _dbContext.InsertAsync(NonMatchingLoginCreateSP, nonMatchingLoginModel);
        }

    }
}
