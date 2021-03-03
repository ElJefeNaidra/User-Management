using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIDPSF.Common.Generics;
using SIDPSF.Common.StringLocalisation;
using SIDPSF.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace SIDPSF.Features.CommonRegistry.Settlement
{
    public partial class SettlementModel : BaseCrudModel
    {

        #region IdSettlement
        [HiddenInput]
        public int IdSettlement { get; set; }
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

        #region IdMunicipality
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("IdMunicipality")]
        public int IdMunicipality { get; set; }

        public GenericGetDdlDataCollection IdMunicipalityDdl
        {
            get
            {
                var obj = GenericGetDdlData.GetDdl("[CommonRegistry].[Municipality]", "IdMunicipality", "");
                return obj;
            }
        }
        #endregion

        #region IsUrban
        [DbDisplayName("IsUrban")]
        public bool IsUrban { get; set; } = false;
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

        public string display_IdSettlement { get; set; } = "";
        public string display_EnDescription { get; set; } = "";
        public string display_SqDescription { get; set; } = "";
        public string display_SrDescription { get; set; } = "";
        public string display_IdMunicipality { get; set; } = "";
        public string display_IsUrban { get; set; } = "";
        public string display_IsEnabled { get; set; } = "";
        public string display_EnabledDisabledComment { get; set; } = "";
        public string display_DateEnabledDisabled { get; set; } = "";
        public string display__CtrlCanUpdate { get; set; } = "";
        public string display__CtrlCanEnable { get; set; } = "";
        public string display__CtrlCanDisable { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields

        public bool disable_IdSettlement { get; set; } = false;
        public bool disable_EnDescription { get; set; } = false;
        public bool disable_SqDescription { get; set; } = false;
        public bool disable_SrDescription { get; set; } = false;
        public bool disable_IdMunicipality { get; set; } = false;
        public bool disable_IsUrban { get; set; } = false;
        public bool disable_IsEnabled { get; set; } = false;
        public bool disable_EnabledDisabledComment { get; set; } = false;
        public bool disable_DateEnabledDisabled { get; set; } = false;
        public bool disable__CtrlCanUpdate { get; set; } = false;
        public bool disable__CtrlCanEnable { get; set; } = false;
        public bool disable__CtrlCanDisable { get; set; } = false;
        #endregion

    }
}