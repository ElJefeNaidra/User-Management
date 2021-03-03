using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Reflection;

namespace SIDPSF.Common.DataAccess
{
    public partial class DBContext
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _accessor;

        private static string _dbContextErrorLogsPath;

        public static void SetDbContextErrorLogsPath(string path)
        {
            _dbContextErrorLogsPath = path;
        }

        public DBContext(string connectionString, IMemoryCache cache, IHttpContextAccessor accessor)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        /// <summary>
        /// Executes a specified stored procedure to Insert a record of type <typeparamref name="TModel"/> in the database.
        /// </summary>
        /// <typeparam name="TModel">The type of the model that represents the table structure in the database.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute for the update operation.</param>
        /// <param name="model">The instance of <typeparamref name="TModel"/> containing the data for the update operation.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, which, upon completion,
        /// returns a <see cref="ResponseInformationModel"/> containing details about the success or failure of the operation.
        /// </returns>
        public async Task<(ResponseInformationModel, TModel)> InsertAsync<TModel>(string storedProcedureName, TModel model)
        {
            var response = new ResponseInformationModel(); // Prepare a default response.

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fetch stored procedure parameters.
            var spParameters = await FetchStoredProcedureParameters(storedProcedureName, connection);

            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var modelType = typeof(TModel);
            var modelKey = modelType.FullName;

            // Try to fetch the model properties from cache first.
            if (!_cache.TryGetValue(modelKey, out List<PropertyInfo> modelProperties))
            {
                modelProperties = modelType.GetProperties().ToList();
                _cache.Set(modelKey, modelProperties); // Cache the determined properties for future use.
            }

            // Add the IdLanguage parameter from the session.
            var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
            command.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "default_value"); // Replace 'default_value' with a sensible default if necessary.

            foreach (var prop in modelProperties)
            {
                var matchingParam = spParameters.FirstOrDefault(p => p.ParameterName.Equals($"@{prop.Name}", StringComparison.OrdinalIgnoreCase));
                if (matchingParam != null)
                {
                    var paramValue = prop.GetValue(model);
                    if (paramValue == null)
                    {
                        if (!matchingParam.IsNullable)
                        {
                            continue; // Skip null properties if SP parameter is not nullable.
                        }
                        paramValue = DBNull.Value;
                    }

                    command.Parameters.AddWithValue(matchingParam.ParameterName, paramValue);
                }
            }

            try
            {
                var reader = await command.ExecuteReaderAsync();

                // Process the result from the stored procedure.
                if (reader.Read())
                {
                    if (reader.GetOrdinal("IdValue") != -1 && !reader.IsDBNull(reader.GetOrdinal("IdValue")))
                        response.IdValue = reader.GetInt32(reader.GetOrdinal("IdValue"));

                    if (reader.GetOrdinal("HasError") != -1 && !reader.IsDBNull(reader.GetOrdinal("HasError")))
                        response.HasError = reader.GetBoolean(reader.GetOrdinal("HasError"));

                    if (reader.GetOrdinal("ErrorCode") != -1 && !reader.IsDBNull(reader.GetOrdinal("ErrorCode")))
                        response.ErrorCode = reader.GetString(reader.GetOrdinal("ErrorCode"));

                    if (reader.GetOrdinal("ErrorMessage") != -1 && !reader.IsDBNull(reader.GetOrdinal("ErrorMessage")))
                        response.ErrorMessage = reader.GetString(reader.GetOrdinal("ErrorMessage"));

                    if (reader.GetOrdinal("InformationMessage") != -1 && !reader.IsDBNull(reader.GetOrdinal("InformationMessage")))
                        response.InformationMessage = reader.GetString(reader.GetOrdinal("InformationMessage"));

                    if (reader.GetOrdinal("_RowGuid") != -1 && !reader.IsDBNull(reader.GetOrdinal("_RowGuid")))
                        response._RowGuid = reader.GetString(reader.GetOrdinal("_RowGuid"));
                }
            }
            catch (SqlException ex)
            {
                // If there's a SQL exception, log the error and prepare an error response.
                File.AppendAllText(_dbContextErrorLogsPath + "InsertAsync.txt", ex.ToString());
                response.HasError = true;
                response.ErrorMessage = "There was an error executing the stored procedure.";
            }
            finally
            {
                // Ensure the connection is closed after executing the command.
                connection.Close();
            }

