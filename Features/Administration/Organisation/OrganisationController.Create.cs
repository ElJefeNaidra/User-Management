using Microsoft.AspNetCore.Mvc;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.Organisation
{
    public partial class OrganisationController : Controller
    {
        #region CREATE

        [ChildAction("Administration.Organisation.Create", "Organisation - Create", "Organizata - Krijim", "Organizacija - Kreiranje")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            OrganisationModel model = new OrganisationModel();
            SetupModel(model, CrudOp.Create);
            return View(ViewPath, model);
        }

        [ChildAction("Administration.Organisation.Create", "Organisation - Create", "Organizata - Krijim", "Organizacija - Kreiranje")]
        [HttpPost]
        public async Task<IActionResult> Create(OrganisationModel model)
        {
            if (model is null)
                return BadRequest("Invalid request.");

            // Set model properties for access control
            SetupModel(model, CrudOp.Create);
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Setup control property values
            model.IsEnabled = true;
            model.EnabledDisabledComment = null;
            model.DateEnabledDisabled = null;
            model._CtrlCanDisable = true;
            model._CtrlCanEnable = false;
            model._CtrlCanUpdate = true;

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, "Administration.USP_Organisation_Create", model);

            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                string SPName = $"[{SchemaName}].[USP_{TableName}_Create]";
                var (ResponseInfo, Data) = await _dbContext.InsertAsync<OrganisationModel>(SPName, model);

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
    }
}

