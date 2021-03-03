using Microsoft.AspNetCore.Mvc;
using UserManagement.Common.Razor;
using static UserManagement.Common.DataAccess.DBContext;

namespace UserManagement.Features.Administration.Organisation
{
    public partial class OrganisationController : Controller
    {
        #region READ

        [ChildAction("Administration.Organisation.Read", "Organisation - Read", "Organizata - Lexim", "Organizacija - Čitanje")]
        [HttpGet]
        public async Task<IActionResult> Read(int Id, string _RowGuid)
        {
            // Are parameters sent in?
            if ((Id == 0 || Id == null) || _RowGuid is null)
                return BadRequest("Invalid request.");

            OrganisationModel model = new OrganisationModel();

            // Filtering the dataset
            model.IdOrganisation = Id;// SET MODEL ID PK FOR FILTERING
            model._RowGuid = _RowGuid;// Security Protocol Impelementation with RowGuid

            // Addding the standard tracking operation values
            model._IdUserOperation = Convert.ToInt32(_accessor.HttpContext.Session.GetString("IdUser"));
            model._GlobalAccessTrackingGUID = _accessor.HttpContext.Session.GetString("GlobalAccessTrackingGUID");
            model._IpAddress = _accessor.HttpContext.Session.GetString("IpAddress");

            // EXEC the SP
            string SPName = $"[{SchemaName}].[USP_{TableName}_Read]";
            var (ResponseInfo, Data) = await _dbContext.ReadFilteredAsync<OrganisationModel>(SPName, model);

            model = Data;
            if (model is not null)
            {
                // Setup the model for READ
                SetupModel(model, CrudOp.Read);

                // Disable all form controls
                ModelDisableFields.SetDisablePropertiesToTrue(model); //Make fields disabled

                model._AllowedOperations = new List<string>(); // Ensure the list is initialized.

                // Add allowed operations
                if (model._CtrlCanUpdate && model.IdOrganisation != 0 && !string.IsNullOrEmpty(model._RowGuid))
                {
                    string UpdateLink = $"<a href='/Organisation/Update?Id={model.IdOrganisation}&_RowGuid={model._RowGuid}&_RowTimestamp={model._RowTimestamp}'>" + _resourceRepo.GetResourceByName("Update") + "</a>";
                    model._AllowedOperations.Add(UpdateLink);
                }

                if (model._CtrlCanDisable && model.IdOrganisation != 0 && !string.IsNullOrEmpty(model._RowGuid))
                {
                    string DisableLink = $"<a href='/Organisation/Disable?Id={model.IdOrganisation}&_RowGuid={model._RowGuid}&_RowTimestamp={model._RowTimestamp}'>" + _resourceRepo.GetResourceByName("Disable") + "</a>";
                    model._AllowedOperations.Add(DisableLink);
                }

                if (model._CtrlCanEnable && model.IdOrganisation != 0 && !string.IsNullOrEmpty(model._RowGuid))
                {
                    string EnableLink = $"<a href='/Organisation/Enable?Id={model.IdOrganisation}&_RowGuid={model._RowGuid}&_RowTimestamp={model._RowTimestamp}'>" + _resourceRepo.GetResourceByName("Enable") + "</a>";
                    model._AllowedOperations.Add(EnableLink);
                }

                // Get history data
                await GetHistoryGrid(Id, model);

                return View(ViewPath, model);
            }
            else
            {
                return BadRequest("Invalid request.");
            }
        }
        #endregion
    }
}

