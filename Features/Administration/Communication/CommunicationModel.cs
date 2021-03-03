using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIDPSF.Common.Generics;
using SIDPSF.Common.StringLocalisation;
using SIDPSF.Features.SharedMVC;
using System.ComponentModel.DataAnnotations;

namespace SIDPSF.Features.Administration.Communication
{
    public partial class CommunicationModel : BaseCrudModel
    {
        #region IdCommunication
        [HiddenInput]
        public int IdCommunication { get; set; }
        #endregion

        #region CommunicationGUID
        [DbDisplayName("CommunicationGUID")]
        public string? CommunicationGUID { get; set; }
        #endregion

        #region IdTypeOfCommunication
        [DbDisplayName("IdTypeOfCommunication")]
        public int? IdTypeOfCommunication { get; set; }

        [JsonIgnore]
        public GenericGetDdlDataCollection IdTypeOfCommunicationDdl
        {
            get
            {
                // Initialize the collection
                GenericGetDdlDataCollection obj = new GenericGetDdlDataCollection();


                // Add items to the collection
                obj.Add(new GenericGetDdlDataModel { Id = 1, Description = "SMS" });
                obj.Add(new GenericGetDdlDataModel { Id = 2, Description = "E-mail" });

                return obj;
            }
        }

        #endregion

        #region PhoneNo
        [DbDisplayName("PhoneNo")]
        public string? PhoneNo { get; set; }
        #endregion

        #region SMSContent
        [DataType(DataType.MultilineText)]
        [DbDisplayName("SMSContent")]
        public string? SMSContent { get; set; }
        #endregion

        #region EmailAddress
        [DbDisplayName("EmailAddress")]
        public string? EmailAddress { get; set; }
        #endregion

        #region EmailSubject
        [DbDisplayName("EmailSubject")]
        public string? EmailSubject { get; set; }
        #endregion

        #region EmailContent
        [DataType(DataType.MultilineText)]
        [DbDisplayName("EmailContent")]
        public string? EmailContent { get; set; }
        #endregion

        #region Sent
        [DbDisplayName("Sent")]
        public bool Sent { get; set; } = false;
        #endregion

        #region DateSent
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessageResourceName = "ValidationDateFormatWrong", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("DateSent")]
        public DateTime? DateSent { get; set; }
        #endregion

        #region IsCanceled
        [DbDisplayName("IsCanceled")]
        public bool IsCanceled { get; set; } = false;
        #endregion

        #region CanceledUncanceledComment
        [DataType(DataType.MultilineText)]
        [StringLength(300, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 5)]
        [Required(ErrorMessageResourceName = "ValidationRequired", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[A-Za-z0-9 \-,.çëčćđšžğışüöİĞŞÜÖ ]*$", ErrorMessageResourceName = "ValidateMultiLangChars", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("CanceledUncanceledComment")]
        public string? CanceledUncanceledComment { get; set; }
        #endregion

        #region DateCanceledUncanceled
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("DateCanceledUncanceled")]
        public DateTime? DateCanceledUncanceled { get; set; }
        #endregion

        public bool _CtrlCanResend { get; set; }
        public bool _CtrlCanCancel { get; set; }
        public bool _CtrlCanUncancel { get; set; }
        
        #region Control Display of Fields

        public string display_IdCommunication { get; set; } = "";
        public string display_CommunicationGUID { get; set; } = "";
        public string display_IdTypeOfCommunication { get; set; } = "";
        public string display_PhoneNo { get; set; } = "";
        public string display_SMSContent { get; set; } = "";
        public string display_EmailAddress { get; set; } = "";
        public string display_EmailSubject { get; set; } = "";
        public string display_EmailContent { get; set; } = "";
        public string display_Sent { get; set; } = "";
        public string display_DateSent { get; set; } = "";
        public string display_IsCanceled { get; set; } = "";
        public string display_CanceledUncanceledComment { get; set; } = "";
        public string display_DateCanceledUncanceled { get; set; } = "";
        public string display__CtrlCanResend { get; set; } = "";
        public string display__CtrlCanCancel { get; set; } = "";
        public string display__CtrlCanUncancel { get; set; } = "";
        #endregion

        #region Control Disabling/Enabling of Fields

        public bool disable_IdCommunication { get; set; } = false;
        public bool disable_CommunicationGUID { get; set; } = false;
        public bool disable_IdTypeOfCommunication { get; set; } = false;
        public bool disable_PhoneNo { get; set; } = false;
        public bool disable_SMSContent { get; set; } = false;
        public bool disable_EmailAddress { get; set; } = false;
        public bool disable_EmailSubject { get; set; } = false;
        public bool disable_EmailContent { get; set; } = false;
        public bool disable_Sent { get; set; } = false;
        public bool disable_DateSent { get; set; } = false;
        public bool disable_IsCanceled { get; set; } = false;
        public bool disable_CanceledUncanceledComment { get; set; } = false;
        public bool disable_DateCanceledUncanceled { get; set; } = false;
        public bool disable__CtrlCanResend { get; set; } = false;
        public bool disable__CtrlCanCancel { get; set; } = false;
        public bool disable__CtrlCanUncancel { get; set; } = false;
        #endregion
    }
}