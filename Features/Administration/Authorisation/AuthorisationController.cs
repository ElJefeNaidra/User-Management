using Microsoft.AspNetCore.Mvc;
using static SIDPSF.Common.StringLocalisation.DatabaseResourceLocalisationProvider;
using static SIDPSF.Common.DataAccess.DBContext;
using SIDPSF.Common.DataAccess;
using SIDPSF.Common.Razor;

namespace SIDPSF.Features.Administration.Authorisation
{
    [Parent("Administration", "Administration", "Administrimi", "Administracija")]
    [TypeFilter(typeof(RequestAuthorisationFilter))]
    public class AuthorisationController : Controller
    {
        private const string SchemaName = "Administration";
        private const string TableName = "Authorisation";
        private const string ViewName = $"~/Features/{SchemaName}/{TableName}/{TableName}.cshtml";
        private const string SP = $"{SchemaName}.USP_{TableName}_";

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _resourceRepo;

        public AuthorisationController(DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _resourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }


        [ChildAction("Administration.Organisation.GetView", "Authorisations", "Autorizimet", "Autorizacije")]
        [IsMenuItem]
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            AuthorisationModel model = new AuthorisationModel();
            // Update common properties
            var operation = CrudOp.Read;


            string CrudOpForTitle = FormTranslationHelper.CrudOpForTitle(operation, _resourceRepo);
            string TableNameDisplay = FormTranslationHelper.TableNameDisplay("Authorisations");

            model.WindowTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            model.FormTitle = $"{TableNameDisplay} - {CrudOpForTitle}";
            model.BreadCrumbRoot = TableNameDisplay;
            model.BreadCrumbTitle = CrudOpForTitle.ToString();
            model.ControllerName = ControllerContext.RouteData.Values["controller"].ToString();

            AuthorisationModel filter = new AuthorisationModel();
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<AuthorisationModel>(SP + "GetAuthorisationTree", filter);

            if (ResponseInfo.HasError || Data == null)
            {
                return BadRequest(ResponseInfo.ErrorMessage);
            }

            return View(ViewName, (AuthorisationModel)Data);
        }
    }
}
