using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.Communication
{
    public partial class CommunicationController : Controller
    {
        #region READ

        [ChildAction("Administration.Communication.Read", "Communication - Read", "Komunikimi - Lexim", "Komunikacija - Čitanje")]
        [HttpGet]
        public async Task<IActionResult> Read(int Id, string _RowGuid)
        {
            // Are parameters sent in?
            if ((Id == 0 || Id == null) || _RowGuid is null)
                return BadRequest("Invalid request.");

            CommunicationModel model = new CommunicationModel();

            // Filtering the dataset
            model.IdCommunication = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            // Addding the standard tracking operation values
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // EXEC the SP
            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<CommunicationModel>(SPName, model);

            model = Data;
            if (model is not null)
            {
                // Setup the model for READ
                SetupModel(model, CrudOp.Read);

                // Disable all form controls
                ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields disabled

                model._AllowedOperations = new List<string>(); // Ensure the list is initialized.

                // Add allowed operations
                if (model._CtrlCanResend && model.IdCommunication != 0 && !string.IsNullOrEmpty(model._RowGuid))
                {
                    string ResendLink = $"<a href='/Communication/Resend?Id={model.IdCommunication}&_RowGuid={model._RowGuid}&_RowTimestamp={model._RowTimestamp}'>" + _resourceRepo.GetResourceByName("Resend") + "</a>";
                    model._AllowedOperations.Add(ResendLink);
                }

                if (model._CtrlCanCancel && model.IdCommunication != 0 && !string.IsNullOrEmpty(model._RowGuid))
                {
                    string CancelLink = $"<a href='/Communication/Cancel?Id={model.IdCommunication}&_RowGuid={model._RowGuid}&_RowTimestamp={model._RowTimestamp}'>" + _resourceRepo.GetResourceByName("Cancel") + "</a>";
                    model._AllowedOperations.Add(CancelLink);
                }

                if (model._CtrlCanUncancel && model.IdCommunication != 0 && !string.IsNullOrEmpty(model._RowGuid))
                {
                    string UncancelLink = $"<a href='/Communication/Uncancel?Id={model.IdCommunication}&_RowGuid={model._RowGuid}&_RowTimestamp={model._RowTimestamp}'>" + _resourceRepo.GetResourceByName("Uncancel") + "</a>";
                    model._AllowedOperations.Add(UncancelLink);
                }

                // Get history data
                await GetHistoryGrid(Id, model);

                return View(ViewPath, model);
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }
        #endregion
    }
}

