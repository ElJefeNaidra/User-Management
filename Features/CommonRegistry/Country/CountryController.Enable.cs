using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.Razor;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.CommonRegistry.Country
{
    public partial class CountryController : Controller
    {
        #region ENABLE

        [ChildAction("CommonRegistry.Country.Enable", "Country - Enable", "Shteti - Aktivizim", "Drzava - Aktiviranje")]
        [HttpGet]
        public async Task<IActionResult> Enable(int Id, string _RowGuid)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            CountryModel model = new CountryModel();
            model.IdCountry = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<CountryModel>(SPName, model);

            model = Data;

            if (model is not null)
            {
                if (model._CtrlCanEnable != true) // IF CANT Enable = TRUE
                {
                    TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("ErrorMessageCantEnable");
                    return RedirectToAction("Read", new { Id = Id });
                }
                else
                {
                    SetupModel(model, CrudOp.Enable);
                    ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields Enabled

                    // Enable the comment box
                    model.disable_EnabledDisabledComment = false;
                    model.EnabledDisabledComment = null;
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

        [ChildAction("CommonRegistry.Country.Enable", "Country - Enable", "Shteti - Aktivizim", "Drzava - Aktiviranje")]
        [HttpPost]
        public async Task<IActionResult> Enable(CountryModel model)
        {
            SetupModel(model, CrudOp.Enable);

            // Enable PROPERTIES WE DONT REQUIRE TO BE ENABLED IN VIEW IN Enable MODE
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Get Stored Procedure Name
            string SPName = $"[{SchemaName}].[USP_{TableName}_Enable]";

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, SPName, model);

            // Setup control property values
            model.IsEnabled = true;
            model.DateEnabledDisabled = DateTime.Now;
            model._CtrlCanDisable = true;
            model._CtrlCanEnable = false;
            model._CtrlCanUpdate = true;

            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                var (ResponseInfo, Data) = await _dbContext.UpdateAsync<CountryModel>(SPName, model);

                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    return RedirectToAction("Read", new { Id = model.IdCountry, _RowGuid = model._RowGuid });
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdCountry, _RowGuid = model._RowGuid });
                }
            }
        }

        #endregion
    }
}