            return (response, model); // Corrected return statement
        }

        /// <summary>
        /// Executes a specified stored procedure to update a record of type <typeparamref name="TModel"/> in the database.
        /// </summary>
        /// <typeparam name="TModel">The type of the model that represents the table structure in the database.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute for the update operation.</param>
        /// <param name="model">The instance of <typeparamref name="TModel"/> containing the data for the update operation.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, which, upon completion,
        /// returns a <see cref="ResponseInformationModel"/> containing details about the success or failure of the operation.
        /// </returns>
        public async Task<(ResponseInformationModel, TModel)> UpdateAsync<TModel>(string storedProcedureName, TModel model)
        {
            var response = new ResponseInformationModel(); // Prepare a default response.

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fetch stored procedure parameters.
            var spParameters = await FetchStoredProcedureParameters(storedProcedureName, connection);

            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var modelType = typeof(TModel);
            var modelKey = modelType.FullName;

            // Try to fetch the model properties from cache first.
            if (!_cache.TryGetValue(modelKey, out List<PropertyInfo> modelProperties))
            {
                modelProperties = modelType.GetProperties().ToList();
                _cache.Set(modelKey, modelProperties); // Cache the determined properties for future use.
            }

            // Add the IdLanguage parameter from the session.
            var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
            command.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "default_value"); // Replace 'default_value' with a sensible default if necessary.

            foreach (var prop in modelProperties)
            {
                var matchingParam = spParameters.FirstOrDefault(p => p.ParameterName.Equals($"@{prop.Name}", StringComparison.OrdinalIgnoreCase));
                if (matchingParam != null)
                {
                    var paramValue = prop.GetValue(model);
                    if (paramValue == null)
                    {
                        if (!matchingParam.IsNullable)
                        {
                            continue; // Skip null properties if SP parameter is not nullable.
                        }
                        paramValue = DBNull.Value;
                    }

                    command.Parameters.AddWithValue(matchingParam.ParameterName, paramValue);
                }
            }

            try
            {
                var reader = await command.ExecuteReaderAsync();

                // Process the result from the stored procedure.
                if (reader.Read())
                {
                    if (reader.GetOrdinal("IdValue") != -1 && !reader.IsDBNull(reader.GetOrdinal("IdValue")))
                        response.IdValue = reader.GetInt32(reader.GetOrdinal("IdValue"));

                    if (reader.GetOrdinal("HasError") != -1 && !reader.IsDBNull(reader.GetOrdinal("HasError")))
                        response.HasError = reader.GetBoolean(reader.GetOrdinal("HasError"));

                    if (reader.GetOrdinal("ErrorCode") != -1 && !reader.IsDBNull(reader.GetOrdinal("ErrorCode")))
                        response.ErrorCode = reader.GetString(reader.GetOrdinal("ErrorCode"));

                    if (reader.GetOrdinal("ErrorMessage") != -1 && !reader.IsDBNull(reader.GetOrdinal("ErrorMessage")))
                        response.ErrorMessage = reader.GetString(reader.GetOrdinal("ErrorMessage"));

                    if (reader.GetOrdinal("InformationMessage") != -1 && !reader.IsDBNull(reader.GetOrdinal("InformationMessage")))
                        response.InformationMessage = reader.GetString(reader.GetOrdinal("InformationMessage"));
                }
            }
            catch (SqlException ex)
            {
                // If there's a SQL exception, log the error and prepare an error response.
                File.AppendAllText(_dbContextErrorLogsPath + "UpdateAsync.txt", ex.ToString());
                response.HasError = true;
                response.ErrorMessage = "There was an error executing the stored procedure.";
            }
            finally
            {
                // Ensure the connection is closed after executing the command.
                connection.Close();
            }

            return (response, model); // Corrected return statement
        }

        /// <summary>
        /// Asynchronously performs an update operation using a nested model and a specified stored procedure.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to be updated.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to be executed.</param>
        /// <param name="model">The model object containing the data to be updated.</param>
        /// <returns>
        /// A Task representing the asynchronous operation, which upon completion returns a tuple consisting of
        /// a <see cref="ResponseInformationModel"/> and the updated <typeparamref name="TModel"/>.
        /// </returns>
        public async Task<(ResponseInformationModel, TModel)> UpdateWithViewModelAsync<TModel>(string storedProcedureName, TModel model)
        {
            var response = new ResponseInformationModel(); // Prepare a default response.

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fetch stored procedure parameters.
            var spParameters = await FetchStoredProcedureParameters(storedProcedureName, connection);

            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var modelType = typeof(TModel);
            var modelKey = modelType.FullName;

            // Try to fetch the model properties from cache first.
            if (!_cache.TryGetValue(modelKey, out List<PropertyInfo> modelProperties))
            {
                modelProperties = modelType.GetProperties().ToList();
                _cache.Set(modelKey, modelProperties); // Cache the determined properties for future use.
            }

            // Add the IdLanguage parameter only if it's a parameter in the stored procedure.
            var idLanguageParamExists = spParameters.Any(p => p.ParameterName.Equals("@IdLanguage", StringComparison.OrdinalIgnoreCase));
            if (idLanguageParamExists)
            {
                var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
                command.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "1"); // Replace '1' with a sensible default if necessary.
            }

            // Create a dictionary to hold the last non-null value for each property
            var propertyValues = new Dictionary<string, object>();

            CollectPropertyValues(model, typeof(TModel), propertyValues);

            foreach (var prop in modelProperties)
            {
                var matchingParam = spParameters.FirstOrDefault(p => p.ParameterName.Equals($"@{prop.Name}", StringComparison.OrdinalIgnoreCase));
                if (matchingParam != null)
                {
                    var paramValue = prop.GetValue(model);
                    if (paramValue == null)
                    {
                        if (!matchingParam.IsNullable)
                        {
                            continue; // Skip null properties if SP parameter is not nullable.
                        }
                        paramValue = DBNull.Value;
                    }

                    command.Parameters.AddWithValue(matchingParam.ParameterName, paramValue);
                }
            }

            try
            {
                var reader = await command.ExecuteReaderAsync();

                // Process the result from the stored procedure.
                if (reader.Read())
                {
                    response.IdValue = reader.GetOrdinal("IdValue") != -1 ? reader.GetInt32(reader.GetOrdinal("IdValue")) : default;
                    response.HasError = reader.GetOrdinal("HasError") != -1 && reader.GetBoolean(reader.GetOrdinal("HasError"));
                    response.ErrorCode = reader.GetOrdinal("ErrorCode") != -1 ? reader.GetString(reader.GetOrdinal("ErrorCode")) : null;
                    response.ErrorMessage = reader.GetOrdinal("ErrorMessage") != -1 ? reader.GetString(reader.GetOrdinal("ErrorMessage")) : null;
                    response.InformationMessage = reader.GetOrdinal("InformationMessage") != -1 ? reader.GetString(reader.GetOrdinal("InformationMessage")) : null;
                }
            }
            catch (SqlException ex)
            {
                // If there's a SQL exception, log the error and prepare an error response.
                File.AppendAllText(_dbContextErrorLogsPath + "UpdateAsync.txt", ex.ToString());
                response.HasError = true;
                response.ErrorMessage = "There was an error executing the stored procedure.";
            }
            finally
            {
                // Ensure the connection is closed after executing the command.
                connection.Close();
            }
            return (response, model); // Corrected return statement
        }

        /// <summary>
        /// Asynchronously executes a stored procedure for an update operation, using a list of parameter values.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterValues">A list of key-value pairs representing the parameters and their values for the stored procedure.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, which upon completion returns a <see cref="ResponseInformationModel"/>.
        /// This model contains details about the success or failure of the operation, including error messages if any.
        /// </returns>
        public async Task<ResponseInformationModel> UpdateFromParametersListAsync(string storedProcedureName, List<KeyValuePair<string, object>> parameterValues)
        {
            var response = new ResponseInformationModel(); // Prepare a default response.

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fetch stored procedure parameters.
            var spParameters = await FetchStoredProcedureParameters(storedProcedureName, connection);

            // Check for missing parameters
            var missingParameters = spParameters.Where(sp => !parameterValues.Any(pv => $"@{pv.Key}".Equals(sp.ParameterName, StringComparison.OrdinalIgnoreCase)))
                                                .Select(sp => sp.ParameterName)
                                                .ToList();

            if (missingParameters.Any())
            {
                response.HasError = true;
                response.ErrorMessage = "Missing parameters: " + string.Join(", ", missingParameters);
                return response;
            }

            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Map provided key-value pairs to command parameters
            foreach (var param in parameterValues)
            {
                var matchingParam = spParameters.FirstOrDefault(p => $"@{param.Key}".Equals(p.ParameterName, StringComparison.OrdinalIgnoreCase));
                if (matchingParam != null)
                {
                    command.Parameters.AddWithValue(matchingParam.ParameterName, param.Value ?? DBNull.Value);
                }
            }

            try
            {
                var reader = await command.ExecuteReaderAsync();

                // Process the result from the stored procedure.
                if (reader.Read())
                {
                    // Assuming these columns are part of the result set. Adjust based on actual stored procedure.
                    response.IdValue = reader.GetOrdinal("IdValue") != -1 ? reader.GetInt32(reader.GetOrdinal("IdValue")) : default;
                    response.HasError = reader.GetOrdinal("HasError") != -1 && reader.GetBoolean(reader.GetOrdinal("HasError"));
                    response.ErrorCode = reader.GetOrdinal("ErrorCode") != -1 ? reader.GetString(reader.GetOrdinal("ErrorCode")) : null;
                    response.ErrorMessage = reader.GetOrdinal("ErrorMessage") != -1 ? reader.GetString(reader.GetOrdinal("ErrorMessage")) : null;
                    response.InformationMessage = reader.GetOrdinal("InformationMessage") != -1 ? reader.GetString(reader.GetOrdinal("InformationMessage")) : null;
                }
            }
            catch (SqlException ex)
            {
                // If there's a SQL exception, log the error and prepare an error response.
                File.AppendAllText(_dbContextErrorLogsPath + "UpdateFromParametersAsync.txt", ex.ToString());
                response.HasError = true;
                response.ErrorMessage = "There was an error executing the stored procedure.";
            }
            finally
            {
                // Ensure the connection is closed after executing the command.
                connection.Close();
            }

            return response; // Return the response
        }

        /// <summary>
        /// Maps properties of a model, including nested complex types, to SQL parameters for a SqlCommand.
        /// </summary>
        /// <param name="model">The model object whose properties are to be mapped to SQL parameters.</param>
        /// <param name="modelType">The Type of the model object.</param>
        /// <param name="command">The SqlCommand to which the parameters will be added.</param>
        /// <param name="spParameters">A list of SqlParameter objects representing the parameters of the stored procedure.</param>
        private void MapModelToParameters(object model, Type modelType, SqlCommand command, List<SqlParameter> spParameters)
        {
            foreach (var prop in modelType.GetProperties())
            {
                if (IsComplexType(prop.PropertyType))
                {
                    // If the property is a complex type, process its properties
                    var nestedModel = prop.GetValue(model);
                    if (nestedModel != null)
                    {
                        MapModelToParameters(nestedModel, prop.PropertyType, command, spParameters);
                    }
                }
                else
                {
                    // Map simple properties as before
                    var matchingParam = spParameters.FirstOrDefault(p => p.ParameterName.Equals($"@{prop.Name}", StringComparison.OrdinalIgnoreCase));
                    if (matchingParam != null)
                    {
                        var paramValue = prop.GetValue(model);
                        paramValue = paramValue ?? (matchingParam.IsNullable ? DBNull.Value : default);
                        command.Parameters.AddWithValue(matchingParam.ParameterName, paramValue);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether a given Type is a complex type, defined as not being a primitive, an enum, or a string.
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <returns>True if the Type is a complex type; otherwise, false.</returns>
        private bool IsComplexType(Type type)
        {
            // Determine if the type is a complex type (not a primitive, enum, or string)
            return !type.IsPrimitive && !type.IsEnum && type != typeof(string);
        }

        /// <summary>
        /// Collects the values of properties from a model, including nested complex types, into a dictionary.
        /// </summary>
        /// <param name="model">The model object from which property values are to be collected.</param>
        /// <param name="modelType">The Type of the model object.</param>
        /// <param name="propertyValues">A dictionary to store the property values, keyed by property names.</param>
        private void CollectPropertyValues(object model, Type modelType, Dictionary<string, object> propertyValues)
        {
            foreach (var prop in modelType.GetProperties())
            {
                if (IsComplexType(prop.PropertyType))
                {
                    var nestedModel = prop.GetValue(model);
                    if (nestedModel != null)
                    {
                        CollectPropertyValues(nestedModel, prop.PropertyType, propertyValues);
                    }
                }
                else
                {
                    var value = prop.GetValue(model);
                    if (value != null)
                    {
                        propertyValues[prop.Name] = value; // This will overwrite the value if it was already set by a previous model
                    }
                }
            }
        }

        /// <summary>
        /// Represents the information about a response from a database operation.
        /// This model is used to encapsulate details about the outcome of a database operation,
        /// such as success or failure, error messages, and any relevant identifiers.
        /// </summary>
        public class ResponseInformationModel
        {
            /// <summary>
            /// Gets or sets the ID value associated with the database operation. Default is -1.
            /// This can be used to return a primary key value or a similar identifier.
            /// </summary>
            public int? IdValue { get; set; } = -1;

            /// <summary>
            /// Indicates whether the database operation resulted in an error. Default is false.
            /// </summary>
            public bool HasError { get; set; } = false;

            /// <summary>
            /// Gets or sets the error code associated with the database operation, if any. Default is "-".
            /// </summary>
            public string? ErrorCode { get; set; } = "-";

            /// <summary>
            /// Gets or sets the error message associated with the database operation, if any. Default is "-".
            /// </summary>
            public string? ErrorMessage { get; set; } = "-";

            /// <summary>
            /// Gets or sets any additional informational message regarding the database operation. Default is "-".
            /// </summary>
            public string? InformationMessage { get; set; } = "-";

            /// <summary>
            /// Gets or sets any additional _RowGUID value regarding the database operation. Default is "-".
            /// </summary>
            public string? _RowGuid { get; set; } = "-";
        }

        /// <summary>
        /// Represents a generic response model that includes both data and response information.
        /// </summary>
        /// <typeparam name="T">The type of data included in the response.</typeparam>
        public class ResponseModel<T>
        {
            /// <summary>
            /// Gets or sets the data of the response.
            /// </summary>
            public T Data { get; set; }

            /// <summary>
            /// Gets or sets the response information, which includes status and error messages.
            /// </summary>
            public ResponseInformationModel Information { get; set; }
        }

        /// <summary>
        /// Fetches the parameters of a stored procedure from a SQL Server database.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to fetch parameters for.</param>
        /// <param name="connection">The active SQL connection.</param>
        /// <returns>A Task representing the asynchronous operation, which upon completion returns a list of SQL parameters for the specified stored procedure.</returns>

        private async Task<List<SqlParameter>> FetchStoredProcedureParameters(string storedProcedureName, SqlConnection connection)
        {
            var parameters = new List<SqlParameter>();

            string query = @"
                SELECT 
                    'Parameter_name' = name, 
                    'Type' = type_name(user_type_id), 
                    'Length' = max_length, 
                    'Prec' = ISNULL(precision, 0), 
                    'Scale' = ISNULL(scale, 0), 
                    'IsOutParam' = is_output, 
                    'IsNullable' = is_nullable
                FROM 
                    sys.parameters 
                WHERE 
                    object_id = OBJECT_ID(@storedProcedureName)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@storedProcedureName", storedProcedureName);

            using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var paramName = reader["Parameter_name"].ToString();
                var isNullable = (bool)reader["IsNullable"];

                parameters.Add(new SqlParameter
                {
                    ParameterName = paramName,
                    IsNullable = isNullable
                });
            }

            return parameters;
        }

        /// <summary>
        /// Converts specified properties of a model into a list of key-value pairs for stored procedure parameters.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="model">The model instance from which to extract values.</param>
        /// <param name="properties">Comma delimited string of property names to include.</param>
        /// <returns>A list of key-value pairs representing the specified properties and their values.</returns>
        public List<KeyValuePair<string, object>> ModelToSPParameterListBuilder<TModel>(
            TModel model, string properties,
            List<KeyValuePair<string, object>> existingParameters = null)
        {
            var parameterList = existingParameters ?? new List<KeyValuePair<string, object>>();
            var propertyNames = properties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim());

            var modelType = typeof(TModel);
            var cacheKey = $"{modelType.FullName}_Properties";

            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, PropertyInfo> cachedProperties))
            {
                // Cache miss, so load properties and cache them
                cachedProperties = modelType.GetProperties().ToDictionary(p => p.Name, p => p);
                _cache.Set(cacheKey, cachedProperties);
            }

            foreach (var propName in propertyNames)
            {
                if (cachedProperties.TryGetValue(propName, out PropertyInfo propertyInfo))
                {
                    var value = propertyInfo.GetValue(model);
                    parameterList.Add(new KeyValuePair<string, object>(propName, value ?? DBNull.Value));
                }
            }

            return parameterList;
        }


        /// <summary>
        /// Common properties for displaying title and text in the search form if there is any
        /// </summary>
        public class GridFormModel
        {
            [Skip]
            public string WindowTitle { get; set; }

            [Skip]
            public string FormTitle { get; set; }

            [Skip]
            public string BreadCrumbRoot { get; set; }

            [Skip]
            public string BreadCrumbTitle { get; set; }

            [Skip]
            public string ControllerName { get; set; }

            [Skip]
            public string ActionName { get; set; }
        }

        /// <summary>
        /// Used to decorate FilterModel properties with [Skip] to enable them to be skipped
        /// during maping of FilterModel properties to SP parameter names 1:1
        /// </summary>

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public class SkipAttribute : Attribute
        {
        }

        /// <summary>
        /// Retrieves a list of data from the database based on the given filter, along with pagination options.
        /// </summary>
        /// <typeparam name="DataModel">The type of data model to be returned.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to be executed.</param>
        /// <param name="filter">An object containing filtering criteria.</param>
        /// <param name="serverOperation">Indicates if the pagination is performed server-side. If true, pageNumber and pageSize must be specified.</param>
        /// <param name="pageNumber">The number of the page to be retrieved. Relevant only if serverOperation is set to true.</param>
        /// <param name="pageSize">The size of the page to be retrieved. Relevant only if serverOperation is set to true.</param>
        /// <returns>A tuple containing a list of data, the total number of rows, and response information.</returns>
        /// <exception cref="SqlException">Thrown when there's a SQL-specific exception during the database operation.</exception>
        /// <exception cref="Exception">Thrown when there's a generic exception during the operation.</exception>
        public async Task<(IEnumerable<DataModel> Data, int TotalRows, ResponseInformationModel ResponseInfo)> GridAsync<DataModel>(
                                    string storedProcedureName,
                                    object filter)
                                    where DataModel : new()
        {
            var responseInfo = new ResponseInformationModel();
            List<DataModel> resultList = new List<DataModel>();
            int totalCount = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Standard parameters.
                        HttpContextAccessor _accessor = new HttpContextAccessor();
                        var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
                        cmd.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "1");

                        string cacheKey = $"GridProperties_{filter.GetType().Name}";

                        if (!_cache.TryGetValue(cacheKey, out List<PropertyInfo> properties))
                        {
                            properties = filter.GetType().GetProperties()
                                .Where(p => p.GetCustomAttribute<SkipAttribute>() == null)
                                .ToList();

                            _cache.Set(cacheKey, properties);
                        }

                        // Bind properties to command parameters
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(filter);
                            cmd.Parameters.AddWithValue($"@{prop.Name}", value ?? DBNull.Value);
                        }

                        if (cmd.Parameters.Contains("@ServerOperation") &&
                            Convert.ToBoolean(cmd.Parameters["@ServerOperation"].Value) &&
                            (cmd.Parameters["@PageNumber"].Value == DBNull.Value || cmd.Parameters["@PageSize"].Value == DBNull.Value))
                        {
                            responseInfo.HasError = true;
                            responseInfo.ErrorMessage = "DatabaseOperations.Grid Error: If ServerOperation = true, you need to specify PageNumber and PageSize values. Operation aborted.";
                            return (resultList, totalCount, responseInfo);
                        }

                        // Execute command and read results.
                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            var fieldNames = new HashSet<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                fieldNames.Add(reader.GetName(i));
                            }

                            while (await reader.ReadAsync())
                            {
                                if (reader is not null)
                                {
                                    var item = Activator.CreateInstance<DataModel>();

                                    foreach (var prop in item.GetType().GetProperties())
                                    {
                                        if (fieldNames.Contains(prop.Name))
                                        {
                                            if (reader[prop.Name] == DBNull.Value)
                                            {
                                                if (prop.PropertyType == typeof(string))
                                                {
                                                    prop.SetValue(item, "-");
                                                }
                                                // Handle other DBNull cases for non-string properties if needed
                                            }
                                            else
                                            {
                                                if (prop.PropertyType == typeof(string))
                                                {
                                                    prop.SetValue(item, reader[prop.Name].ToString());
                                                }
                                                else
                                                {
                                                    prop.SetValue(item, reader[prop.Name]);
                                                }
                                            }
                                        }
                                    }
                                    resultList.Add(item);
                                }

                            }

                            // Read total count from the second result set.
                            if (await reader.NextResultAsync() && await reader.ReadAsync())
                            {
                                totalCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                return (resultList, totalCount, responseInfo);
            }
            catch (SqlException sqlEx)
            {
                // Log SQL specific exceptions.
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "GridAsync.txt", sqlEx.ToString());
                responseInfo.HasError = true;
                responseInfo.ErrorCode = sqlEx.Number.ToString();
                responseInfo.ErrorMessage = sqlEx.Message;
            }
            catch (Exception ex)
            {
                // Log generic exceptions.
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "GridAsync.txt", ex.ToString());
                responseInfo.HasError = true;
                responseInfo.ErrorCode = ex.Message;
                responseInfo.ErrorMessage = ex.Message;
            }

            return (resultList, totalCount, responseInfo);
        }

        /// <summary>
        /// Retrieves a DataTable from the database based on the given filter.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to be executed.</param>
        /// <param name="filter">An object containing filtering criteria.</param>
        /// <returns>A tuple containing a DataTable, the total number of rows, and response information.</returns>
        /// <exception cref="SqlException">Thrown when there's a SQL-specific exception during the database operation.</exception>
        /// <exception cref="Exception">Thrown when there's a generic exception during the operation.</exception>
        public async Task<(DataTable Data, int TotalRows)> GridDataAsync(
           string storedProcedureName,
           List<KeyValuePair<string, object>> parameters)
        {
            var dataTable = new DataTable();
            int totalCount = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Retrieve names of parameters required by the stored procedure (case-insensitive)
                    var requiredParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (SqlCommand getParamsCmd = new SqlCommand($"SELECT NAME FROM sys.parameters WHERE object_id = OBJECT_ID(@procName)", conn))
                    {
                        getParamsCmd.Parameters.AddWithValue("@procName", storedProcedureName);

                        using (SqlDataReader paramReader = await getParamsCmd.ExecuteReaderAsync())
                        {
                            while (paramReader.Read())
                            {
                                // Add parameter names in lowercase to avoid case sensitivity issues
                                requiredParams.Add(paramReader["NAME"].ToString().TrimStart('@').ToLowerInvariant());
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        HttpContextAccessor _accessor = new HttpContextAccessor();
                        var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
                        cmd.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "1");
                        requiredParams.Remove("IdLanguage".ToLowerInvariant());

                        // Add provided parameters and remove them from the required list (case-insensitive)
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                            requiredParams.Remove(param.Key.ToLowerInvariant());
                        }

                        // Add default values for missing required parameters
                        foreach (var paramName in requiredParams)
                        {
                            cmd.Parameters.AddWithValue($"@{paramName}", DBNull.Value);
                        }


                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // Manually populate DataTable
                            if (reader.HasRows)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                                }

                                while (reader.Read())
                                {
                                    var values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    dataTable.Rows.Add(values);
                                }
                            }

                            // Read total count from the second result set
                            if (await reader.NextResultAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    totalCount = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }

                return (dataTable, totalCount);
            }
            catch (SqlException sqlEx)
            {
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "GridDataAsync.txt", sqlEx.ToString());
                throw;
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "GridDataAsync.txt", ex.ToString());
                throw;
            }
        }


        /// <summary>
        ///  Overload of GridDataTableSimpleAsync accepting additional param for hiding and showing columns in datatable
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="columnNamesToShow"></param>
        /// <returns></returns>
        public async Task<(DataTable Data, int TotalRows)> GridDataTableSimpleAsync(
            string storedProcedureName,
            List<KeyValuePair<string, object>> parameters,
            List<string> columnNamesToShow = null) // Optional parameter for column filtering
        {
            var dataTable = new DataTable();
            int totalCount = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var requiredParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (SqlCommand getParamsCmd = new SqlCommand($"SELECT NAME FROM sys.parameters WHERE object_id = OBJECT_ID(@procName)", conn))
                    {
                        getParamsCmd.Parameters.AddWithValue("@procName", storedProcedureName);

                        using (SqlDataReader paramReader = await getParamsCmd.ExecuteReaderAsync())
                        {
                            while (paramReader.Read())
                            {
                                requiredParams.Add(paramReader["NAME"].ToString().TrimStart('@').ToLowerInvariant());
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        HttpContextAccessor _accessor = new HttpContextAccessor();
                        var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
                        cmd.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "1");
                        requiredParams.Remove("IdLanguage".ToLowerInvariant());

                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                            requiredParams.Remove(param.Key.ToLowerInvariant());
                        }

                        foreach (var paramName in requiredParams)
                        {
                            cmd.Parameters.AddWithValue($"@{paramName}", DBNull.Value);
                        }

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            HashSet<string> columnFilterSet = null;
                            if (columnNamesToShow != null)
                            {
                                columnFilterSet = new HashSet<string>(columnNamesToShow, StringComparer.OrdinalIgnoreCase);
                            }

                            if (reader.HasRows)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (columnFilterSet == null || columnFilterSet.Contains(columnName))
                                    {
                                        dataTable.Columns.Add(columnName, reader.GetFieldType(i));
                                    }
                                }

                                while (reader.Read())
                                {
                                    var values = new object[dataTable.Columns.Count];
                                    for (int i = 0, j = 0; i < reader.FieldCount; i++)
                                    {
                                        if (columnFilterSet == null || columnFilterSet.Contains(reader.GetName(i)))
                                        {
                                            values[j++] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                                        }
                                    }
                                    dataTable.Rows.Add(values);
                                }
                            }

                            // Read total count from the second result set
                            if (await reader.NextResultAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    totalCount = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }

                return (dataTable, totalCount);
            }
            catch (SqlException sqlEx)
            {
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "GridDataTableSimpleAsync.txt", sqlEx.ToString());
                throw;
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "GridDataTableSimpleAsync.txt", ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Executes a specified stored procedure to read and filter data, updating a record of type <typeparamref name="TModel"/> in the database.
        /// </summary>
        /// <typeparam name="TModel">The type of the model that represents the table structure in the database.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute for the read operation.</param>
        /// <param name="model">The instance of <typeparamref name="TModel"/> containing the filter criteria for the read operation.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, which, upon completion,
        /// returns a tuple containing a <see cref="ResponseInformationModel"/> with details about the operation's success or failure,
        /// and an instance of <typeparamref name="TModel"/> with the data read from the database.
        /// </returns>
        public async Task<(ResponseInformationModel, TModel)> ReadFilteredAsync<TModel>(string storedProcedureName, TModel model)
        {
            var response = new ResponseInformationModel();

            // Open a new SQL connection
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fetch the parameters defined in the stored procedure
            var spParameters = await FetchStoredProcedureParameters(storedProcedureName, connection);

            // Initialize the SQL command for the stored procedure
            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Use reflection to get properties of TModel
            var modelType = typeof(TModel);
            var modelKey = modelType.FullName;

            // Try to retrieve model properties from cache, or cache them if not already present
            if (!_cache.TryGetValue(modelKey, out List<PropertyInfo> modelProperties))
            {
                modelProperties = modelType.GetProperties().ToList();
                _cache.Set(modelKey, modelProperties);
            }

            bool idLanguageAdded = false;

            // Check if the stored procedure expects the IdLanguage parameter
            if (spParameters.Any(p => p.ParameterName.Equals("@IdLanguage", StringComparison.OrdinalIgnoreCase)))
            {
                var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
                command.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "default_value");
                idLanguageAdded = true; // Mark that IdLanguage has been added
            }

            // Match and add stored procedure parameters with model properties
            foreach (var spParam in spParameters)
            {
                // Skip adding IdLanguage if it has already been added
                if (spParam.ParameterName.Equals("@IdLanguage", StringComparison.OrdinalIgnoreCase) && idLanguageAdded)
                {
                    continue;
                }

                var matchingProp = modelProperties.FirstOrDefault(p => p.Name.Equals(spParam.ParameterName.TrimStart('@'), StringComparison.OrdinalIgnoreCase));

                if (matchingProp != null)
                {
                    var paramValue = matchingProp.GetValue(model);
                    command.Parameters.AddWithValue(spParam.ParameterName, paramValue ?? DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue(spParam.ParameterName, DBNull.Value);
                }
            }
            try
            {
                // Execute the stored procedure and read the results
                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows) // Check if the reader has any rows
                {

                    if (reader.Read())
                    {
                        // Create a new instance of TModel to hold the data
                        model = Activator.CreateInstance<TModel>();
                        var properties = modelType.GetProperties().ToDictionary(p => p.Name, p => p);

                        // Map the data from the reader to the TModel instance
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            if (properties.TryGetValue(name, out var propertyInfo))
                            {
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                propertyInfo.SetValue(model, value);
                            }
                        }
                    }
                }
                else
                {
                    // Set error response if no rows are returned
                    response.ErrorCode = "-2";
                    response.HasError = true;
                    response.ErrorMessage = "No rows returned.";
                    model = default; // Set model to default value
                }
            }
            catch (SqlException ex)
            {
                // Handle exceptions and log the error
                File.AppendAllText(_dbContextErrorLogsPath + "ReadFilteredAsync.txt", ex.ToString());
                response.HasError = true;
                response.ErrorMessage = "There was an error executing the stored procedure.";
            }
            finally
            {
                // Ensure the connection is closed after executing the command
                connection.Close();
            }

            return (response, model);
        }

        /// <summary>
        /// Removes validation errors from the ModelState that correspond to the input parameters of a specified stored procedure.
        /// </summary>
        /// <remarks>
        /// This method establishes a connection to the database using the provided connection string, retrieves the parameters
        /// for the specified stored procedure, and iterates through them. If a parameter name matches a property name in the
        /// provided model, any validation errors associated with that model property in the ModelState are removed.
        /// </remarks>
        /// <param name="modelState">The ModelState dictionary from which to remove validation errors.</param>
        /// <param name="storedProcedureName">The name of the stored procedure whose input parameters are used for matching model properties.</param>
        /// <param name="model">The model instance whose properties are checked against the stored procedure's parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the method arguments are null.</exception>
        /// <exception cref="ArgumentException">Thrown if the storedProcedureName is null or empty.</exception>

        public void RemoveErrorsBasedOnSpParams(ModelStateDictionary modelState, string storedProcedureName, object model)
        {
            if (modelState == null) throw new ArgumentNullException(nameof(modelState));
            if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentException("Stored procedure name cannot be null or empty", nameof(storedProcedureName));
            if (model == null) throw new ArgumentNullException(nameof(model));

            var modelProperties = model.GetType().GetProperties();
            var spParameters = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(storedProcedureName, connection) { CommandType = CommandType.StoredProcedure };

                // Retrieve stored procedure parameters
                SqlCommandBuilder.DeriveParameters(command);

                foreach (SqlParameter param in command.Parameters)
                {
                    if (param.Direction == ParameterDirection.Input || param.Direction == ParameterDirection.InputOutput)
                    {
                        string paramName = param.ParameterName.Replace("@", ""); // Normalize parameter name
                        spParameters.Add(paramName);
                    }
                }
            }

            // Check for model properties that do not match any SP parameter
            foreach (var prop in modelProperties)
            {
                if (!spParameters.Contains(prop.Name) && modelState.ContainsKey(prop.Name))
                {
                    modelState.Remove(prop.Name); // Remove the ModelState errors for non-matching properties
                }
            }
        }





        /// <summary>
        /// Executes a specified stored procedure to read and filter data, updating a record of type <typeparamref name="TReturnModel"/> in the database.
        /// </summary>
        /// <typeparam name="TFilterModel">The type of the model that represents the filter criteria.</typeparam>
        /// <typeparam name="TReturnModel">The type of the model that represents the table structure in the database and the data to be returned.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute for the read operation.</param>
        /// <param name="filterModel">The instance of <typeparamref name="TFilterModel"/> containing the filter criteria for the read operation.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, which, upon completion,
        /// returns a tuple containing a <see cref="ResponseInformationModel"/> with details about the operation's success or failure,
        /// and an instance of <typeparamref name="TReturnModel"/> with the data read from the database.
        /// </returns>
        public async Task<(ResponseInformationModel, TReturnModel)> ReadDifferentFilterModelAsync<TFilterModel, TReturnModel>(string storedProcedureName, TFilterModel filterModel)
        {
            var response = new ResponseInformationModel();

            // Open a new SQL connection
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Fetch the parameters defined in the stored procedure
            var spParameters = await FetchStoredProcedureParameters(storedProcedureName, connection);

            // Initialize the SQL command for the stored procedure
            using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Use reflection to get properties of TFilterModel
            var filterModelType = typeof(TFilterModel);
            var filterModelKey = filterModelType.FullName;

            // Try to retrieve filter model properties from cache, or cache them if not already present
            if (!_cache.TryGetValue(filterModelKey, out List<PropertyInfo> filterModelProperties))
            {
                filterModelProperties = filterModelType.GetProperties().ToList();
                _cache.Set(filterModelKey, filterModelProperties);
            }

            bool idLanguageAdded = false;

            // Check if the stored procedure expects the IdLanguage parameter
            if (spParameters.Any(p => p.ParameterName.Equals("@IdLanguage", StringComparison.OrdinalIgnoreCase)))
            {
                var sessionLang = _accessor.HttpContext.Session.GetString("IdLanguage");
                command.Parameters.AddWithValue("@IdLanguage", sessionLang ?? "default_value");
                idLanguageAdded = true; // Mark that IdLanguage has been added
            }

            // Match and add stored procedure parameters with filter model properties
            foreach (var spParam in spParameters)
            {
                // Skip adding IdLanguage if it has already been added
                if (spParam.ParameterName.Equals("@IdLanguage", StringComparison.OrdinalIgnoreCase) && idLanguageAdded)
                {
                    continue;
                }

                var matchingProp = filterModelProperties.FirstOrDefault(p => p.Name.Equals(spParam.ParameterName.TrimStart('@'), StringComparison.OrdinalIgnoreCase));

                if (matchingProp != null)
                {
                    var paramValue = matchingProp.GetValue(filterModel);
                    command.Parameters.AddWithValue(spParam.ParameterName, paramValue ?? DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue(spParam.ParameterName, DBNull.Value);
                }
            }

            TReturnModel model = default;

            try
            {
                // Execute the stored procedure and read the results
                using var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows) // Check if the reader has any rows
                {
                    var returnModelType = typeof(TReturnModel);

                    if (reader.Read())
                    {
                        // Create a new instance of TReturnModel to hold the data
                        model = Activator.CreateInstance<TReturnModel>();
                        var properties = returnModelType.GetProperties().ToDictionary(p => p.Name, p => p);

                        // Map the data from the reader to the TReturnModel instance
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            if (properties.TryGetValue(name, out var propertyInfo))
                            {
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                propertyInfo.SetValue(model, value);
                            }
                        }
                    }
                }
                else
                {
                    // Set error response if no rows are returned
                    response.ErrorCode = "-2";
                    response.HasError = true;
                    response.ErrorMessage = "No rows returned.";
                    model = default; // Set model to default value
                }
            }
            catch (SqlException ex)
            {
                // Handle exceptions and log the error
                File.AppendAllText(_dbContextErrorLogsPath + "ReadDifferentFilterModelAsync.txt", ex.ToString());
                response.HasError = true;
                response.ErrorMessage = "There was an error executing the stored procedure.";
            }
            finally
            {
                // Ensure the connection is closed after executing the command
                connection.Close();
            }

            return (response, model);
        }


        /// <summary>
        /// Retrieves a DataTable from the database based on the given filter.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to be executed.</param>
        /// <param name="filter">An object containing filtering criteria.</param>
        /// <returns>A tuple containing a DataTable, the total number of rows, and response information.</returns>
        /// <exception cref="SqlException">Thrown when there's a SQL-specific exception during the database operation.</exception>
        /// <exception cref="Exception">Thrown when there's a generic exception during the operation.</exception>
        public async Task<DataTable> QueryResultToDatatable(
           string storedProcedureName,
           List<KeyValuePair<string, object>> parameters)
        {
            var dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Retrieve names of parameters required by the stored procedure (case-insensitive)
                    var requiredParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (SqlCommand getParamsCmd = new SqlCommand($"SELECT NAME FROM sys.parameters WHERE object_id = OBJECT_ID(@procName)", conn))
                    {
                        getParamsCmd.Parameters.AddWithValue("@procName", storedProcedureName);

                        using (SqlDataReader paramReader = await getParamsCmd.ExecuteReaderAsync())
                        {
                            while (paramReader.Read())
                            {
                                // Add parameter names in lowercase to avoid case sensitivity issues
                                requiredParams.Add(paramReader["NAME"].ToString().TrimStart('@').ToLowerInvariant());
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add provided parameters and remove them from the required list (case-insensitive)
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                            requiredParams.Remove(param.Key.ToLowerInvariant());
                        }

                        // Add default values for missing required parameters
                        foreach (var paramName in requiredParams)
                        {
                            cmd.Parameters.AddWithValue($"@{paramName}", DBNull.Value);
                        }


                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // Manually populate DataTable
                            if (reader.HasRows)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                                }

                                while (reader.Read())
                                {
                                    var values = new object[reader.FieldCount];
                                    reader.GetValues(values);
                                    dataTable.Rows.Add(values);
                                }
                            }
                        }
                    }
                }

                return dataTable;
            }
            catch (SqlException sqlEx)
            {
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "QueryResultToDatatable.txt", sqlEx.ToString());
                throw;
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync(_dbContextErrorLogsPath + "QueryResultToDatatable.txt", ex.ToString());
                throw;
            }
        }

    }
}
