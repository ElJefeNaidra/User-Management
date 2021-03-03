namespace UserManagement.Common.Generics
{
    public class GenericGetDdlData
    {
        public static GenericGetDdlDataCollection GetDdl(string tableName, string idKeyColumn, string filterSearch)
        {
            return GenericGetDdlDataDataLayer.SelectDdlData(tableName, idKeyColumn, filterSearch);
        }
    }
}
