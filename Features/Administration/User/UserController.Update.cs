using Microsoft.AspNetCore.Mvc;
using SIDPSF.Features.Administration.Document;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.User
{
    public partial class UserController : Controller
    {
        #region UPDATE

        [ChildAction("Administration.User.Update", "User - Update", "Përdoruesi - Azhurim", "Korisnik - Ažuriranje")]
        [HttpGet]
        public async Task<IActionResult> Update(int Id, string _RowGuid)
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
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync(SPName, model);

            model = Data;

            if (model is not null)
            {
                if (model._CtrlCanUpdate != true) // IF CANT UPDATE = TRUE
                {
                    TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("ErrorMessageCantUpdate");
                    return RedirectToAction("Read", new { Id = Id });
                }
                else
                {
                    SetupModel(model, CrudOp.Update);
                    // Disable fields we dont need
                    model.disable_PersonalNo = true;
                    model.disable_FirstName = true;
                    model.disable_LastName = true;
                    model.disable_Username = true;
                    await GetHistoryGrid(Id, model);
                    return View(ViewPath, model);
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }


        [ChildAction("Administration.User.Update", "User - Update", "Përdoruesi - Azhurim", "Korisnik - Ažuriranje")]
        [HttpPost]
        public async Task<IActionResult> Update(UserModel model)
        {
            SetupModel(model, CrudOp.Update);

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            if (model.FileUploadPersonalDataUpdateInterface != null)
            {
                model.FileUploadPersonalData = FileService.SaveFile(model.FileUploadPersonalDataUpdateInterface, _configService.SystemPathsConfig.PathUpload);
            }

            // Get Stored Procedure Name
            string SPName = $"[{SchemaName}].[USP_{TableName}_Update]";

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, SPName, model);

            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                var (ResponseInfo, Data) = await _dbContext.UpdateAsync(SPName, model);

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
