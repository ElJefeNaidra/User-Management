using Microsoft.Data.SqlClient;
using System.Data;
namespace UserManagement.Common.Generics
{
    public class GenericGetDdlDataDataLayer
    {
        public static GenericGetDdlDataCollection SelectDdlData(string TableName, string IdKeyColumn, string FilterSearch)
        {

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("ConnectionString");


            HttpContextAccessor _accessor = new HttpContextAccessor();

            GenericGetDdlDataCollection objCollection = null;
            string storedProcName = "[Generic].[sp_Generic_GetDdlData]";

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand(storedProcName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var IdLanguage = _accessor.HttpContext.Session.GetString("IdLanguage");

            command.Parameters.AddWithValue("@IdLanguage", _accessor.HttpContext.Session.GetString("IdLanguage"));
            command.Parameters.AddWithValue("@TableName", TableName);
            command.Parameters.AddWithValue("@IdKeyColumn", IdKeyColumn);

            if (FilterSearch != null && FilterSearch != "")
            {
                command.Parameters.AddWithValue("@FilterSearch", FilterSearch);
            }
            else
            {
                command.Parameters.AddWithValue("@FilterSearch", DBNull.Value);
            }

            using SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt?.Rows.Count > 0)
            {
                objCollection = new GenericGetDdlDataCollection();

                foreach (DataRow dr in dt.Rows)
                {
                    GenericGetDdlDataModel model = new GenericGetDdlDataModel();
                    model.Id = (int)dr["Id"];
                    model.Description = (string)dr["Description"];
                    objCollection.Add(model);
                }
            }

            return objCollection;
        }
    }
}
