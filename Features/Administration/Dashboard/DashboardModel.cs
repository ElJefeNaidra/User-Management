using SIDPSF.Common.Grid;
using SIDPSF.Common.StringLocalisation;
using System.Data;

namespace SIDPSF.Features.Administration.Dashboard
{
    public partial class DashboardModel
    {

        #region TotalNumberOfUsers
        [DbDisplayName("TotalNumberOfUsers")]
        public int TotalNumberOfUsers { get; set; }
        #endregion

        #region TotalNumberOfLoggedInUsers
        [DbDisplayName("TotalNumberOfLoggedInUsers")]
        public int TotalNumberOfLoggedInUsers { get; set; }
        #endregion

        #region TotalNumberOfDisabledUsers
        [DbDisplayName("TotalNumberOfDisabledUsers")]
        public int TotalNumberOfDisabledUsers { get; set; }
        #endregion

        #region TotalNumberOfEnabledUsers
        [DbDisplayName("TotalNumberOfEnabledUsers")]
        public int TotalNumberOfEnabledUsers { get; set; }
        #endregion

        #region TotalNumberOfLockedOutUsers
        [DbDisplayName("TotalNumberOfLockedOutUsers")]
        public int TotalNumberOfLockedOutUsers { get; set; }
        #endregion

        #region TotalNumberOfRequestsThisHour
        [DbDisplayName("TotalNumberOfRequestsThisHour")]
        public int TotalNumberOfRequestsThisHour { get; set; }
        #endregion

        #region TotalNumberOfRequestsToday
        [DbDisplayName("TotalNumberOfRequestsToday")]
        public int TotalNumberOfRequestsToday { get; set; }
        #endregion

        #region TotalNumberOfFailedLoginsToday
        [DbDisplayName("TotalNumberOfFailedLoginsToday")]
        public int TotalNumberOfFailedLoginsToday { get; set; }
        #endregion

        #region TotalNumberOfFailedLoginsLastHour
        [DbDisplayName("TotalNumberOfFailedLoginsLastHour")]
        public int TotalNumberOfFailedLoginsLastHour { get; set; }
        #endregion

        #region TotalNumberOfSMSSent
        [DbDisplayName("TotalNumberOfSMSSent")]
        public int TotalNumberOfSMSSent { get; set; }
        #endregion

        #region TotalNumberOfSMSNotSent
        [DbDisplayName("TotalNumberOfSMSNotSent")]
        public int TotalNumberOfSMSNotSent { get; set; }
        #endregion

        #region TotalNumberOfEmailSent
        [DbDisplayName("TotalNumberOfEmailSent")]
        public int TotalNumberOfEmailSent { get; set; }
        #endregion

        #region TotalNumberOfEmailNotSent
        [DbDisplayName("TotalNumberOfEmailNotSent")]
        public int TotalNumberOfEmailNotSent { get; set; }
        #endregion


        // Property for the DataTable to hold the data
        public DataTable UserLoggedInData { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicUserLoggedInData()
        {
            return Converters.ConvertDataTableToDynamic(UserLoggedInData);
        }


        // Property for the DataTable to hold the data
        public DataTable FailedLogins { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicFailedLogins()
        {
            return Converters.ConvertDataTableToDynamic(FailedLogins);
        }


        // Total Rows
        public int TotalRows { get; set; }


    }
}