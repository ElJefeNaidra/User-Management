using System.Data;
using System.Dynamic;

namespace SIDPSF.Common.Grid
{
    public class Converters
    {
        public static IEnumerable<dynamic> ConvertDataTableToDynamic(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                var dynamicDict = new ExpandoObject() as IDictionary<string, object>;
                foreach (DataColumn column in dt.Columns)
                {
                    dynamicDict[column.ColumnName] = row[column];
                }
                yield return dynamicDict;
            }
        }

        public static List<dynamic> ConvertDataTableToDynamicList(DataTable dataTable)
        {
            var list = new List<dynamic>();

            foreach (DataRow row in dataTable.Rows)
            {
                dynamic obj = new ExpandoObject();
                var dict = (IDictionary<string, object>)obj;

                foreach (DataColumn column in dataTable.Columns)
                {
                    dict[column.ColumnName] = row.IsNull(column) ? null : row[column];
                }

                list.Add(obj);
            }

            return list;
        }


    }
}
