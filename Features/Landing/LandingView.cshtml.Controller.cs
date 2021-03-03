using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using UserManagement.Common.DataAccess;
using static UserManagement.Common.Attributes.Tools;
using static UserManagement.Common.StringLocalisation.DatabaseResourceLocalisationProvider;

namespace UserManagement.Features.SystemAdmin.LandingView
{
    public class LandingViewController : Controller
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _accessor;

        private readonly DBContext _dbContext;
        private readonly ResourceRepository _ResourceRepo;

        private const string SchemaName = "Landing";
        private const string TableName = "Landing";
        private const string ViewName = $"~/Features/{TableName}/{TableName}View.cshtml";

        public LandingViewController(string connectionString, IMemoryCache cache, IHttpContextAccessor accessor, DBContext dbContext, ResourceRepository ResourceRepo)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _ResourceRepo = ResourceRepo;
            _dbContext = dbContext;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetView()
        {
            AuthorisationInserter inserter = new(_connectionString);
            //inserter.InsertAuthorisationsForController(typeof(DashboardController));

            //inserter.InsertAuthorisationsForController(typeof(SyncStatusController));

            //inserter.InsertAuthorisationsForController(typeof(UserSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(UserController));
            //inserter.InsertAuthorisationsForController(typeof(UserAccessDeniedController));
            //inserter.InsertAuthorisationsForController(typeof(UserChangePasswordController));
            //inserter.InsertAuthorisationsForController(typeof(UserLanguageController));
            //inserter.InsertAuthorisationsForController(typeof(UserLogOutController));
            //inserter.InsertAuthorisationsForController(typeof(UserLogOutController));

            //inserter.InsertAuthorisationsForController(typeof(NonMatchingLoginSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(NonMatchingLoginController));

            ////inserter.InsertAuthorisationsForController(typeof(TransactionPagedSearchController));
            ////inserter.InsertAuthorisationsForController(typeof(TransactionController));

            //inserter.InsertAuthorisationsForController(typeof(CriteriaSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(PaymentTypeSimpleSearchController));

            //inserter.InsertAuthorisationsForController(typeof(CommunicationPagedSearchController));
            //inserter.InsertAuthorisationsForController(typeof(CommunicationController));

            //inserter.InsertAuthorisationsForController(typeof(LocalisationSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(LocalisationController));

            ////--inserter.InsertAuthorisationsForController(typeof(ReportController));
            ////inserter.InsertAuthorisationsForController(typeof(InstllmentSim));


            //inserter.InsertAuthorisationsForController(typeof(AuthorisationController));


            ////inserter.InsertAuthorisationsForController(typeof(CaseController));
            //inserter.InsertAuthorisationsForController(typeof(InstallmentSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(InstallmentController));

            //inserter.InsertAuthorisationsForController(typeof(CasePersonPagedSearchController));
            //inserter.InsertAuthorisationsForController(typeof(CasePersonController));

            //inserter.InsertAuthorisationsForController(typeof(RelationSimpleSearchController));


            //inserter.InsertAuthorisationsForController(typeof(TypeOfDecisionSimpleSearchController));

            //inserter.InsertAuthorisationsForController(typeof(TypeOfReimbursementSimpleSearchController));

            // Nomenclatures controllers

            //inserter.InsertAuthorisationsForController(typeof(TypeOfTransactionSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(TypeOfTransactionController));

            //inserter.InsertAuthorisationsForController(typeof(SchemeSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(SchemeController));

            //inserter.InsertAuthorisationsForController(typeof(TypeOfIncomeSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(TypeOfIncomeController));

            //inserter.InsertAuthorisationsForController(typeof(PersonPagedSearchController));
            //inserter.InsertAuthorisationsForController(typeof(PersonController));

            //inserter.InsertAuthorisationsForController(typeof(NationalitySimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(NationalityController));


            //inserter.InsertAuthorisationsForController(typeof(SettlementSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(SettlementController));

            //inserter.InsertAuthorisationsForController(typeof(MaritalStatusSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(MaritalStatusController));

            //inserter.InsertAuthorisationsForController(typeof(LanguageSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(LanguageController));

            //inserter.InsertAuthorisationsForController(typeof(CriteriaSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(CriteriaController));

            //inserter.InsertAuthorisationsForController(typeof(PaymentTypeSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(PaymentTypeController));

            //inserter.InsertAuthorisationsForController(typeof(RelationSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(RelationController));

            //inserter.InsertAuthorisationsForController(typeof(TypeOfDecisionSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(TypeOfDecisionController));

            //inserter.InsertAuthorisationsForController(typeof(TypeOfReimbursementSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(TypeOfReimbursementController));

            //inserter.InsertAuthorisationsForController(typeof(RelationSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(RelationController));

            //inserter.InsertAuthorisationsForController(typeof(CasePersonController));
            //inserter.InsertAuthorisationsForController(typeof(CaseReimbursementController));

            //inserter.InsertAuthorisationsForController(typeof(CasePersonDuplicatesController));

            //inserter.InsertAuthorisationsForController(typeof(CaseReimbursementSimpleSearchController));
            //inserter.InsertAuthorisationsForController(typeof(CaseReimbursementController));


            return View(ViewName);
        }
    }
}
