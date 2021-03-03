using UserManagement.Common.Grid;
using System.Data;

namespace UserManagement.Features.CommonRegistry.Municipality
{
    // Implementation of Search Model for Simple no ServerOperation Grids
    public class MunicipalitySimpleSearchModel
    {
        #region GUI Stuff
        public string WindowTitle { get; set; }
        public string FormTitle { get; set; }
        public string BreadCrumbRoot { get; set; }
        public string BreadCrumbTitle { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        #endregion


        // Property for the DataTable to hold the data
        public DataTable MunicipalityData { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicMunicipalityData()
        {
            return Converters.ConvertDataTableToDynamic(MunicipalityData);
        }

        // Total Rows
        public int TotalRows { get; set; }
    }
}
