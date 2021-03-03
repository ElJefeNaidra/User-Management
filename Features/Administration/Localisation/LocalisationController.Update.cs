using Microsoft.AspNetCore.Mvc;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.Localisation
{
    public partial class LocalisationController : Controller
    {
        #region UPDATE

        [ChildAction("Administration.Localisation.Update", "Localisation - Update", "Lokallizimi - Azhurim", "Lokalizacija - Ažuriranje")]
        [HttpGet]
        public async Task<IActionResult> Update(int Id, string _RowGuid)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            LocalisationModel model = new LocalisationModel();
            model.IdLocalisation = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync(SPName, model);

            model = Data;

            // Setup control disabled/enabled
            model.disable_ResourceName = true;

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
                    await GetHistoryGrid(Id, model);
                    return View(ViewPath, model);
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }


        [ChildAction("Administration.Localisation.Update", "Localisation - Update", "Lokallizimi - Azhurim", "Lokalizacija - Ažuriranje")]
        [HttpPost]
        public async Task<IActionResult> Update(LocalisationModel model)
        {
            SetupModel(model, CrudOp.Update);

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Setup control properties
            model._CtrlCanUpdate = true;

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
                    // Get history data
                    await GetHistoryGrid(model.IdLocalisation, model);
                    return View(ViewPath, model);
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdLocalisation, _RowGuid = model._RowGuid });
                }
            }
        }

        #endregion
    }
}
