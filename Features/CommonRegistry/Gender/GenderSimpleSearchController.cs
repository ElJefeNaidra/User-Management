using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace SIDPSF.Features.CommonRegistry.Gender
{
    [Parent("CommonRegistry", "Common Registries", "Regjistrat", "Registri")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class GenderSimpleSearchController : Controller
    {
        private const string SchemaName = "CommonRegistry";
        private const string TableName = "Gender";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}SimpleSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        public GenderSimpleSearchController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }

        [ChildAction("CommonRegistry.Gender.GetView", "Gender", "Gjinia", "Pol")]
        [IsMenuItem]
        
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            string SchemaNameDisplay = FormTranslationHelper.TableNameDisplay("Common Registries");
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Gender");

            GenderSimpleSearchModel model = new GenderSimpleSearchModel();
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

            var (Data, TotalRows) = await _dbContext.GridDataAsync("[CommonRegistry].[USP_Gender_Grid_Simple]", filterParameters);

            // Put the returned data into the model
            model.GenderData = Data;
            model.TotalRows = TotalRows;

            // Return View
            return View(ViewName, model);
        }
    }
}
