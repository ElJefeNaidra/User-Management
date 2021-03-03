using Microsoft.AspNetCore.Mvc;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.CommonRegistry.Municipality
{
    public partial class MunicipalityController : Controller
    {
        #region UPDATE

        [ChildAction("CommonRegistry.Municipality.Update", "Municipality - Update", "Komuna - Azhurim", "Opstina - Ažuriranje")]
        [HttpGet]
        public async Task<IActionResult> Update(int Id, string _RowGuid)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            MunicipalityModel model = new MunicipalityModel();
            model.IdMunicipality = Id;// SET MODEL ID PK FOR FILTERING
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
                    model.ReturnUrl = Request.Headers["Referer"].ToString();
                    await GetHistoryGrid(Id, model);
                    return View(ViewPath, model);
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }

        [ChildAction("CommonRegistry.Municipality.Update", "Municipality - Update", "Komuna - Azhurim", "Opstina - Ažuriranje")]
        [HttpPost]
        public async Task<IActionResult> Update(MunicipalityModel model)
        {
            SetupModel(model, CrudOp.Update);

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Get Stored Procedure Name
            string SPName = $"[{SchemaName}].[USP_{TableName}_Update]";

            // Setup control property values
            model.IsEnabled = true;
            model._CtrlCanDisable = true;
            model._CtrlCanEnable = false;
            model._CtrlCanUpdate = true;

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
                    return RedirectToAction("Read", new { Id = model.IdMunicipality, _RowGuid = model._RowGuid });
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdMunicipality, _RowGuid = model._RowGuid });
                }
            }
        }

        #endregion
    }
}
