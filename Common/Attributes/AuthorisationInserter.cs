using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace UserManagement.Common.Attributes
{
    public class Tools
    {
        public class AuthorisationInserter
        {
            private readonly string _connectionString;

            public AuthorisationInserter(string connectionString)
            {
                _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            }

            public void InsertAuthorisationsForController(Type controllerType)
            {
                if (controllerType.IsClass && typeof(Controller).IsAssignableFrom(controllerType))
                {
                    var parentAttribute = controllerType.GetCustomAttribute<ParentAttribute>();
                    int? parentId = null;
                    if (parentAttribute != null)
                    {
                        parentId = InsertItem(parentAttribute.Name, parentAttribute.EnDescription, parentAttribute.SqDescription, parentAttribute.SrDescription, true, null, null, false);
                    }

                    foreach (var method in controllerType.GetMethods())
                    {
                        var childAttribute = method.GetCustomAttribute<ChildActionAttribute>();
                        var isMenuItemAttribute = method.GetCustomAttribute<IsMenuItemAttribute>();

                        if (childAttribute != null)
                        {
                            string url = $"/{controllerType.Name.Replace("Controller", "")}/{method.Name}";
                            bool isMenuItem = isMenuItemAttribute != null;
                            InsertItem(childAttribute.Name, childAttribute.EnDescription, childAttribute.SqDescription, childAttribute.SrDescription, isMenuItem, parentId, url, !isMenuItem);
                        }
                    }
                }
            }

            private int? InsertItem(string name, string enDescription, string sqDescription, string srDescription, bool isMenuItem, int? parentId, string url, bool isControllerAction)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var maxOrderCommand = connection.CreateCommand();
                    maxOrderCommand.CommandText = @"
                        SELECT MAX([Ordering])
                        FROM [Administration].[Authorisation]";
                    var maxOrderResult = maxOrderCommand.ExecuteScalar();
                    int nextOrder = maxOrderResult != DBNull.Value ? Convert.ToInt32(maxOrderResult) + 1 : 1;

                    var checkCommand = connection.CreateCommand();
                    checkCommand.CommandText = @"
                        SELECT [IdAuthorisation]
                        FROM [Administration].[Authorisation]
                        WHERE [EnDescription] = @EnDescription";
                    checkCommand.Parameters.AddWithValue("@EnDescription", enDescription);

                    var existingId = checkCommand.ExecuteScalar();
                    if (existingId != null)
                    {
                        return (int?)existingId;
                    }

                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = @"
                        INSERT INTO [Administration].[Authorisation]
                            ([EnDescription], [SqDescription], [SrDescription], [IsMenuItem], [ParentId], [Url], [Ordering], [IsControllerAction])
                        VALUES
                            (@EnDescription, @SqDescription, @SrDescription, @IsMenuItem, @ParentId, @Url, @Ordering, @IsControllerAction);
                        SELECT SCOPE_IDENTITY();";
                    insertCommand.Parameters.AddWithValue("@EnDescription", enDescription);
                    insertCommand.Parameters.AddWithValue("@SqDescription", sqDescription);
                    insertCommand.Parameters.AddWithValue("@SrDescription", srDescription);
                    insertCommand.Parameters.AddWithValue("@IsMenuItem", isMenuItem);
                    insertCommand.Parameters.AddWithValue("@ParentId", parentId.HasValue ? parentId.Value : DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@Url", url ?? string.Empty);
                    insertCommand.Parameters.AddWithValue("@Ordering", nextOrder);
                    insertCommand.Parameters.AddWithValue("@IsControllerAction", isControllerAction);

                    return (int?)(decimal)insertCommand.ExecuteScalar();
                }
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ParentAttribute : Attribute
{
    public string Name { get; private set; }
    public string EnDescription { get; private set; }
    public string SqDescription { get; private set; }
    public string SrDescription { get; private set; }

    public ParentAttribute(string name, string enDescription, string sqDescription, string srDescription)
    {
        Name = name;
        EnDescription = enDescription;
        SqDescription = sqDescription;
        SrDescription = srDescription;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class ChildActionAttribute : Attribute
{
    public string Name { get; private set; }
    public string EnDescription { get; private set; }
    public string SqDescription { get; private set; }
    public string SrDescription { get; private set; }

    public ChildActionAttribute(string name, string enDescription, string sqDescription, string srDescription)
    {
        Name = name;
        EnDescription = enDescription;
        SqDescription = sqDescription;
        SrDescription = srDescription;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class IsMenuItemAttribute : Attribute { }
