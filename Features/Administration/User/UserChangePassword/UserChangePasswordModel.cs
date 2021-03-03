using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIDPSF.Common.StringLocalisation;
using SIDPSF.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace SIDPSF.Features.Administration.User.UserChangePassword
{
    public partial class UserChangePasswordModel : BaseCrudModel
    {
        #region IdUser
        [HiddenInput]
        public int IdUser { get; set; }
        #endregion

        #region Password
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DataType(DataType.Password)]
        [StringLength(25, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Password")]
        public string? Password { get; set; }
        #endregion

        #region NewPassword
        [Remote(action: "ValidatePasswordStrength", controller: "UserChangePassword", AdditionalFields = nameof(Password), ErrorMessage = "New password must be different from the current password and meet the strength requirements.")]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DataType(DataType.Password)]
        [StringLength(25, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("NewPassword")]
        public string? NewPassword { get; set; }
        #endregion

        #region ConfirmNewPassword
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceName = "ValidationPasswordsDoNotMatch", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(25, MinimumLength = 6, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("ConfirmNewPassword")]
        public string? ConfirmNewPassword { get; set; }
        #endregion
    }

}