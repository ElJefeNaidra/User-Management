using Microsoft.Data.SqlClient;
using System.Data;

namespace UserManagement.Common.Generics
{
    public class GenericGetNonStandardDdlDataDataLayer
    {
        public static GenericGetNonStandardDdlDataCollection SelectDdlData(string TableName, string IdKeyColumn, string DescriptionColumnName, string FilterSearch)
        {

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("ConnectionString");


            var objCollection = new GenericGetNonStandardDdlDataCollection();
            string storedProcName = "[Generic].[sp_Generic_GetNonStandardDdlData]";

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TableName", TableName);
            command.Parameters.AddWithValue("@IdKeyColumn", IdKeyColumn);
            command.Parameters.AddWithValue("@DescriptionColumnName", DescriptionColumnName);

            if (FilterSearch != null && FilterSearch != "")
            {
                command.Parameters.AddWithValue("@FilterSearch", FilterSearch);
            }
            else
            {
                command.Parameters.AddWithValue("@FilterSearch", DBNull.Value);
            }

            var diski = command.Parameters;


            using SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt?.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GenericGetNonStandardDdlDataModel model = new GenericGetNonStandardDdlDataModel();
                    model.Id = (int)dr["Id"];
                    model.Description = (string)dr["Description"];
                    objCollection.Add(model);
                }
            }

            return objCollection;
        }

        private static void AddCommandParameter(SqlCommand command, string parameterName, object value)
        {
            command.Parameters.AddWithValue(parameterName, value);
        }
    }
}
