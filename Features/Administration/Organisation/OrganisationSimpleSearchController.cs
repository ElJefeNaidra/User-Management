using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.Razor;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace SIDPSF.Features.Administration.Organisation
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class OrganisationSimpleSearchController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "Organisation";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}SimpleSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        public OrganisationSimpleSearchController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }

        [ChildAction("Administration.Organisation.GetView", "Organisation", "Organizata", "Organizacija")]
        [IsMenuItem]
        
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            string SchemaNameDisplay = FormTranslationHelper.TableNameDisplay("Administration");
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Organisation");

            OrganisationSimpleSearchModel model = new OrganisationSimpleSearchModel();
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

            var (Data, TotalRows) = await _dbContext.GridDataAsync("[Administration].[USP_Organisation_Grid_Simple]", filterParameters);

            // Put the returned data into the model
            model.OrganisationData = Data;
            model.TotalRows = TotalRows;

            // Return View
            return View(ViewName, model);
        }
    }
}
