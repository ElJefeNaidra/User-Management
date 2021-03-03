using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SIDPSF.Common;
using SIDPSF.Common.Generics;
using SIDPSF.Common.StringLocalisation;
using SIDPSF.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.Administration.User
{
    public partial class UserModel : BaseCrudModel
    {
        
        #region IdUser
        [HiddenInput]
        public int IdUser { get; set; }
        #endregion

        #region IsSuperAdmin
        [DbDisplayName("IsSuperAdmin")]
        public bool IsSuperAdmin { get; set; } = false;
        #endregion

        #region IsSystemProceess
        [DbDisplayName("IsSystemProceess")]
        public bool IsSystemProceess { get; set; } = false;
        #endregion

        #region IdOrganisation
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IdOrganisation")]
        public int IdOrganisation { get; set; }

        [JsonIgnore]
        public GenericGetDdlDataCollection IdOrganisationDdl
        {
            get
            {
                if (this.crudOp == CrudOp.Create || this.crudOp == CrudOp.Update)
                {
                    var obj = GenericGetDdlData.GetDdl("[Administration].[Organisation]", "IdOrganisation", "");
                    return obj;
                }
                else
                {
                    var obj = GenericGetDdlData.GetDdl("[Administration].[Organisation]", "IdOrganisation", "IdOrganisation=" + this.IdOrganisation);
                    return obj;
                }
            }
        }
        #endregion

        #region PersonalNo
        [Remote(action: "ValidatePersonalNoDuplicate", controller: "User")]
        [ValidationAttribute_RKSID]
        [RegularExpression(@"^(1[0-9]|2[0-9])[0-9]{8}$", ErrorMessageResourceName = "ValidationKosovoPersonalNo", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(10, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 10)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("PersonalNo")]
        public string PersonalNo { get; set; }
        #endregion

        #region FirstName
        [StringLength(50, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("FirstName")]
        public string FirstName { get; set; }
        #endregion

        #region LastName
        [StringLength(50, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("LastName")]
        public string LastName { get; set; }
        #endregion

        #region FileUploadPersonalData
        // Actual property holding the file path

        [DbDisplayName("FileUploadPersonalData")]
        public string? FileUploadPersonalData { get; set; }

        [ValidationAttribute_FileTypeMaxSize(5000, new string[] { ".docx", ".png", ".pdf" })]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("FileUploadPersonalData")]
        public IFormFile FileUploadPersonalDataInterface { get; set; }

        #endregion

        #region FileUploadPersonalDataUpdateUpdate
        [ValidationAttribute_FileTypeMaxSize(5000, new string[] { ".docx", ".png", ".pdf" })]
        [DbDisplayName("FileUploadPersonalData")]
        public IFormFile? FileUploadPersonalDataUpdateInterface { get; set; }
        #endregion

        #region Email
        [Remote(action: "ValidateEmailDuplicate", controller: "User")]
        [EmailAddress(ErrorMessageResourceName = "ValidationEmail", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessageResourceName = "ValidationEmail", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(100, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 6)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Email")]
        public string Email { get; set; }
        #endregion

        #region PhoneNo
        [RegularExpression(@"^04[3-9](1[0-9]{5}|[2-9][0-9]{5})$", ErrorMessageResourceName = "ValidationKosovoPhoneNo", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(9, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 9)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("PhoneNo")]
        public string PhoneNo { get; set; }
        #endregion

        #region WorkTitle
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("WorkTitle")]
        public string WorkTitle { get; set; }
        #endregion

        #region IdLanguageOfUser
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IdLanguageOfUser")]
        public int IdLanguageOfUser { get; set; }

        [JsonIgnore]
        public GenericGetDdlDataCollection IdLanguageOfUserDdl
        {
            get
            {
                // Initialize the collection
                GenericGetDdlDataCollection obj = new GenericGetDdlDataCollection();

                // Add items to the collection
                obj.Add(new GenericGetDdlDataModel { Id = 1, Description = "English" });
                obj.Add(new GenericGetDdlDataModel { Id = 2, Description = "Albanian" });
                obj.Add(new GenericGetDdlDataModel { Id = 3, Description = "Serbian" });

                // Return the populated collection
                return obj;
            }
        }

        #endregion

        #region Username
        [StringLength(30, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 8)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression("^[a-z0-9]+[.][a-z0-9]+$", ErrorMessageResourceName = "ValidationUsername", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Username")]
        public string Username { get; set; }
        #endregion

        #region Password
        [DbDisplayName("Password")]
        public string? Password { get; set; }
        #endregion

        #region TemporaryPasswordText
        [DbDisplayName("TemporaryPasswordText")]
        public string? TemporaryPasswordText { get; set; }
        #endregion

        #region IdAuthorisationArray
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IdAuthorisationArrayValues")]
        public List<int> IdAuthorisationArrayValues { get; set; }
        public List<SelectListItem>? IdAuthorisationArrayList
        {
            get
            {
                var selectListItems = new List<SelectListItem>();

                if (!string.IsNullOrEmpty(IdAuthorisationArray))
                {
                    var selectedIds = IdAuthorisationArray.Split(',')
                                            .Select(int.Parse)
                                            .ToList();

                    // Assuming IdAuthorisationArrayDdl is already populated with the items
                    foreach (var item in IdAuthorisationArrayDdl)
                    {
                        selectListItems.Add(new SelectListItem
                        {
                            Value = item.Id.ToString(),
                            Text = item.Description,
                            Selected = selectedIds.Contains(item.Id)
                        });
                    }
                }
                else
                {
                    foreach (var item in IdAuthorisationArrayDdl)
                    {
                        selectListItems.Add(new SelectListItem
                        {
                            Value = item.Id.ToString(),
                            Text = item.Description,
                            Selected = false
                        });
                    }
                }


                return selectListItems;
            }
            set { }
        }
        public GenericGetDdlDataCollection IdAuthorisationArrayDdl
        {
            get
            {
                var obj = GenericGetDdlData.GetDdl("[Administration].[Authorisation]", "IdAuthorisation", "");
                return obj;
            }
        }

        //The actual array values holder from database column in 1,2,3,4..... format
        [HiddenInput]
        public string? IdAuthorisationArray
        {
            get => _IdAuthorisationArray;
            set
            {
                // On setting IdAuthorisationArray, join the values from IdAuthorisationArrayValues
                if (IdAuthorisationArrayValues != null && IdAuthorisationArrayValues.Any())
                {
                    _IdAuthorisationArray = String.Join(",", IdAuthorisationArrayValues);
                }
                else
                {
                    _IdAuthorisationArray = value;
                }
            }
        }

        private string? _IdAuthorisationArray;
        #endregion

        #region IsEnabled
        [DbDisplayName("IsEnabled")]
        public bool IsEnabled { get; set; } = false;
        #endregion

        #region EnabledDisabledComment
        [DataType(DataType.MultilineText)]
        [StringLength(300, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 5)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("EnabledDisabledComment")]
        public string? EnabledDisabledComment { get; set; }
        #endregion

        #region DateEnabledDisabled
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateEnabledDisabled")]
        public DateTime? DateEnabledDisabled { get; set; }
        #endregion

        #region IsLockedOut
        [DbDisplayName("IsLockedOut")]
        public bool IsLockedOut { get; set; } = false;
        #endregion

        #region DateLockedOut
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateLockedOut")]
        public DateTime? DateLockedOut { get; set; }
        #endregion

        #region DateLockOutExpired
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateLockOutExpired")]
        public DateTime? DateLockOutExpired { get; set; }
        #endregion

        #region IsLoggedIn
        [DbDisplayName("IsLoggedIn")]
        public bool IsLoggedIn { get; set; } = false;
        #endregion

        #region DateLoggedIn
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateLoggedIn")]
        public DateTime? DateLoggedIn { get; set; }
        #endregion

        #region CurrentSessionID
        [DbDisplayName("CurrentSessionID")]
        public string? CurrentSessionID { get; set; }
        #endregion

        #region PasswordNeedsChange
        [DbDisplayName("PasswordNeedsChange")]
        public bool PasswordNeedsChange { get; set; } = false;
        #endregion

        public bool _CtrlCanDisable { get; set; }
        public bool _CtrlCanEnable { get; set; }
        public bool _CtrlCanLogIn { get; set; }
        public bool _CtrlCanUpdate { get; set; }
        public bool _CtrlCanLogOut { get; set; }
        public bool _CtrlCanPasswordResetAdmin { get; set; }
        public bool _CtrlCanUserChangePassword { get; set; }

        #region Control Display of Fields
        public string display_IsSuperAdmin { get; set; } = "";
        public string display_IsSystemProceess { get; set; } = "";
        public string display_IdOrganisation { get; set; } = "";
        public string display_PersonalNo { get; set; } = "";
        public string display_FirstName { get; set; } = "";
        public string display_LastName { get; set; } = "";
        public string display_FileUploadPersonalData { get; set; } = "";
        public string display_Email { get; set; } = "";
        public string display_PhoneNo { get; set; } = "";
        public string display_WorkTitle { get; set; } = "";
        public string display_IdLanguageOfUser { get; set; } = "";
        public string display_Username { get; set; } = "";
        public string display_Password { get; set; } = "";
        public string display_TemporaryPasswordText { get; set; } = "";
        public string display_IdAuthorisationArray { get; set; } = "";
        public string display_IsEnabled { get; set; } = "";
        public string display_EnabledDisabledComment { get; set; } = "";
        public string display_DateEnabledDisabled { get; set; } = "";
        public string display_IsLockedOut { get; set; } = "";
        public string display_DateLockedOut { get; set; } = "";
        public string display_DateLockOutExpired { get; set; } = "";
        public string display_IsLoggedIn { get; set; } = "";
        public string display_DateLoggedIn { get; set; } = "";
        public string display_CurrentSessionID { get; set; } = "";
        public string display_PasswordNeedsChange { get; set; } = "";
        public string display__CtrlCanDisable { get; set; } = "";
        public string display__CtrlCanEnable { get; set; } = "";
        public string display__CtrlCanLogIn { get; set; } = "";
        public string display__CtrlCanUpdate { get; set; } = "";
        public string display__CtrlCanLogOut { get; set; } = "";
        public string display__CtrlCanPasswordResetAdmin { get; set; } = "";
        public string display__CtrlCanUserChangePassword { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields
       public bool disable_IsSuperAdmin { get; set; } = false;
        public bool disable_IsSystemProceess { get; set; } = false;
        public bool disable_IdOrganisation { get; set; } = false;
        public bool disable_PersonalNo { get; set; } = false;
        public bool disable_FirstName { get; set; } = false;
        public bool disable_LastName { get; set; } = false;
        public bool disable_FileUploadPersonalData { get; set; } = false;
        public bool disable_Email { get; set; } = false;
        public bool disable_PhoneNo { get; set; } = false;
        public bool disable_WorkTitle { get; set; } = false;
        public bool disable_IdLanguageOfUser { get; set; } = false;
        public bool disable_Username { get; set; } = false;
        public bool disable_Password { get; set; } = false;
        public bool disable_TemporaryPasswordText { get; set; } = false;
        public bool disable_IdAuthorisationArray { get; set; } = false;
        public bool disable_IsEnabled { get; set; } = false;
        public bool disable_EnabledDisabledComment { get; set; } = false;
        public bool disable_DateEnabledDisabled { get; set; } = false;
        public bool disable_IsLockedOut { get; set; } = false;
        public bool disable_DateLockedOut { get; set; } = false;
        public bool disable_DateLockOutExpired { get; set; } = false;
        public bool disable_IsLoggedIn { get; set; } = false;
        public bool disable_DateLoggedIn { get; set; } = false;
        public bool disable_CurrentSessionID { get; set; } = false;
        public bool disable_PasswordNeedsChange { get; set; } = false;
        public bool disable__CtrlCanDisable { get; set; } = false;
        public bool disable__CtrlCanEnable { get; set; } = false;
        public bool disable__CtrlCanLogIn { get; set; } = false;
        public bool disable__CtrlCanUpdate { get; set; } = false;
        public bool disable__CtrlCanLogOut { get; set; } = false;
        public bool disable__CtrlCanPasswordResetAdmin { get; set; } = false;
        public bool disable__CtrlCanUserChangePassword { get; set; } = false;
        #endregion

        // Remote validation methods that are implemented in THIS
        public partial class UserController : Controller
        {
            public IActionResult PlaceHolder()
            {
                return null;
            }
        }
    }
}