using UserManagement.Common.Generics;
using UserManagement.Common.Grid;
using UserManagement.Common.StringLocalisation;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace UserManagement.Features.Administration.GlobalAccessTracking
{
    // Implementation of Search Model for Simple no ServerOperation Grids
    public class GlobalAccessTrackingPagedSearchModel
    {
        #region GUI Stuff
        public string WindowTitle { get; set; }
        public string FormTitle { get; set; }
        public string BreadCrumbRoot { get; set; }
        public string BreadCrumbTitle { get; set; }
        public string FormControllerName { get; set; }
        public string FormActionName { get; set; }
        #endregion


        // Property for the DataTable to hold the data
        public DataTable GlobalAccessTrackingData { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicGlobalAccessTrackingData()
        {
            return Converters.ConvertDataTableToDynamic(GlobalAccessTrackingData);
        }

        // Total Rows
        public int Total { get; set; }

        #region Filtering properties

        #region DateOfRecord
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessageResourceName = "ValidationDateFormatWrong", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("DateOfRecord")]
        public DateTime? DateOfRecord { get; set; }
        #endregion

        #region GlobalAccessTrackingGUID
        [StringLength(32, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessageResourceName = "ValidationAlphanumeric", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("GlobalAccessTrackingGUID")]
        public string? GlobalAccessTrackingGUID { get; set; }
        #endregion

        #region IdUser
        [DbDisplayName("IdUser")]
        public int? IdUser { get; set; }

        public GenericGetNonStandardDdlDataCollection IdUserDdl
        {
            get
            {
                var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "");
                return obj;
            }
        }
        #endregion

        #region IdAuthorisation
        [DbDisplayName("IdAuthorisation")]
        public int? IdAuthorisation { get; set; }

        public GenericGetDdlDataCollection IdAuthorisationDdl
        {
            get
            {
                    var obj = GenericGetDdlData.GetDdl("[Administration].[Authorisation]", "IdAuthorisation", "");
                    return obj;
            }
        }
        #endregion


        #region ControllerName
        [StringLength(50, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessageResourceName = "ValidationAlphanumeric", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("ControllerName")]
        public string? ControllerName { get; set; }
        #endregion

        #region ControllerAction
        [StringLength(50, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessageResourceName = "ValidationAlphanumeric", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("ControllerAction")]
        public string? ControllerAction { get; set; }
        #endregion

        #region IPAddress
        [StringLength(15, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [RegularExpression("^[0-9.]+$", ErrorMessageResourceName = "ValidationIPFormat", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IPAddress")]
        public string? IPAddress { get; set; }
        #endregion

        #region UserAgent
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [DbDisplayName("UserAgent")]
        public string? UserAgent { get; set; }
        #endregion

        #region Browser
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [DbDisplayName("Browser")]
        public string? Browser { get; set; }
        #endregion

        #region OperatingSystem
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [DbDisplayName("OperatingSystem")]
        public string? OperatingSystem { get; set; }
        #endregion

        #region SessionID
        [StringLength(50, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessageResourceName = "ValidationAlphanumeric", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("SessionID")]
        public string? SessionID { get; set; }
        #endregion


        #region Control Display of Fields

        public string display_DateOfRecord { get; set; } = "";
        public string display_GlobalAccessTrackingGUID { get; set; } = "";
        public string display_IdUser { get; set; } = "";
        public string display_IdAuthorisation { get; set; } = "";
        public string display_ControllerName { get; set; } = "";
        public string display_ControllerAction { get; set; } = "";
        public string display_IPAddress { get; set; } = "";
        public string display_IsLocalIP { get; set; } = "";
        public string display_UserAgent { get; set; } = "";
        public string display_Browser { get; set; } = "";
        public string display_OperatingSystem { get; set; } = "";
        public string display_SessionID { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields
        public bool disable_DateOfRecord { get; set; } = false;
        public bool disable_GlobalAccessTrackingGUID { get; set; } = false;
        public bool disable_IdUser { get; set; } = false;
        public bool disable_IdAuthorisation { get; set; } = false;
        public bool disable_ControllerName { get; set; } = false;
        public bool disable_ControllerAction { get; set; } = false;
        public bool disable_IPAddress { get; set; } = false;
        public bool disable_IsLocalIP { get; set; } = false;
        public bool disable_UserAgent { get; set; } = false;
        public bool disable_Browser { get; set; } = false;
        public bool disable_OperatingSystem { get; set; } = false;
        public bool disable_SessionID { get; set; } = false;
        #endregion

        #endregion
    }
}
