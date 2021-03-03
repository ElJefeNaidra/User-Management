namespace SIDPSF.Common.Generics
{
    public class GenericGetNonStandardDdlData
    {
        public static GenericGetNonStandardDdlDataCollection GetDdl(string TableName, string IdKeyColumn, string DescriptionColumnName, string FilterSearch)
        {
            return GenericGetNonStandardDdlDataDataLayer.SelectDdlData(TableName, IdKeyColumn, DescriptionColumnName, FilterSearch);
        }
    }
}




