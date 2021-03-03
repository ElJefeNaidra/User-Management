using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// String Localisation service for RESX resource strings using database storage
/// Last updated: 4th May 2022 Ardian_K.
/// </summary>

namespace SIDPSF.Common.StringLocalisation
{
    public class DatabaseResourceLocalisationProvider
    {
        /// <summary>
        /// Gets the resources by its name. The Name is an unique string identifier.
        /// The data is taken from the table [Administration].[Localisation]
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>

        public class ResourceModel
        {
            public int IdLocalisation { get; set; }
            public string? ResourceName { get; set; }
            public string? EnDescription { get; set; }
            public string? SqDescription { get; set; }
            public string? SrDescription { get; set; }

        }

        /// <summary>
        /// Repository Implementation
        /// </summary>
        public class ResourceRepository
        {
            private readonly string _connectionstring;
            private readonly IMemoryCache _cache;
            private const string LocalisationCachePrefix = "LocalisationCache_";

            public ResourceRepository(string connectionString, IMemoryCache memoryCache)
            {
                _connectionstring = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
                _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

                LoadAllResourcesIntoCache();
            }

            /// <summary>
            /// Retrieves a localized resource by its name. If the resource is not found in cache, it is fetched from the database. 
            /// If it doesn't exist in the database, it is inserted. The resource is returned based on the current language setting.
            /// </summary>
            /// <param name="resourceName">The name of the resource to retrieve.</param>
            /// <returns>The localized string of the resource based on the current language setting.</returns>
            public string GetResourceByName(string resourceName)
            {
                string cacheKey = $"{LocalisationCachePrefix}{resourceName}";
                if (!_cache.TryGetValue(cacheKey, out ResourceModel resource))
                {
                    resource = GetResourceFromDatabase(resourceName);
                    if (resource != null)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(240)
                        };
                        _cache.Set(cacheKey, resource, cacheEntryOptions);
                    }
                }

                // If resource doesnt exist then INSERT it first into the database table
                if (resource == null)
                {
                    InsertMissingResource(resourceName);
                    return resourceName;
                }

                HttpContextAccessor _accessor = new HttpContextAccessor();
                int? idLanguage = Convert.ToInt32(_accessor.HttpContext?.Session.GetString("IdLanguage"));

                switch (idLanguage)
                {
                    case 1:
                        return resource.EnDescription ?? "ResNotExist " + resourceName;
                    case 2:
                        return resource.SqDescription ?? "ResNotExist " + resourceName;
                    case 3:
                        return resource.SrDescription ?? "ResNotExist " + resourceName;
                    case null:
                    case 0:
                        return "ResNotExist " + resourceName;
                    default:
                        return resource.SqDescription ?? "ResNotExist " + resourceName;
                }
            }

            /// <summary>
            /// Loads all resources from the database into the cache. This is typically used for initializing the cache.
            /// </summary>
            private void LoadAllResourcesIntoCache()
            {
                var resources = GetAllResourcesFromDatabase();

                foreach (var resource in resources)
                {
                    _cache.Set(resource.ResourceName, resource);
                }
            }

            /// <summary>
            /// Fetches a resource from the database by its name.
            /// </summary>
            /// <param name="resourceName">The name of the resource to retrieve.</param>
            /// <returns>A <see cref="ResourceModel"/> object containing the resource details, or null if not found.</returns>
            private ResourceModel GetResourceFromDatabase(string resourceName)
            {
                using (SqlConnection conn = new SqlConnection(_connectionstring))
                {
                    conn.Open();

                    string query = "SELECT * FROM [Administration].Localisation WHERE ResourceName = @resourceName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ResourceModel
                                {
                                    IdLocalisation = reader.GetInt32(reader.GetOrdinal("IdLocalisation")),
                                    ResourceName = reader.GetString(reader.GetOrdinal("ResourceName")),
                                    EnDescription = reader.IsDBNull(reader.GetOrdinal("EnDescription")) ? null : reader.GetString(reader.GetOrdinal("EnDescription")),
                                    SqDescription = reader.IsDBNull(reader.GetOrdinal("SqDescription")) ? null : reader.GetString(reader.GetOrdinal("SqDescription")),
                                    SrDescription = reader.IsDBNull(reader.GetOrdinal("SrDescription")) ? null : reader.GetString(reader.GetOrdinal("SrDescription"))
                                };
                            }
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Retrieves all resources from the database.
            /// </summary>
            /// <returns>A list of <see cref="ResourceModel"/> objects containing all resources.</returns>
            private List<ResourceModel> GetAllResourcesFromDatabase()
            {
                List<ResourceModel> resources = new List<ResourceModel>();

                using (SqlConnection conn = new SqlConnection(_connectionstring))
                {
                    conn.Open();

                    string query = "SELECT * FROM [Administration].Localisation";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resources.Add(new ResourceModel
                                {
                                    IdLocalisation = reader.GetInt32(reader.GetOrdinal("IdLocalisation")),
                                    ResourceName = reader.GetString(reader.GetOrdinal("ResourceName")),
                                    EnDescription = reader.IsDBNull(reader.GetOrdinal("EnDescription")) ? null : reader.GetString(reader.GetOrdinal("EnDescription")),
                                    SqDescription = reader.IsDBNull(reader.GetOrdinal("SqDescription")) ? null : reader.GetString(reader.GetOrdinal("SqDescription")),
                                    SrDescription = reader.IsDBNull(reader.GetOrdinal("SrDescription")) ? null : reader.GetString(reader.GetOrdinal("SrDescription"))
                                });
                            }
                        }
                    }
                }
                return resources;
            }

            /// <summary>
            /// Inserts a new resource into the database when a resource with the specified name does not exist.
            /// </summary>
            /// <param name="resourceName">The name of the resource to insert.</param>
            private void InsertMissingResource(string resourceName)
            {
                using (SqlConnection conn = new SqlConnection(_connectionstring))
                {
                    conn.Open();

                    // Adding ResourceName value to multiple columns in the table.
                    string query = "INSERT INTO [Administration].Localisation (ResourceName, EnDescription, SqDescription, SrDescription) VALUES (@resourceName, @resourceName, @resourceName, @resourceName)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@resourceName", resourceName);

                        // Execute the command. No need for a SqlDataReader since we're not reading any results.
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
