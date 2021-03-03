using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Data;
using System.Dynamic;
using System.Runtime.CompilerServices;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using UserManagement.Features.Administration.Communication;
using UserManagement.Common.DataAccess;
using UserManagement.Common.Razor;
using UserManagement.Common.Grid;

namespace UserManagement.Features.Administration.GlobalAccessTracking
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]

    public partial class GlobalAccessTrackingPagedSearchController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "GlobalAccessTracking";

        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}PagedSearch.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;

        public GlobalAccessTrackingPagedSearchController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _resourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }

        [TypeFilter(typeof(RequestAuthorisationFilter))]
        [ChildAction("Administration.GlobalAccessTracking.GetView", "Global Access Tracking", "Monitorimi i qasjes", "Monitorovanje pristupa")]
        [IsMenuItem]
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            string SchemaNameDisplay = FormTranslationHelper.TableNameDisplay("Administration");
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Global Access Tracking");

            GlobalAccessTrackingPagedSearchModel model = new GlobalAccessTrackingPagedSearchModel();
            model.WindowTitle = $"{TableNameDisplay} - {FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Search, _resourceRepo)}";
            model.FormTitle = $"{TableNameDisplay} - {FormTranslationHelper.CrudOpForTitle(DBContext.CrudOp.Search, _resourceRepo)}";
            model.BreadCrumbRoot = SchemaNameDisplay;
            model.BreadCrumbTitle = TableNameDisplay;

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

            string SPName = "[Administration].[USP_GlobalAccessTracking_Grid_Paged]";
            var (Data, TotalRows) = await _dbContext.GridDataAsync(SPName, filterParameters);

            // Put the returned data into the model
            model.GlobalAccessTrackingData = Data;
            model.Total = TotalRows;

            return View(ViewName, model);
        }

        [TypeFilter(typeof(RequestAuthorisationBasicFilter))]
        
        [HttpPost]
        public async Task<IActionResult> GetSearchGridData([DataSourceRequest] DataSourceRequest request, GlobalAccessTrackingPagedSearchModel filterData, int PageSize, int Page)
        {
            GlobalAccessTrackingPagedSearchModel filter = new();
            filter = filterData;

            var filterParameters = new List<KeyValuePair<string, object>>()
            {
               // Add parameters if you please
                new KeyValuePair<string, object>("PageSize", PageSize),
                new KeyValuePair<string, object>("PageNumber", request.Page),
                new KeyValuePair<string, object>("GlobalAccessTrackingGUID", filterData.GlobalAccessTrackingGUID),
                new KeyValuePair<string, object>("IdUser", filterData.IdUser),
                new KeyValuePair<string, object>("IdAuthorisation", filterData.IdAuthorisation),
                new KeyValuePair<string, object>("ControllerName", filterData.ControllerName),
                new KeyValuePair<string, object>("ControllerAction", filterData.ControllerAction),
                new KeyValuePair<string, object>("IPAddress", filterData.IPAddress),
                new KeyValuePair<string, object>("UserAgent", filterData.UserAgent),
                new KeyValuePair<string, object>("Browser", filterData.Browser),
                new KeyValuePair<string, object>("OperatingSystem", filterData.OperatingSystem),
                new KeyValuePair<string, object>("SessionID", filterData.SessionID)
            };

            var columnsToRender = new List<string>()
            {
                // Add columns that you explicitly want to show
                // new KeyValuePair<string, object>("PersonalNo", "101")
            };

            string SPName = "[Administration].[USP_GlobalAccessTracking_Grid_Paged]";
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

