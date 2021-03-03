using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.DataAccess;
using UserManagement.Common.Razor;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace UserManagement.Features.CommonRegistry.Municipality
{
    [Parent("CommonRegistry", "Common Registries", "Regjistrat", "Registri")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class MunicipalitySimpleSearchController : Controller
    {
        private const string SchemaName = "CommonRegistry";
        private const string TableName = "Municipality";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}SimpleSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        public MunicipalitySimpleSearchController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }

        [ChildAction("CommonRegistry.Municipality.GetView", "Municipality", "Komuna", "Opstina")]
        [IsMenuItem]
        
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            string SchemaNameDisplay = FormTranslationHelper.TableNameDisplay("Common Registries");
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Municipality");

            MunicipalitySimpleSearchModel model = new MunicipalitySimpleSearchModel();
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

            var (Data, TotalRows) = await _dbContext.GridDataAsync("[CommonRegistry].[USP_Municipality_Grid_Simple]", filterParameters);

            // Put the returned data into the model
            model.MunicipalityData = Data;
            model.TotalRows = TotalRows;

            // Return View
            return View(ViewName, model);
        }
    }
}
