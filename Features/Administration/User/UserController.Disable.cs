using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.Razor;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.User
{
    public partial class UserController : Controller
    {
        #region DISABLE

        [ChildAction("Administration.User.Disable", "User - Disable", "Pėrdoruesi - Deaktivizim", "Korisnik - Deaktiviranje")]
        [HttpGet]
        public async Task<IActionResult> Disable(int Id, string _RowGuid)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            UserModel model = new UserModel();
            model.IdUser = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<UserModel>(SPName, model);

            model = Data;

            if (model is not null)
            {
                if (model._CtrlCanDisable != true) // IF CANT Disable = TRUE
                {
                    TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("ErrorMessageCantDisable");
                    return RedirectToAction("Read", new { Id = Id });
                }
                else
                {
                    SetupModel(model, CrudOp.Disable);
                    ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields disabled
                    // Enable the comment box
                    model.disable_EnabledDisabledComment = false;
                    model.EnabledDisabledComment = null;
                    await GetHistoryGrid(Id, model);
                    return View(ViewPath, model);
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }


        [ChildAction("Administration.User.Disable", "User - Disable", "Pėrdoruesi - Deaktivizim", "Korisnik - Deaktiviranje")]
        [HttpPost]
        public async Task<IActionResult> Disable(UserModel model)
        {
            SetupModel(model, CrudOp.Disable);

            // DISABLE PROPERTIES WE DONT REQUIRE TO BE ENABLED IN VIEW IN Disable MODE
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Get Stored Procedure Name
            string SPName = $"[{SchemaName}].[USP_{TableName}_Disable]";

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, SPName, model);

            // Setup control property values
            model.IsEnabled = false;
            model.DateEnabledDisabled = DateTime.Now;
            model._CtrlCanDisable = false;
            model._CtrlCanEnable = true;
            model._CtrlCanUpdate = false;
            model._CtrlCanLogIn = false;
			model._CtrlCanLogOut = false;
            model._CtrlCanPasswordResetAdmin = false;
            model._CtrlCanUserChangePassword = false;

            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                var (ResponseInfo, Data) = await _dbContext.UpdateAsync<UserModel>(SPName, model);

                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    return RedirectToAction("Read", new { Id = model.IdUser, _RowGuid = model._RowGuid });
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdUser, _RowGuid = model._RowGuid });
                }
            }
        }

        #endregion
    }
}
