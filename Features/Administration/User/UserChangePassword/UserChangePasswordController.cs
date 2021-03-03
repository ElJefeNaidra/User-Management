using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.RequestFilters;
using SIDPSF.Common.Security;
using System.Data;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Features.Administration.ConfigurationProvider;

namespace SIDPSF.Features.Administration.User.UserChangePassword
{
    [TypeFilter(typeof(RequestAuthorisationBasicFilter))]
    public partial class UserChangePasswordController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "User";

        private const string ViewPath = $"~/Features/Administration/User/UserChangePassword/UserChangePassword.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserAgentService _userAgentService;
        private readonly SystemConfigService _configService;

        public UserChangePasswordController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor, IUserAgentService userAgentService, SystemConfigService configService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _userAgentService = userAgentService ?? throw new ArgumentNullException(nameof(userAgentService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {

            var model = new UserChangePasswordModel
            {
                WindowTitle = TableName + "ChangePassword",
                FormTitle = TableName + "ChangePassword",
                BreadCrumbRoot = TableName,
                BreadCrumbTitle = "ChangePassword",
                ControllerName = "UserChangePassword",
                ActionName = "ChangePassword"

            };
            return View(ViewPath, model);
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // Set the model values
                model.IdUser = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
                model.Password = EncryptionDecryptionRoutines.Encrypt(model.Password, "");
                model.NewPassword = EncryptionDecryptionRoutines.Encrypt(model.NewPassword, "");
                model.ConfirmNewPassword = EncryptionDecryptionRoutines.Encrypt(model.ConfirmNewPassword, "");

                // additional check if NewPassword and ConfirmNewPassword match
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    // Set the error message indicating the passwords do not match
                    ModelState.AddModelError("", _resourceRepo.GetResourceByName("ChangePasswordPasswordsDontMatch"));
                    return RedirectToAction("ChangePassword");
                }

                UserAgentInfoModel userAgentInfoModel = _userAgentService.GetUserAgentInfo(_accessor.HttpContext);
                model._IdUserOperation = model.IdUser;
                model._IpAddress = userAgentInfoModel.IPAddress;
                model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");

                // Executed update to set new password
                string SPName = "[Administration].[USP_User_ChangePassword]";
                var (ResponseInfo, Data) = await _dbContext.UpdateAsync(SPName, model);


                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    // Security Protocol Impelementation with RowGuid
                    return RedirectToAction("LogOut", "UserLogOut");
                }
            }
            else
            {
                return RedirectToAction("ChangePassword");
            }
        }

        // Validate noe password
        public IActionResult ValidatePasswordStrength(string NewPassword, string Password)
        {

            if (!string.IsNullOrEmpty(NewPassword) == true && !string.IsNullOrEmpty(Password) == false)
            {
                return Json(_resourceRepo.GetResourceByName("EnterTheCurrentPassword"));
            }

            if (NewPassword.Length < _configService.AuthenticationConfig.PasswordMinimumLength)
            {
                return Json(_resourceRepo.GetResourceByName("PasswordMinimumLengthIs") + _configService.AuthenticationConfig.PasswordMinimumLength.ToString());
            }

            // Capture SQLI common chars
            if (Utilities.PasswordIsSQLI(NewPassword) == true)
            {
                return Json(_resourceRepo.GetResourceByName("DisallowedCharactersEntered"));
            }

            // Check if the new password is the same as the current password
            if (!string.IsNullOrEmpty(NewPassword) == true && NewPassword == Password)
            {
                return Json(_resourceRepo.GetResourceByName("NewPasswordMustNotBeTheSameAsCurrent"));
            }

            // Perform other password strength checks based on the configuration

            bool RequiresLowerCase = true;
            bool RequiresUpperCase = _configService.AuthenticationConfig.PasswordRequiresUppercase;
            bool RequiresDigit = _configService.AuthenticationConfig.PasswordRequiresDigit;
            bool RequiresSpecialChar = _configService.AuthenticationConfig.PasswordRequiresSpecialChar;

            List<string> requirements = new List<string>();

            if (RequiresLowerCase == true)
            {
                if (NewPassword.Any(char.IsLower) == false)
                {
                    requirements.Add(_resourceRepo.GetResourceByName("OneLowercaseLetter"));
                }
            }

            if (_configService.AuthenticationConfig.PasswordRequiresUppercase == true)
            {
                if (NewPassword.Any(char.IsUpper) == false)
                {
                    requirements.Add(_resourceRepo.GetResourceByName("OneUppercaseLetter"));
                }
            }

            if (_configService.AuthenticationConfig.PasswordRequiresDigit == true)
            {
                if (NewPassword.Any(char.IsDigit) == false)
                {
                    requirements.Add(_resourceRepo.GetResourceByName("OneDigit"));
                }
            }

            if (_configService.AuthenticationConfig.PasswordRequiresSpecialChar == true)
            {
                if (NewPassword.Any(ch => "#$@!".Contains(ch)) == false)
                {
                    requirements.Add(_resourceRepo.GetResourceByName("OneSpecialCharacter") + " # @ $ ! ");
                }
            }

            string formattedRequirements = string.Join(", ", requirements);

            if (formattedRequirements != null && formattedRequirements != "")
            {
                return Json($"{_resourceRepo.GetResourceByName("PasswordMustContainAtLeast")} {formattedRequirements}.");
            }
            else
            {
                return Json(true);
            }
        }
    }
}
