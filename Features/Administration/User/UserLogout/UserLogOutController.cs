using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.RequestFilters;
using System.Data;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace SIDPSF.Features.Administration.User
{
    [TypeFilter(typeof(RequestAuthorisationBasicFilter))]
    public class UserLogOutController : Controller
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _accessor;
        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;
        private readonly IOptions<SessionOptions> _sessionOptions;
        private readonly IUserAgentService _userAgentService;

        public UserLogOutController(string connectionString, IMemoryCache cache, IHttpContextAccessor accessor, DBContext dbContext, ResourceRepository ResourceRepo, IOptions<SessionOptions> SsessionOptions, IUserAgentService userAgentService)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _resourceRepo = ResourceRepo ?? throw new ArgumentNullException(nameof(ResourceRepo));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _sessionOptions = SsessionOptions ?? throw new ArgumentNullException(nameof(dbContext));
            _userAgentService = userAgentService ?? throw new ArgumentNullException(nameof(userAgentService));
        }


        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            UserModel model = new UserModel();

            model.IdUser = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));

            UserAgentInfoModel userAgentInfoModel = new UserAgentInfoModel();
            userAgentInfoModel = _userAgentService.GetUserAgentInfo(_accessor.HttpContext);

            model._IpAddress = userAgentInfoModel.IPAddress;
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");

            string SPName = "[Administration].[USP_User_LogOut]";

            var (ResponseInfo, Data) = await _dbContext.UpdateAsync(SPName, model);

            _accessor.HttpContext.Session.Clear();
            var keys = new List<string>();

            // Collect all session keys
            foreach (var key in _accessor.HttpContext.Session.Keys)
            {
                keys.Add(key);
            }

            // Remove all keys collected from the session
            foreach (var key in keys)
            {
                _accessor.HttpContext.Session.Remove(key);
            }

            var idlanguage = _accessor.HttpContext.Session.GetInt32("IdLanguage");
            _accessor.HttpContext.Response.Cookies.Delete(_sessionOptions.Value.Cookie.Name);

            // After logging out, you should redirect the user to a login page or home page
            return RedirectToAction("Login", "UserLogin"); // Adjust the redirection as needed
        }
    }
}

