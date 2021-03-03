using Microsoft.AspNetCore.Mvc;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Common.DataAccess.DBContext;
using static SIDPSF.Features.Administration.ConfigurationProvider;
using SIDPSF.Common.DataAccess;

namespace SIDPSF.Features.Administration.User
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public partial class UserController : Controller
    {
        private const string ReturnUrl = "/UserSimpleSearch/GetView";
        private const string SchemaName = "Administration";
        private const string TableName = "User";
        private const string ViewPath = $"~/Features/Administration/User/{TableName}.cshtml";
        private const string ProfileViewPath = $"~/Features/Administration/User/UserProfile/{TableName}Profile.cshtml";
        private const string LoginViewPath = $"~/Features/Administration/User/UserLogin/{TableName}Login.cshtml";


        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;
        private readonly SystemConfigService _configService;

        public UserController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor, SystemConfigService configService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }
        private async void SetupModel(UserModel model, CrudOp operation)
        {
            // Update common properties
            model.crudOp = operation;
            model.WindowTitle = $"{TableName} {operation}";
            model.FormTitle = $"{TableName} {operation}";
            model.BreadCrumbRoot = TableName;
            model.BreadCrumbTitle = operation.ToString();
            model.ControllerName = ControllerContext.RouteData.Values["controller"].ToString();
            model.ActionName = operation.ToString();
            model.ReturnUrl = ReturnUrl;
        }
    }
}

