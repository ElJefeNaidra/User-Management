using System.Reflection;

namespace SIDPSF.Common.Razor
{
    public class ModelDisableFields
    {
        public static void SetDisablePropertiesToTrue(object obj)
        {
            // Get the type of the object
            Type type = obj.GetType();

            // Iterate over all properties of the object
            foreach (PropertyInfo property in type.GetProperties())
            {
                // Check if the property is of type bool and starts with 'disable_'
                if (property.PropertyType == typeof(bool) && property.Name.StartsWith("disable_"))
                {
                    // Set the property value to true
                    property.SetValue(obj, true);
                }
            }
        }
        public static void SetDisablePropertiesToTrueWithNestedModels(object obj)
        {
            // Get the type of the object
            Type type = obj.GetType();

            // Iterate over all properties of the object
            foreach (PropertyInfo property in type.GetProperties())
            {
                // Check if the property is of type bool and starts with 'disable_'
                if (property.PropertyType == typeof(bool) && property.Name.StartsWith("disable_"))
                {
                    // Set the property value to true
                    property.SetValue(obj, true);
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    // Handle nested objects
                    var nestedObj = property.GetValue(obj);
                    if (nestedObj != null)
                    {
                        SetDisablePropertiesToTrueWithNestedModels(nestedObj);
                    }
                }
            }
        }
    }
}
