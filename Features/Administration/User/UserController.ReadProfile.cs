using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.User
{
    public partial class UserController : Controller
    {
        #region READ PROFILE

        [ChildAction("Administration.User.ReadProfile", "User - Read Profile", "Përdoruesi - Lexim profili", "Korisnik - Čitanje profila")]
        [HttpGet]
        public async Task<IActionResult> ReadProfile()
        {
            // Are parameters sent in?

            UserModel model = new UserModel();

            // Filtering the dataset
            model.IdUser = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser")); // SET MODEL ID PK FOR FILTERING
            model._RowGuid = _accessor.HttpContext.Session.GetString("IdUserRowGUID"); // Security Protocol Impelementation with RowGuid

            // Addding the standard tracking operation values
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");


            // EXEC the SP
            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<UserModel>(SPName, model);

            model = Data;

            if (model is not null)
            {
                // Update common properties
                model.crudOp = null;
                model.WindowTitle = $"{TableName} ReadProfile";
                model.FormTitle = $"{TableName} ReadProfile";
                model.BreadCrumbRoot = TableName;
                model.BreadCrumbTitle = "ReadProfile";
                model.ControllerName = "";
                model.ActionName = "";
                model.ReturnUrl = ReturnUrl;


                // Disable all form controls
                ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields disabled

                // Get history data
                await GetUserActivityGrid(model.IdUser, model);

                return View(ProfileViewPath, model);
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }

        #endregion

        #region USER ACTIVITY
        private async Task GetUserActivityGrid(int Id, UserModel model)
        {
            var filterParameters = new List<KeyValuePair<string, object>>()
            {
                // Add parameters if you please
                new KeyValuePair<string, object>("IdUser", Id)
            };

            var columnsToRender = new List<string>()
            {
                // Add columns that you explicitly want to show
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            string SPName = $"[{SchemaName}].[USP_{TableName}_Grid_UserActivity]";

            var (Data, TotalRows) = await _dbContext.GridDataTableSimpleAsync(SPName, filterParameters);
            model.UserActivityData = Data;
            model.TotalRowsUserActivityData = TotalRows;
        }
        #endregion
    }
}

