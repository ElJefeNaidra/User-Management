using Newtonsoft.Json;
using UserManagement.Common.StringLocalisation;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Features.Administration.User.UserLogin
{
    public class UserLoginModel
    {
        #region Username
        [StringLength(50, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression("^[a-zA-Z0-9.]+$", ErrorMessageResourceName = "ValidationUsername", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Username")]
        public string Username { get; set; }
        #endregion

        #region Password
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DataType(DataType.Password)]
        [DbDisplayName("Password")]
        public string Password { get; set; }
        #endregion

        public int MaxNumberOfFailedLoginsBeforeLockout { get; set; }
        public int MaxNumberOfFailedLoginsBeforeLockoutPeriodBetweenTriesMinutes { get; set; }
        public int UnlockLockoutStatusAutomaticallyAfterMinutes { get; set; }
        public int LoggedInUserNoActivityPeriodMinutes { get; set; }
    }

    public class UserStatusModel
    {
        public int IdUser { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}