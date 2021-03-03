using Microsoft.AspNetCore.Mvc;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.CommonRegistry.Language
{
    public partial class LanguageController : Controller
    {
        #region CREATE

        [ChildAction("CommonRegistry.Language.Create", "Language - Create", "Gjuha - Krijim", "Jezik - Kreiranje")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            LanguageModel model = new LanguageModel();
            SetupModel(model, CrudOp.Create);
            return View(ViewPath, model);
        }

        [ChildAction("CommonRegistry.Language.Create", "Language - Create", "Gjuha - Krijim", "Jezik - Kreiranje")]
        [HttpPost]
        public async Task<IActionResult> Create(LanguageModel model)
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
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, "CommonRegistry.USP_Language_Create", model);

            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                string SPName = $"[{SchemaName}].[USP_{TableName}_Create]";
                var (ResponseInfo, Data) = await _dbContext.InsertAsync<LanguageModel>(SPName, model);

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

