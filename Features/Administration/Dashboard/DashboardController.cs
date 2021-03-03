using Microsoft.AspNetCore.Mvc;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Common.DataAccess.DBContext;
using SIDPSF.Common.DataAccess;

namespace SIDPSF.Features.Administration.Dashboard
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class DashboardController : Controller
    {
        private const string ViewPath = $"~/Features/Administration/Dashboard/Dashboard.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;

        public DashboardController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        [ChildAction("Administration.Dashboard", "Dashboard", "Dashboard", "Dashboard")]
        [IsMenuItem]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {

            DashboardModel model = new DashboardModel();
            // EXEC the SP
            string SPName = $"[Administration].[USP_Administration_Dashboard]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<DashboardModel>(SPName, model);
            model = Data;

            var filterParameters = new List<KeyValuePair<string, object>>()
            {
                // Add parameters if you please
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            var columnsToRender = new List<string>()
            {
                // Add columns that you explicitly want to show
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            if (model is not null)
            {
                var (UserLoggedInData, TotalRows) = await _dbContext.GridDataAsync("[Administration].[USP_Administration_Dashboard_Grid_LoggedInUsers]", filterParameters);
                // Put the returned data into the model
                model.UserLoggedInData = UserLoggedInData;
                model.TotalRows = TotalRows;


                (var FailedLoginsData, TotalRows) = await _dbContext.GridDataAsync("[Administration].[USP_Administration_Dashboard_Grid_FailedLogins]", filterParameters);
                // Put the returned data into the model
                model.FailedLogins = FailedLoginsData;
                model.TotalRows = TotalRows;

                return View(ViewPath, model);
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }
    }
}

