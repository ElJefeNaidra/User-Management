using Microsoft.AspNetCore.Mvc;
using SIDPSF.Common.Generics;
using SIDPSF.Common.Grid;
using SIDPSF.Common.StringLocalisation;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Features.SharedMVC
{
    public class BaseCrudModel
    {
        public string? WindowTitle { get; set; }
        public string? FormTitle { get; set; }
        public string? BreadCrumbRoot { get; set; }
        public string? BreadCrumbTitle { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public CrudOp? crudOp { get; set; }
        public string? ReturnUrl { get; set; }

        #region AUDIT DATA

        #region _IdUserOperation
        [DbDisplayName("_IdUserOperation")]
        public int? _IdUserOperation { get; set; }

        [JsonIgnore]
        public GenericGetNonStandardDdlDataCollection _IdUserOperationDdl
        {
            get
            {
                if (_IdUserOperation != null && _IdUserOperation != 0)
                {
                    var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "IdUser=" + this._IdUserOperation);
                    return obj;
                }
                else
                {
                    var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "");
                    return obj;
                }
            }
        }
        #endregion

        #region _IdOperation

        [DbDisplayName("_IdOperation")]
        public int? _IdOperation { get; set; }

        [JsonIgnore]
        public GenericGetDdlDataCollection _IdOperationDdl
        {
            get
            {
                if (this._IdOperation != null && this._IdOperation != 0)
                {
                    var obj = GenericGetDdlData.GetDdl("[Constants].[Operation]", "IdOperation", "IdOperation=" + this._IdOperation);
                    return obj;
                }
                else
                {
                    var obj = GenericGetDdlData.GetDdl("[Constants].[Operation]", "IdOperation", "");
                    return obj;
                }
            }
        }
        #endregion

        #region _OperationDate

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("_OperationDate")]
        public DateTime? _OperationDate { get; set; }
        
        #endregion

        #region _CreatedOn

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("_CreatedOn")]
        public DateTime? _CreatedOn { get; set; }

        #endregion

        #region _CreatedBy

        [DbDisplayName("_CreatedBy")]
        public int? _CreatedBy { get; set; }

        [JsonIgnore]
        public GenericGetNonStandardDdlDataCollection _CreatedByDdl
        {
            get
            {
                if (_CreatedBy != null && _CreatedBy != 0)
                {
                    var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "IdUser=" + this._CreatedBy);
                    return obj;
                }
                else
                {
                    var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "");
                    return obj;
                }
            }
        }

        #endregion

        #region _LastUpdatedOn

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DbDisplayName("_LastUpdatedOn")]
        public DateTime? _LastUpdatedOn { get; set; }

        #endregion

        #region _UpdatedBy

        [DbDisplayName("_UpdatedBy")]
        public int? _UpdatedBy { get; set; }

        [JsonIgnore]
        public GenericGetNonStandardDdlDataCollection _UpdatedByDdl
        {
            get
            {
                if (_UpdatedBy != null && _UpdatedBy != 0)
                {
                    var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "IdUser=" + this._UpdatedBy);
                    return obj;
                }
                else
                {
                    var obj = GenericGetNonStandardDdlData.GetDdl("[Administration].[User]", "IdUser", "FirstName +' '+ LastName ", "");
                    return obj;
                }
            }
        }

        #endregion

        #region _IpAddress
        [DbDisplayName("_IpAddress")]
        public string? _IpAddress { get; set; }
        #endregion

        #region _GlobalAccessTrackingGUID
        [DbDisplayName("_GlobalAccessTrackingGUID")]
        public string? _GlobalAccessTrackingGUID { get; set; }
        #endregion

        #region _Deleted
        [DbDisplayName("_Deleted")]
        public bool _Deleted { get; set; } = false;
        #endregion

        #region _RowGuid
        [HiddenInput]
        public string? _RowGuid { get; set; }
        #endregion

        #region _RowTimestamp
        [HiddenInput]
        public int? _RowTimestamp { get; set; }
        #endregion

        #region _IsErrorLog
        [DbDisplayName("_IsErrorLog")]
        public bool _IsErrorLog { get; set; } = false;
        #endregion

        #region _LogContent
        [DataType(DataType.MultilineText)]

        [StringLength(500, ErrorMessageResourceName = "ValidationMaximumLength", ErrorMessageResourceType = typeof(_Resources.Common), MinimumLength = 1)]

        [DbDisplayName("_LogContent")]

        public string? _LogContent { get; set; }
        #endregion

        [DbDisplayName("SysStartTime")]
        #region SysStartTime
        public DateTime? SysStartTime { get; set; }

        #endregion

        #region SysEndTime
        [DbDisplayName("SysEndTime")]
        public DateTime? SysEndTime { get; set; }

        #endregion

        #endregion

        public List<string>? _AllowedOperations { get; set; }

        #region History Grid Properties
        public DataTable? HistoryData { get; set; }

        public IEnumerable<dynamic>? GetDynamicHistoryData()
        {
            return Converters.ConvertDataTableToDynamic(HistoryData);
        }

        public int? TotalRows { get; set; }

        #endregion


        #region User Activity Grid Properties

        public DataTable? UserActivityData { get; set; }

        public IEnumerable<dynamic>? GetDynamicUserActivityData()
        {
            return Converters.ConvertDataTableToDynamic(UserActivityData);
        }

        public int? TotalRowsUserActivityData { get; set; }

        #endregion
    }
}
