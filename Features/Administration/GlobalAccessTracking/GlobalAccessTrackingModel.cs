using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common;
using SIDPSF.Common.Generics;
using SIDPSF.Common.StringLocalisation;
using SIDPSF.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace SIDPSF.Features.Administration.GlobalAccessTracking
{
    public partial class GlobalAccessTrackingModel : BaseCrudModel
    {

        #region IdGlobalAccessTracking
        [HiddenInput]
        public int IdGlobalAccessTracking { get; set; }
        #endregion

        #region DateOfRecord
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateOfRecord")]
        public DateTime? DateOfRecord { get; set; }
        #endregion

        #region GlobalAccessTrackingGUID
        [DbDisplayName("GlobalAccessTrackingGUID")]
        public string? GlobalAccessTrackingGUID { get; set; }
        #endregion

        #region IdUser
        [DbDisplayName("IdUser")]
        public int? IdUser { get; set; }

        private int? _IdUser;
        public GenericGetNonStandardDdlDataCollection IdUserDdl
        {
            get
            {
                _IdUser = IdUser;

                if (_IdUser.HasValue && _IdUser.Value != 0)
                {
                    return GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "IdUser=" + this.IdUser);
                }
                else
                {
                    // Return an empty GenericGetNonStandardDdlDataCollection
                    return new GenericGetNonStandardDdlDataCollection();
                }
            }
        }
        #endregion

        #region IdAuthorisation
        [DbDisplayName("IdAuthorisation")]
        public int? IdAuthorisation { get; set; }

        private int? _IdAuthorisation;
        public GenericGetDdlDataCollection IdAuthorisationDdl
        {
            get
            {
                _IdAuthorisation = IdAuthorisation;

                if (_IdAuthorisation.HasValue && _IdAuthorisation.Value != 0)
                {
                    return GenericGetDdlData.GetDdl("[Administration].[Authorisation]", "IdAuthorisation", "IdAuthorisation=" + this.IdAuthorisation);
                }
                else
                {
                    // Return an empty GenericGetNonStandardDdlDataCollection
                    return new GenericGetDdlDataCollection();
                }
            }
        }
        #endregion

        #region SPExecuted
        [DbDisplayName("SPExecuted")]
        public string? SPExecuted { get; set; }
        #endregion

        #region ControllerName
        [DbDisplayName("ControllerName")]
        public string? ControllerName { get; set; }
        #endregion

        #region ControllerAction
        [DbDisplayName("ControllerAction")]
        public string? ControllerAction { get; set; }
        #endregion

        #region QueryString
        [DbDisplayName("QueryString")]
        public string? QueryString { get; set; }
        #endregion

        #region IPAddress
        [DbDisplayName("IPAddress")]
        public string? IPAddress { get; set; }
        #endregion

        #region IsLocalIP
        [DbDisplayName("IsLocalIP")]
        public bool IsLocalIP { get; set; } = false;
        #endregion

        #region UserAgent
        [DbDisplayName("UserAgent")]
        public string? UserAgent { get; set; }
        #endregion

        #region Browser
        [DbDisplayName("Browser")]
        public string? Browser { get; set; }
        #endregion

        #region OperatingSystem
        [DbDisplayName("OperatingSystem")]
        public string? OperatingSystem { get; set; }
        #endregion

        #region Referrer
        [DbDisplayName("Referrer")]
        public string? Referrer { get; set; }
        #endregion

        #region Languages
        [DbDisplayName("Languages")]
        public string? Languages { get; set; }
        #endregion

        #region IsHttps
        [DbDisplayName("IsHttps")]
        public bool IsHttps { get; set; } = false;
        #endregion

        #region Protocol
        [DbDisplayName("Protocol")]
        public string? Protocol { get; set; }
        #endregion

        #region SessionID
        [DbDisplayName("SessionID")]
        public string? SessionID { get; set; }
        #endregion

        #region SessionContent
        [DataType(DataType.MultilineText)]
        [DbDisplayName("SessionContent")]
        public string? SessionContent { get; set; }
        #endregion

        #region RequestInfo
        [DataType(DataType.MultilineText)]
        [DbDisplayName("RequestInfo")]
        public string? RequestInfo { get; set; }
        #endregion

        #region Control Display of Fields

        public string display_IdGlobalAccessTracking { get; set; } = "";
        public string display_DateOfRecord { get; set; } = "";
        public string display_GlobalAccessTrackingGUID { get; set; } = "";
        public string display_IdUser { get; set; } = "";
        public string display_IdAuthorisation { get; set; } = "";
        public string display_SPExecuted { get; set; } = "";
        public string display_ControllerName { get; set; } = "";
        public string display_ControllerAction { get; set; } = "";
        public string display_QueryString { get; set; } = "";
        public string display_IPAddress { get; set; } = "";
        public string display_IsLocalIP { get; set; } = "";
        public string display_UserAgent { get; set; } = "";
        public string display_Browser { get; set; } = "";
        public string display_OperatingSystem { get; set; } = "";
        public string display_Referrer { get; set; } = "";
        public string display_Languages { get; set; } = "";
        public string display_IsHttps { get; set; } = "";
        public string display_Protocol { get; set; } = "";
        public string display_SessionID { get; set; } = "";
        public string display_SessionContent { get; set; } = "";
        public string display_RequestInfo { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields

        public bool disable_IdGlobalAccessTracking { get; set; } = false;
        public bool disable_DateOfRecord { get; set; } = false;
        public bool disable_GlobalAccessTrackingGUID { get; set; } = false;
        public bool disable_IdUser { get; set; } = false;
        public bool disable_IdAuthorisation { get; set; } = false;
        public bool disable_SPExecuted { get; set; } = false;
        public bool disable_ControllerName { get; set; } = false;
        public bool disable_ControllerAction { get; set; } = false;
        public bool disable_QueryString { get; set; } = false;
        public bool disable_IPAddress { get; set; } = false;
        public bool disable_IsLocalIP { get; set; } = false;
        public bool disable_UserAgent { get; set; } = false;
        public bool disable_Browser { get; set; } = false;
        public bool disable_OperatingSystem { get; set; } = false;
        public bool disable_Referrer { get; set; } = false;
        public bool disable_Languages { get; set; } = false;
        public bool disable_IsHttps { get; set; } = false;
        public bool disable_Protocol { get; set; } = false;
        public bool disable_SessionID { get; set; } = false;
        public bool disable_SessionContent { get; set; } = false;
        public bool disable_RequestInfo { get; set; } = false;
        #endregion

        // Remote validation methods that are implemented in THIS
        public partial class GlobalAccessTrackingController : Controller
        {
            public IActionResult PlaceHolder()
            {
                return null;
            }
        }
    }
}