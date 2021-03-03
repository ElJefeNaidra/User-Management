using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.GlobalAccessTracking
{
    public partial class GlobalAccessTrackingController : Controller
    {
        #region HISTORY
        private async Task GetHistoryGrid(int Id, GlobalAccessTrackingModel model)
        {
            var filterParameters = new List<KeyValuePair<string, object>>()
            {
                // Add parameters if you please
                new KeyValuePair<string, object>("IdGlobalAccessTracking", Id)
            };

            var columnsToRender = new List<string>()
            {
                // Add columns that you explicitly want to show
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            string SPName = $"[{SchemaName}].[USP_{TableName}_Grid_History]";

            var (Data, TotalRows) = await _dbContext.GridDataAsync(SPName, filterParameters);
            model.HistoryData = Data;
            model.TotalRows = TotalRows;
        }

        [ChildAction("Administration.GlobalAccessTracking.ReadHistory", "Global Access Tracking - History", "Monitorimi i qasjes - Historiku", "Monitorovanje pristupa - Istorija")]
        [HttpGet]
        public async Task<IActionResult> ReadHistory(int Id, string _RowGuid, int _RowTimestamp)
        {
            if (Id == 0 || Id == null || _RowGuid is null)
                return BadRequest("Invalid request.");

            GlobalAccessTrackingModel model = new GlobalAccessTrackingModel();
            model.IdGlobalAccessTracking = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid
            model._RowTimestamp = _RowTimestamp;// Security Protocol Impelementation with RowGuid

            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");


            string SPName = $"[{SchemaName}].[USP_{TableName}_Read_History]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<GlobalAccessTrackingModel>(SPName, model);

            model = Data;
            if (model is not null)
            {
                SetupModel(model, CrudOp.ReadHistory);

                ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields disabled

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
