using SIDPSF.Common.Grid;
using System.Data;

namespace SIDPSF.Features.CommonRegistry.Region
{
    // Implementation of Search Model for Simple no ServerOperation Grids
    public class RegionSimpleSearchModel
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
        public DataTable RegionData { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicRegionData()
        {
            return Converters.ConvertDataTableToDynamic(RegionData);
        }

        // Total Rows
        public int TotalRows { get; set; }
    }
}
