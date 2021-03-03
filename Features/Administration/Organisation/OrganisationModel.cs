using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using UserManagement.Common.Generics;
using UserManagement.Common.StringLocalisation;
using UserManagement.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Features.Administration.Organisation
{
    public partial class OrganisationModel : BaseCrudModel
    {

        #region IdOrganisation
        [HiddenInput]
        public int IdOrganisation { get; set; }
        #endregion

        #region EnDescription
        [StringLength(100, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("EnDescription")]
        public string EnDescription { get; set; }
        #endregion

        #region SqDescription
        [StringLength(100, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("SqDescription")]
        public string SqDescription { get; set; }
        #endregion

        #region SrDescription
        [StringLength(100, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("SrDescription")]
        public string SrDescription { get; set; }
        #endregion

        #region Address
        [StringLength(100, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Address")]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        public string Address { get; set; }
        #endregion

        #region Email
        [EmailAddress(ErrorMessageResourceName = "ValidationEmail", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessageResourceName = "ValidationEmail", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(100, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 6)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Email")]
        public string Email { get; set; }
        #endregion

        #region Phone
        [RegularExpression(@"^04[3-9](1[0-9]{5}|[2-9][0-9]{5})$", ErrorMessageResourceName = "ValidationKosovoPhoneNo", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(9, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 9)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("Phone")]
        public string Phone { get; set; }
        #endregion

        #region IdMunicipalityArray
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IdMunicipalityArrayValues")]
        public List<int> IdMunicipalityArrayValues { get; set; }
        public List<SelectListItem>? IdMunicipalityArrayList
        {
            get
            {
                var selectListItems = new List<SelectListItem>();

                if (!string.IsNullOrEmpty(IdMunicipalityArray))
                {
                    var selectedIds = IdMunicipalityArray.Split(',')
                                            .Select(int.Parse)
                                            .ToList();

                    // Assuming IdMunicipalityArrayDdl is already populated with the items
                    foreach (var item in IdMunicipalityArrayDdl)
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
                    foreach (var item in IdMunicipalityArrayDdl)
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
        public GenericGetDdlDataCollection IdMunicipalityArrayDdl
        {
            get
            {
                var obj = GenericGetDdlData.GetDdl("[CommonRegistry].[Municipality]", "IdMunicipality", "");
                return obj;
            }
        }

        //The actual array values holder from database column in 1,2,3,4..... format
        [HiddenInput]
        public string? IdMunicipalityArray
        {
            get => _IdMunicipalityArray;
            set
            {
                // On setting IdMunicipalityArray, join the values from IdMunicipalityArrayValues
                if (IdMunicipalityArrayValues != null && IdMunicipalityArrayValues.Any())
                {
                    _IdMunicipalityArray = String.Join(",", IdMunicipalityArrayValues);
                }
                else
                {
                    _IdMunicipalityArray = value;
                }
            }
        }

        private string? _IdMunicipalityArray;
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

        #region _CtrlCanDisable
        [DbDisplayName("_CtrlCanDisable")] public bool _CtrlCanDisable { get; set; }
        #endregion

        #region _CtrlCanEnable
        [DbDisplayName("_CtrlCanEnable")] public bool _CtrlCanEnable { get; set; }
        #endregion

        #region _CtrlCanUpdate
        [DbDisplayName("_CtrlCanUpdate")] public bool _CtrlCanUpdate { get; set; }
        #endregion

        #region Control Display of Fields

        public string display_IdOrganisation { get; set; } = "";
        public string display_EnDescription { get; set; } = "";
        public string display_SqDescription { get; set; } = "";
        public string display_SrDescription { get; set; } = "";
        public string display_Address { get; set; } = "";
        public string display_Email { get; set; } = "";
        public string display_Phone { get; set; } = "";
        public string display_IdMunicipalityArray { get; set; } = "";
        public string display_IsEnabled { get; set; } = "";
        public string display_EnabledDisabledComment { get; set; } = "";
        public string display_DateEnabledDisabled { get; set; } = "";
        public string display__CtrlCanDisable { get; set; } = "";
        public string display__CtrlCanEnable { get; set; } = "";
        public string display__CtrlCanUpdate { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields

        public bool disable_IdOrganisation { get; set; } = false;
        public bool disable_EnDescription { get; set; } = false;
        public bool disable_SqDescription { get; set; } = false;
        public bool disable_SrDescription { get; set; } = false;
        public bool disable_Address { get; set; } = false;
        public bool disable_Email { get; set; } = false;
        public bool disable_Phone { get; set; } = false;
        public bool disable_IdMunicipalityArray { get; set; } = false;
        public bool disable_IsEnabled { get; set; } = false;
        public bool disable_EnabledDisabledComment { get; set; } = false;
        public bool disable_DateEnabledDisabled { get; set; } = false;
        public bool disable__CtrlCanDisable { get; set; } = false;
        public bool disable__CtrlCanEnable { get; set; } = false;
        public bool disable__CtrlCanUpdate { get; set; } = false;
        #endregion

        // Remote validation methods that are implemented in THIS
        public partial class OrganisationController : Controller
        {
            public IActionResult PlaceHolder()
            {
                return null;
            }
        }
    }
}