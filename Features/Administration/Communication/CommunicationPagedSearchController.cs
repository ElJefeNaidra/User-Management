using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.Grid;
using SIDPSF.Common.Razor;
using System.Data;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Features.Administration.ConfigurationProvider;

namespace SIDPSF.Features.Administration.Communication
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    public partial class CommunicationPagedSearchController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "Communication";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}PagedSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;
        private readonly SystemConfigService _configService;

        public CommunicationPagedSearchController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor, SystemConfigService configService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        [TypeFilter(typeof(RequestAuthorisationFilter))]
        [ChildAction("Administration.Communication.GetView", "Communication", "Komunikimi", "Komunikacija")]
        [IsMenuItem]
        [HttpGet]
        public async Task<IActionResult> GetView()
        {

            string SchemaNameDisplay = FormTranslationHelper.TableNameDisplay("Administration");
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Communication");

            CommunicationPagedSearchModel model = new CommunicationPagedSearchModel();
            model.WindowTitle = $"{TableNameDisplay} - {FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Search, _resourceRepo)}";
            model.FormTitle = $"{TableNameDisplay} - {FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Search, _resourceRepo)}";
            model.BreadCrumbRoot = SchemaNameDisplay;
            model.BreadCrumbTitle = TableNameDisplay;

            // Retrieve IdLanguage from the session
            string idLanguage = _accessor.HttpContext.Session.GetString("IdLanguage");

            // Populate the dropdown list based on IdLanguage
            model.PopulateIdStatusDdl(idLanguage);

            // Initial Load
            var filterParameters = new List<KeyValuePair<string, object>>()
            {
                // Add parameters if you please
                new KeyValuePair<string, object>("PageSize", "10"),
                new KeyValuePair<string, object>("PageNumber", "1"),
            };

            var columnsToRender = new List<string>()
            {
                // Add columns that you explicitly want to show
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            string SPName = "[Administration].[USP_Communication_Grid_Paged]";
            var (Data, TotalRows) = await _dbContext.GridDataAsync(SPName, filterParameters);

            // Put the returned data into the model
            model.CommunicationData = Data;
            model.Total = TotalRows;

            return View(ViewName, model);
        }

        [TypeFilter(typeof(RequestAuthorisationBasicFilter))]
        
        [HttpPost]
        public async Task<IActionResult> GetSearchGridData([DataSourceRequest] DataSourceRequest request, CommunicationPagedSearchModel filterData, int PageSize, int Page)
        {
            CommunicationPagedSearchModel filter = new();
            filter = filterData;

            var filterParameters = new List<KeyValuePair<string, object>>()
            {
               // Add parameters if you please
                new KeyValuePair<string, object>("PageSize", PageSize),
                new KeyValuePair<string, object>("PageNumber", request.Page),
                new KeyValuePair<string, object>("IdTypeOfCommunication", filterData.IdTypeOfCommunication),
                new KeyValuePair<string, object>("PhoneNo", filterData.PhoneNo),
                new KeyValuePair<string, object>("EmailAddress", filterData.EmailAddress),
                new KeyValuePair<string, object>("EmailSubject", filterData.EmailSubject),
                new KeyValuePair<string, object>("IdStatus", filterData.IdStatus)
            };

            var columnsToRender = new List<string>()
            {
                // Add columns that you explicitly want to show
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            string SPName = "[Administration].[USP_Communication_Grid_Paged]";
            var (Data, TotalRows) = await _dbContext.GridDataAsync(SPName, filterParameters);

            if (Data == null)
            {
                return Json(new { error = true, message = _Resources.Grid.NoRecords });
            }

            List<dynamic> list = Converters.ConvertDataTableToDynamicList(Data);

            DataSourceResult result = new DataSourceResult
            {
                Data = list,
                Total = TotalRows
            };

            return Json(result);
        }
    }
}

