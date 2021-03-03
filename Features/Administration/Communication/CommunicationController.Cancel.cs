using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.Communication
{
    public partial class CommunicationController : Controller
    {
        #region Cancel

        [ChildAction("Administration.Communication.Cancel", "Communication - Cancel", "Komunikimi - Anulim", "Komunikacija - Anuliranjle")]
        [HttpGet]
        public async Task<IActionResult> Cancel(int Id, string _RowGuid)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            CommunicationModel model = new CommunicationModel();
            model.IdCommunication = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<CommunicationModel>(SPName, model);

            model = Data;

            if (model is not null)
            {
                if (model._CtrlCanCancel != true) // IF CANT Cancel = TRUE
                {
                    TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("ErrorMessageCantCancel");
                    return RedirectToAction("Read", new { Id = Id });
                }
                else
                {
                    SetupModel(model, CrudOp.Cancel);
                    ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields Cancel

                    // Cancel the comment box
                    model.disable_CanceledUncanceledComment = false;
                    model.CanceledUncanceledComment = null;

                    await GetHistoryGrid(Id, model);
                    return View(ViewPath, model);
                }
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }


        [ChildAction("Administration.Communication.Cancel", "Communication - Cancel", "Komunikimi - Anulim", "Komunikacija - Anuliranjle")]
        [HttpPost]
        public async Task<IActionResult> Cancel(CommunicationModel model)
        {
            SetupModel(model, CrudOp.Cancel);

            // Cancel PROPERTIES WE DONT REQUIRE TO BE Cancel IN VIEW IN Cancel MODE
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Get Stored Procedure Name
            string SPName = $"[{SchemaName}].[USP_{TableName}_Cancel]";

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, SPName, model);

            // Setup control property values
            model.IsCanceled = true;
            model.DateCanceledUncanceled = DateTime.Now;
            model._CtrlCanUncancel = true;
            model._CtrlCanCancel = false;
            model._CtrlCanResend = false;


            // If model has errors
            if (!ModelState.IsValid)
            {
                return View(ViewPath, model);
            }
            else
            {
                var (ResponseInfo, Data) = await _dbContext.UpdateAsync<CommunicationModel>(SPName, model);

                if (ResponseInfo.HasError || Data == null)
                {
                    TempData["ErrorMessages"] = ResponseInfo.ErrorMessage;
                    return RedirectToAction("Read", new { Id = model.IdCommunication, _RowGuid = model._RowGuid });
                }
                else
                {
                    TempData["SuccessMessages"] = ResponseInfo.InformationMessage;
                    return RedirectToAction("Read", new { Id = model.IdCommunication, _RowGuid = model._RowGuid });
                }
            }
        }

        #endregion
    }
}
