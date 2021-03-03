using Microsoft.AspNetCore.Mvc;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Common.DataAccess.DBContext;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.Razor;

namespace SIDPSF.Features.CommonRegistry.Settlement
{
    [Parent("CommonRegistry", "Common Registries", "Regjistrat", "Registri")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class SettlementController : Controller
    {
        private const string ReturnUrl = "/SettlementSimpleSearch/GetView";
        private const string SchemaName = "CommonRegistry";
        private const string TableName = "Settlement";
        private const string ViewPath = $"~/Features/{SchemaName}/{TableName}/{TableName}.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;

        public SettlementController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }
        private void SetupModel(SettlementModel model, CrudOp operation)
        {
            // Update common properties
            model.crudOp = operation;

            string CrudOpForTitle = FormTranslationHelper.CrudOpForTitle(operation, _resourceRepo);
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Settlement");

            model.WindowTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            model.FormTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            model.BreadCrumbRoot = TableNameDisplay;
            model.BreadCrumbTitle = CrudOpForTitle.ToString();
            model.ControllerName = ControllerContext.RouteData.Values["controller"].ToString();
            model.ActionName = operation.ToString();
            model.ReturnUrl = ReturnUrl;
        }
    }
}

