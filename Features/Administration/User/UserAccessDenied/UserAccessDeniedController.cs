using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.DataAccess;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace SIDPSF.Features.Administration.User.UserAccessDenied
{
    [TypeFilter(typeof(RequestAuthorisationBasicFilter))]
    public partial class UserAccessDeniedController : Controller
    {
        private const string AccessDeniedViewPath = $"~/Features/Administration/User/UserAccessDenied/UserAccessDenied.cshtml";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IHttpContextAccessor _accessor;

        public UserAccessDeniedController(DBContext dbContext, ResourceRepository resourceRepo, IHttpContextAccessor accessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _resourceRepo = resourceRepo ?? throw new ArgumentNullException(nameof(resourceRepo));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            TempData["ErrorMessages"] = _resourceRepo.GetResourceByName("MsgAccessDenied");
            return View(AccessDeniedViewPath);
        }
    }
}


