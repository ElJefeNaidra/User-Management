using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIDPSF.Common.StringLocalisation;
using SIDPSF.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace SIDPSF.Features.Administration.Localisation
{
    public partial class LocalisationModel : BaseCrudModel
    {

        #region IdLocalisation
        [HiddenInput]
        public int IdLocalisation { get; set; }
        #endregion

        #region ResourceName
        [DbDisplayName("ResourceName")]
        public string ResourceName { get; set; }
        #endregion

        #region EnDescription
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 3)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("EnDescription")]
        public string? EnDescription { get; set; }
        #endregion

        #region SqDescription
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 3)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("SqDescription")]
        public string? SqDescription { get; set; }
        #endregion

        #region SrDescription
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 3)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("SrDescription")]
        public string? SrDescription { get; set; }
        #endregion

        public bool _CtrlCanUpdate { get; set; }

        #region Control Display of Fields

        public string display_IdLocalisation { get; set; } = "";
        public string display_ResourceName { get; set; } = "";
        public string display_EnDescription { get; set; } = "";
        public string display_SqDescription { get; set; } = "";
        public string display_SrDescription { get; set; } = "";
        public string display__CtrlCanUpdate { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields

        public bool disable_IdLocalisation { get; set; } = false;
        public bool disable_ResourceName { get; set; } = false;
        public bool disable_EnDescription { get; set; } = false;
        public bool disable_SqDescription { get; set; } = false;
        public bool disable_SrDescription { get; set; } = false;
        public bool disable__CtrlCanUpdate { get; set; } = false;
        #endregion

    }
}