using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.DataAccess;
using UserManagement.Common.Razor;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace UserManagement.Features.Administration.NonMatchingLogin
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class NonMatchingLoginSimpleSearchController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "NonMatchingLogin";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}SimpleSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        public NonMatchingLoginSimpleSearchController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }


        [ChildAction("Administration.NonMatchingLogin.GetView", "Failed Logins", "Lajm�rimet e d�shtuara", "Neuspela prijavljivanja")]
        [IsMenuItem]
        
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            string SchemaNameDisplay = FormTranslationHelper.TableNameDisplay("Administration");
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Failed Logins");

           NonMatchingLoginSimpleSearchModel model = new NonMatchingLoginSimpleSearchModel();
            model.WindowTitle = $"{TableNameDisplay} - {FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Search, _ResourceRepo)}";
            model.FormTitle = $"{TableNameDisplay} - {FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Search, _ResourceRepo)}";
            model.BreadCrumbRoot = SchemaNameDisplay;
            model.BreadCrumbTitle = TableNameDisplay;

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

            var (Data, TotalRows) = await _dbContext.GridDataAsync("[Administration].[USP_NonMatchingLogin_Grid_Simple]", filterParameters);

            // Put the returned data into the model
            model.NonMatchingLoginData = Data;
            model.TotalRows = TotalRows;

            // Return View
            return View(ViewName, model);
        }
    }
}
