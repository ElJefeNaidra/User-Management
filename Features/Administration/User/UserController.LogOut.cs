using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.Other;
using UserManagement.Common.Security;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.User
{
    public partial class UserController : Controller
    {
        #region LogOut

        [ChildAction("Administration.User.LogOut", "User - LogOut", "Përdoruesi - Ç'lajmërim", "Korisnik - Odjava")]
        [HttpGet]
        public async Task<IActionResult> LogOut(int Id, string _RowGuid)
        {
            // Are parameters sent in?
            if ((Id == 0 || Id == null) || _RowGuid is null)
                return BadRequest("Invalid request.");

            UserModel model = new UserModel();

            // Filtering the dataset
            model.IdUser = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            // Addding the standard tracking operation values
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // EXEC the SP and get User data
            string SPName = $"[Administration].[USP_User_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<UserModel>(SPName, model);

            model = Data;
            if (model is not null)
            {
                // Generate the new password
                string NewPassword = RandomStringGenerator.GenerateRandomString(10);
                model.Password = EncryptionDecryptionRoutines.Encrypt(NewPassword, "");
                model.TemporaryPasswordText = NewPassword;
                model.PasswordNeedsChange = true;

                // Addding the standard tracking operation values
                model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
                model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
                model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

                // Lets reset the password and log out the user
                SPName = $"[Administration].[USP_User_LogOut]";
                (ResponseInfo, Data) = await _dbContext.UpdateAsync<UserModel>(SPName, model);

                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    // Get history data
                    await GetHistoryGrid(model.IdUser, model);
                    return View(ViewPath, model);
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdUser, _RowGuid = model._RowGuid });
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }
        #endregion
    }
}

