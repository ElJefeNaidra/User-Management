using System.ComponentModel;

namespace UserManagement.Common.StringLocalisation
{
    /// <summary>
    /// A_Kras: An extension/override of [Display] attribute using a database table to retrieve
    /// label names.
    /// It uses the localisation service. If a string ID is located in the cache then it uses the
    /// cache otherwise it retrieves it from the database.
    /// </summary>

    public static class ServiceLocator
    {
        public static IServiceProvider Current { get; private set; }

        public static void SetCurrent(IServiceProvider serviceProvider)
        {
            Current = serviceProvider;
        }
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DbDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _resourceName;

        public DbDisplayNameAttribute(string resourceName)
        {
            _resourceName = resourceName;
        }

        public override string DisplayName
        {
            get
            {
                // Use IHttpContextAccessor to get the current context and services
                var httpContextAccessor = (IHttpContextAccessor)ActivatorUtilities.GetServiceOrCreateInstance(ServiceLocator.Current, typeof(IHttpContextAccessor));
                var serviceProvider = httpContextAccessor.HttpContext.RequestServices;
                var resourceProvider = serviceProvider.GetRequiredService<DatabaseResourceLocalisationProvider.ResourceRepository>();
                return resourceProvider.GetResourceByName(_resourceName);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DescriptionAttribute : Attribute
    {
        private readonly string _resourceName;

        public DescriptionAttribute(string resourceName)
        {
            _resourceName = resourceName;
        }

        public string Description
        {
            get
            {
                // Ensure the service locator has been initialized and contains the IHttpContextAccessor
                if (ServiceLocator.Current != null)
                {
                    var httpContextAccessor = ServiceLocator.Current.GetService<IHttpContextAccessor>();
                    if (httpContextAccessor?.HttpContext != null)
                    {
                        var resourceProvider = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<DatabaseResourceLocalisationProvider.ResourceRepository>();
                        return resourceProvider.GetResourceByName(_resourceName);
                    }
                }
                return string.Empty;
            }
        }
    }
}