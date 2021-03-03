using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace SIDPSF.Features.Administration.User
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class UserSimpleSearchController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "User";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}SimpleSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        public UserSimpleSearchController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }

        [ChildAction("Administration.User.GetView", "User", "Përdoruesi", "Korisnik")]
        [IsMenuItem]
        
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            var model = new UserSimpleSearchModel
            {
                WindowTitle = TableName + "Search",
                FormTitle = TableName + "Search",
                BreadCrumbRoot = SchemaName,
                BreadCrumbTitle = TableName
            };


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

            var (Data, TotalRows) = await _dbContext.GridDataAsync("[Administration].[USP_User_Grid_Simple]", filterParameters);

            // Put the returned data into the model
            model.UserData = Data;
            model.TotalRows = TotalRows;

            // Return View
            return View(ViewName, model);
        }
    }
}
