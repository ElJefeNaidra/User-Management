using SIDPSF.Common.Generics;
using SIDPSF.Common.Grid;
using SIDPSF.Common.StringLocalisation;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace SIDPSF.Features.Administration.Communication
{
    public class CommunicationPagedSearchModel
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
        public DataTable CommunicationData { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicCommunicationData()
        {
            return Converters.ConvertDataTableToDynamic(CommunicationData);
        }

        // Total Rows
        public int Total { get; set; }

        #region Filtering properties

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
        [RegularExpression(@"^04[3-9](1[0-9]{5}|[2-9][0-9]{5})$", ErrorMessageResourceName = "ValidationKosovoPhoneNo", ErrorMessageResourceType = typeof(_Resources.Common))]
        [StringLength(9, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 9)]
        [DbDisplayName("PhoneNo")]
        public string? PhoneNo { get; set; }

        #endregion

        #region EmailAddress
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 9)]
        [EmailAddress(ErrorMessageResourceName = "ValidationEmail", ErrorMessageResourceType = typeof(_Resources.Common))]
        [RegularExpression(@"^[a-zA-Z0-9@\.\-_]+$", ErrorMessageResourceName = "ValidationInvalidEmailCharacters", ErrorMessageResourceType = typeof(_Resources.Common))]
        [DbDisplayName("EmailAddress")]
        public string? EmailAddress { get; set; }
        #endregion

        #region EmailSubject
        [StringLength(255, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]
        [DbDisplayName("EmailSubject")]
        public string? EmailSubject { get; set; }
        #endregion

        #region IdStatus
        [DbDisplayName("IdStatus")]
        public int? IdStatus { get; set; }
        public GenericGetDdlDataCollection IdStatusDdl { get; set; } = new GenericGetDdlDataCollection();
        public void PopulateIdStatusDdl(string idLanguage)
        {
            IdStatusDdl.Clear();

            switch (idLanguage)
            {
                case "1":
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 1, Description = "Sent" });
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 2, Description = "Canceled" });
                    break;

                case "2":
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 1, Description = "Dërguar" });
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 2, Description = "Anuluar" });
                    break;

                case "3":
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 1, Description = "Poslano" });
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 2, Description = "Otkazano" });
                    break;

                default:
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 1, Description = "Sent" });
                    IdStatusDdl.Add(new GenericGetDdlDataModel { Id = 2, Description = "Canceled" });
                    break;
            }
        }

        #endregion

        #endregion
    }
}