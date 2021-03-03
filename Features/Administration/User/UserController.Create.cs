using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.Other;
using UserManagement.Common.Security;
using UserManagement.Features.Administration.Document;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.User
{
    public partial class UserController : Controller
    {
        #region CREATE

        [ChildAction("Administration.User.Create", "User - Create", "Përdoruesi - Krijim", "Korisnik - Kreiranje")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            UserModel model = new UserModel();
            SetupModel(model, CrudOp.Create);
            return View(ViewPath, model);
        }

        [ChildAction("Administration.User.Create", "User - Create", "Përdoruesi - Krijim", "Korisnik - Kreiranje")]
        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (model is null)
                return BadRequest("Invalid request.");

            // Set model properties for access control
            SetupModel(model, CrudOp.Create);
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Setup control property values
            model.IsSuperAdmin = false;
            model.IsSystemProceess = false;

            // Convert Username to lowercase
            model.Username = model.Username.ToLower();

            // Generate the new password
            string NewPassword = RandomStringGenerator.GenerateRandomString(10);
            model.Password = EncryptionDecryptionRoutines.Encrypt(NewPassword, "");
            model.TemporaryPasswordText = NewPassword;

            model.IsEnabled = true;
            model.IsLockedOut = false;
            model.DateLockedOut = null;
            model.DateLockOutExpired = null;
            model.IsLoggedIn = false;
            model.DateLoggedIn = null;
            model.CurrentSessionID = null;
            model.PasswordNeedsChange = true;
            model._CtrlCanDisable = true;
            model._CtrlCanEnable = false;
            model._CtrlCanLogIn = true;
            model._CtrlCanUpdate = true;
            model._CtrlCanLogOut = false;
            model._CtrlCanPasswordResetAdmin = true;
            model._CtrlCanUserChangePassword = true;

            if (model.FileUploadPersonalDataInterface != null)
            {
                model.FileUploadPersonalData = FileService.SaveFile(model.FileUploadPersonalDataInterface, _configService.SystemPathsConfig.PathUpload);
            }

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, "Administration.USP_User_Create", model);

            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                string SPName = $"[{SchemaName}].[USP_{TableName}_Create]";
                var (ResponseInfo, Data) = await _dbContext.InsertAsync<UserModel>(SPName, model);

                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    return View(ViewPath, model);
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    // Security Protocol Impelementation with RowGuid
                    return RedirectToAction("Read", new { Id = ResponseInfo.IdValue, _RowGuid = ResponseInfo._RowGuid });
                }
            }
        }

        #endregion

        // Validate Email duplicate allowability
        [HttpGet]
        public async Task<IActionResult> ValidateEmailDuplicate(string Email)
        {
            // Check if the new password is the same as the current password
            if (string.IsNullOrEmpty(Email))
            {
                return Json(_resourceRepo.GetResourceByName("EmailCantBeEmpty"));
            }

            // EXEC the SP
            string SPName = $"[Administration].[USP_User_CheckDuplicate]";
            UserModel model = new UserModel();
            model.Email = Email;

            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<UserModel>(SPName, model);

            if (Data == null)
            {
                return Json(true);
            }
            model = Data;

            if (model is not null)
            {
                if (_configService.AuthenticationConfig.PreventCreationOfDuplicateEmailUser == true)
                {
                    return Json(_resourceRepo.GetResourceByName("EmailAddressAlreadyInUse"));
                }
            }

            return Json(true);
        }

        // Validate PersonalNo duplicate allowability
        [HttpGet]
        public async Task<IActionResult> ValidatePersonalNoDuplicate(string PersonalNo)
        {
            // Check if the new password is the same as the current password
            if (string.IsNullOrEmpty(PersonalNo))
            {
                return Json(_resourceRepo.GetResourceByName("PersonalNoCantBeEmpty"));
            }

            // EXEC the SP
            string SPName = $"[Administration].[USP_User_CheckDuplicate]";
            UserModel model = new UserModel();
            model.PersonalNo = PersonalNo;

            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<UserModel>(SPName, model);

            if (Data == null)
            {
                return Json(true);
            }
            model = Data;

            if (model is not null)
            {
                if (_configService.AuthenticationConfig.PreventCreationOfDuplicatePersonalNoUser == true)
                {
                    return Json(_resourceRepo.GetResourceByName("PersonalNoAddressAlreadyInUse"));
                }
            }

            return Json(true);
        }
    }
}

