using SIDPSF.Common.Grid;
using System.Data;

namespace SIDPSF.Features.CommonRegistry.Language
{
    // Implementation of Search Model for Simple no ServerOperation Grids
    public class LanguageSimpleSearchModel
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
        public DataTable LanguageData { get; set; }

        // Get non-fixed structure data
        public IEnumerable<dynamic> GetDynamicLanguageData()
        {
            return Converters.ConvertDataTableToDynamic(LanguageData);
        }

        // Total Rows
        public int TotalRows { get; set; }
    }
}
