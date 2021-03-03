using Microsoft.AspNetCore.Mvc;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static UserManagement.Common.DataAccess.DBContext;
using UserManagement.Common.DataAccess;
using UserManagement.Common.Razor;

namespace UserManagement.Features.Administration.Localisation
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class LocalisationController : Controller
    {
        private const string ReturnUrl = "/LocalisationSimpleSearch/GetView";
        private const string SchemaName = "Administration";
        private const string TableName = "Localisation";
        private const string ViewPath = $"~/Features/{SchemaName}/{TableName}/{TableName}.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;

        public LocalisationController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }
        private void SetupModel(LocalisationModel model, CrudOp operation)
        {
            // Update common properties
            model.crudOp = operation;

            string CrudOpForTitle = FormTranslationHelper.CrudOpForTitle(operation, _resourceRepo);
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Localisation");

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

