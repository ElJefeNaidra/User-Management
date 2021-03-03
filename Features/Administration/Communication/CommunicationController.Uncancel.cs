using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.Razor;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.Communication
{
    public partial class CommunicationController : Controller
    {
        #region UNCANCEL

        [ChildAction("Administration.Communication.Uncancel", "Communication - Uncancel", "Komunikimi - Aktivizim", "Komunikacija - Aktiviranje")]
        [HttpGet]
        public async Task<IActionResult> Uncancel(int Id, string _RowGuid)
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
                if (model._CtrlCanUncancel != true) // IF CANT Uncancel = TRUE
                {
                    TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("ErrorMessageCantUncancel");
                    return RedirectToAction("Read", new { Id = Id });
                }
                else
                {
                    SetupModel(model, CrudOp.Uncancel);
                    ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields Uncancel

                    // Uncancel the comment box
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


        [ChildAction("Administration.Communication.Uncancel", "Communication - Uncancel", "Komunikimi - Aktivizim", "Komunikacija - Aktiviranje")]
        [HttpPost]
        public async Task<IActionResult> Uncancel(CommunicationModel model)
        {
            SetupModel(model, CrudOp.Uncancel);

            // Uncancel PROPERTIES WE DONT REQUIRE TO BE Uncancel IN VIEW IN Uncancel MODE
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // Get Stored Procedure Name
            string SPName = $"[{SchemaName}].[USP_{TableName}_Uncancel]";

            // Remove model state errors based on SPName
            _dbContext.RemoveErrorsBasedOnSpParams(ModelState, SPName, model);

            // Setup control property values
            model.IsCanceled = false;
            model.DateCanceledUncanceled = DateTime.Now;
            model._CtrlCanUncancel = false;
            model._CtrlCanCancel = true;
            model._CtrlCanResend = true;

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
