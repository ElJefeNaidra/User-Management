using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.Organisation
{
    public partial class OrganisationController : Controller
    {
        #region ENABLE

        [ChildAction("Administration.Organisation.Enable", "Organisation - Enable", "Organizata - Aktivizim", "Organizacija - Aktiviranje")]
        [HttpGet]
        public async Task<IActionResult> Enable(int Id, string _RowGuid)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            OrganisationModel model = new OrganisationModel();
            model.IdOrganisation = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<OrganisationModel>(SPName, model);

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
                    
                    await GetHistoryGrid(Id, model);
                    return View(ViewPath, model);
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }


        [ChildAction("Administration.Organisation.Enable", "Organisation - Enable", "Organizata - Aktivizim", "Organizacija - Aktiviranje")]
        [HttpPost]
        public async Task<IActionResult> Enable(OrganisationModel model)
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
                var (ResponseInfo, Data) = await _dbContext.UpdateAsync<OrganisationModel>(SPName, model);

                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    return RedirectToAction("Read", new { Id = model.IdOrganisation, _RowGuid = model._RowGuid });
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdOrganisation, _RowGuid = model._RowGuid });
                }
            }
        }

        #endregion
    }
}
