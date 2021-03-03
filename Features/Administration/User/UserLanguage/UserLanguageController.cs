using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SIDPSF.Common.DataAccess;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace SIDPSF.Features.Administration.User
{
    public class UserLanguageController : Controller
    {
        private const string UserGetMenuSP = "[Administration].[USP_User_GetMenu]";

        private readonly string _connectionString;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _accessor;

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        public UserLanguageController(string connectionString, IMemoryCache cache, IHttpContextAccessor accessor, DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> SetLanguageAsync(string IdLanguage, string RawUrl)
        {
            HttpContext.Session.SetString("IdLanguage", IdLanguage);

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("IdUser")))
            {
                // Load Authorisations/Permissions
                var filterParameters = new List<KeyValuePair<string, object>>()
                {
                // Add parameters to filter the stored procedure
                new KeyValuePair<string, object>("IdUser", HttpContext.Session.GetString("IdUser")),
                new KeyValuePair<string, object>("IdLanguage", HttpContext.Session.GetString("IdLanguage"))
                };

                // Get Menu Structure
                var DatatableResult = await _dbContext.QueryResultToDatatable(UserGetMenuSP, filterParameters);
                // Check if the DataTable has any rows
                if (DatatableResult.Rows.Count > 0)
                {
                    // Assuming the column contains string data and you want to store the first row's value
                    string menuStructure = DatatableResult.Rows[0]["MenuStructure"].ToString();
                    HttpContext.Session.SetString("MenuStructure", menuStructure);
                }
            }
            return Ok();
        }
    }
}
