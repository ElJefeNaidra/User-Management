using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserManagement.Common.Generics;
using UserManagement.Common.StringLocalisation;
using UserManagement.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Features.Administration.NonMatchingLogin
{
    public partial class NonMatchingLoginModel : BaseCrudModel
    {

        #region IdNonMatchingLogin
        [HiddenInput]
        public int IdNonMatchingLogin { get; set; }
        #endregion

        #region DateOfAttempt
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateOfAttempt")]
        public DateTime? DateOfAttempt { get; set; }
        #endregion

        #region IdUser
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
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

        #region Username
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Username")]
        public string Username { get; set; }
        #endregion

        #region Password
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Password")]
        public string Password { get; set; }
        #endregion

        #region SessionID
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("SessionID")]
        public string SessionID { get; set; }
        #endregion

        #region IpAddress
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IpAddress")]
        public string IpAddress { get; set; }
        #endregion

        #region IsLocalIP
        [DbDisplayName("IsLocalIP")]
        public bool IsLocalIP { get; set; } = false;
        #endregion

        #region CountryName
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [DbDisplayName("CountryName")]
        public string? CountryName { get; set; }
        #endregion

        #region RegionName
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 0)]
        [DbDisplayName("RegionName")]
        public string? RegionName { get; set; }
        #endregion

        #region DeviceType
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("DeviceType")]
        public string DeviceType { get; set; }
        #endregion

        #region BrowserName
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("BrowserName")]
        public string BrowserName { get; set; }
        #endregion

        #region BrowserVersion
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("BrowserVersion")]
        public string BrowserVersion { get; set; }
        #endregion

        #region OperatingSystem
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("OperatingSystem")]
        public string OperatingSystem { get; set; }
        #endregion


        #region Control Display of Fields

        public string display_IdNonMatchingLogin { get; set; } = "";
        public string display_DateOfAttempt { get; set; } = "";
        public string display_IdUser { get; set; } = "";
        public string display_Username { get; set; } = "";
        public string display_Password { get; set; } = "";
        public string display_SessionID { get; set; } = "";
        public string display_IpAddress { get; set; } = "";
        public string display_IsLocalIP { get; set; } = "";
        public string display_CountryName { get; set; } = "";
        public string display_RegionName { get; set; } = "";
        public string display_DeviceType { get; set; } = "";
        public string display_BrowserName { get; set; } = "";
        public string display_BrowserVersion { get; set; } = "";
        public string display_OperatingSystem { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields

        public bool disable_IdNonMatchingLogin { get; set; } = false;
        public bool disable_DateOfAttempt { get; set; } = false;
        public bool disable_IdUser { get; set; } = false;
        public bool disable_Username { get; set; } = false;
        public bool disable_Password { get; set; } = false;
        public bool disable_SessionID { get; set; } = false;
        public bool disable_IpAddress { get; set; } = false;
        public bool disable_IsLocalIP { get; set; } = false;
        public bool disable_CountryName { get; set; } = false;
        public bool disable_RegionName { get; set; } = false;
        public bool disable_DeviceType { get; set; } = false;
        public bool disable_BrowserName { get; set; } = false;
        public bool disable_BrowserVersion { get; set; } = false;
        public bool disable_OperatingSystem { get; set; } = false;
        #endregion

        // Remote validation methods that are implemented in THIS
        public partial class NonMatchingLoginController : Controller
        {
            public IActionResult PlaceHolder()
            {
                return null;
            }
        }
    }
}