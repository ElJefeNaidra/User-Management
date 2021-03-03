
namespace SIDPSF.Features.Administration.Authorisation
{
    public partial class AuthorisationModel
    {
        #region Model Configuration
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }

        public string? WindowTitle { get; set; }
        public string? FormTitle { get; set; }
        public string? BreadCrumbRoot { get; set; }
        public string? BreadCrumbTitle { get; set; }
        public string? PageDescription { get; set; }

        #endregion
       public string? TreeView { get; set; }
    }
}
