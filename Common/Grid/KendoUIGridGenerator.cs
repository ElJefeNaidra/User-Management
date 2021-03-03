using Kendo.Mvc.UI.Fluent;
using Microsoft.Extensions.Caching.Memory;
using SIDPSF.Common.StringLocalisation;
using System.Data;
using SIDPSF._Resources;
using System.Reflection;
using static SIDPSF.Common.Grid.KendoUIGridGenerator;

namespace SIDPSF.Common.Grid
{
    /// <summary>
    /// Provides extension methods to assist with the generation of Kendo UI Grid columns based on model attributes.
    /// </summary>
    public class KendoUIGridGenerator
    {
        /// <summary>
        /// Specifies that the property should not be displayed as a column in the Kendo UI Grid.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class HideAttribute : Attribute
        {
        }

        /// <summary>
        /// Indicates that the column, corresponding to the decorated property, should have filtering enabled in the Kendo UI Grid.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class FilterableAttribute : Attribute
        {
        }

        /// <summary>
        /// Sets the width of the column corresponding to the decorated property in the Kendo UI Grid.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class WidthAttribute : Attribute
        {
            /// <summary>
            /// Gets the width value for the column.
            /// </summary>
            public int Value { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="WidthAttribute"/> class with the specified width.
            /// </summary>
            /// <param name="width">The width value for the column.</param>
            public WidthAttribute(int width)
            {
                Value = width;
            }
        }

        /// <summary>
        /// Indicates that the column, corresponding to the decorated property, should not be rendered at all
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class SkipRenderAttribute : Attribute
        {
        }

        /// <summary>
        /// Indicates that the column, corresponding to the decorated property, should render as multicheckbox ddl
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class FilterableDdlAttribute : Attribute
        {
        }

        /// <summary>
        /// Indicates that the column, corresponding to the decorated property, is buttons column
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class ButtonsAttribute : Attribute
        {
        }
    }

    public static class KendoGridColumnBuilderExtensions
    {
        /// <summary>
        /// Dynamically generates columns for a Kendo UI Grid based on the properties and custom attributes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the data model.</typeparam>
        /// <param name="columns">The grid column factory used to create columns.</param>
        /// <param name="resourceRepo">The repository for fetching localized resource strings.</param>
        public static void GenerateColumns<T>(
            this GridColumnFactory<T> columns,
            DatabaseResourceLocalisationProvider.ResourceRepository resourceRepo,
            int type,
            IMemoryCache memoryCache) where T : class
        {
            if (resourceRepo == null)
            {
                throw new ArgumentNullException(nameof(resourceRepo));
            }

            var cacheKey = "GeneratedColumnsFor_" + typeof(T).FullName;
            List<PropertyInfo> properties;

            if (!memoryCache.TryGetValue(cacheKey, out properties))
            {
                properties = typeof(T).GetProperties().ToList();

                // Cache the properties for 10 days
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(240)
                };
                memoryCache.Set(cacheKey, properties, cacheEntryOptions);
            }

            foreach (var prop in properties)
            {
                var hideAttribute = prop.GetCustomAttribute<HideAttribute>();
                var filterableAttribute = prop.GetCustomAttribute<FilterableAttribute>();
                var widthAttribute = prop.GetCustomAttribute<WidthAttribute>();
                var skipRenderAttribute = prop.GetCustomAttribute<SkipRenderAttribute>();
                var filterableDdlAttribute = prop.GetCustomAttribute<FilterableDdlAttribute>();
                var buttonsAttribute = prop.GetCustomAttribute<ButtonsAttribute>();

                // Using the resourceRepo to get the title from the database
                string columnTitle = resourceRepo.GetResourceByName(prop.Name);

                if (skipRenderAttribute == null)
                {
                    var column = columns.Bound(prop.Name).Title(columnTitle);


                    // Simple Columns with no filters
                    if (type == 1)
                    {
                        if (buttonsAttribute != null)
                        {
                            column.Width(widthAttribute.Value).Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(false);
                        }
                        else
                        {
                            if (hideAttribute != null)
                            {
                                column.Hidden(true);
                                column.IncludeInMenu(false);
                            }

                            if (widthAttribute != null)
                            {
                                column.Width(widthAttribute.Value);
                            }
                        }
                    }

                    // Simple Columns with no filters and sorting
                    if (type == 1)
                    {
                        if (buttonsAttribute != null)
                        {
                            column.Width(widthAttribute.Value).Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(false);
                        }
                        else
                        {
                            column.Sortable(true);

                            if (hideAttribute != null)
                            {
                                column.Hidden(true);
                                column.IncludeInMenu(false);
                            }

                            if (widthAttribute != null)
                            {
                                column.Width(widthAttribute.Value);
                            }
                        }
                    }

                    // Simple Columns with filters
                    if (type == 3)
                    {
                        if (buttonsAttribute != null)
                        {
                            column.Width(widthAttribute.Value).Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(false);
                        }
                        else
                        {
                            column.Sortable(true);

                            if (hideAttribute != null)
                            {
                                column.Hidden(true);
                                column.IncludeInMenu(false);
                            }

                            if (widthAttribute != null)
                            {
                                column.Width(widthAttribute.Value);
                            }

                            if (filterableAttribute != null)
                            {
                                column.Filterable(f => f.Enabled(true).Extra(false).Operators(o => o.ForString(c => c.Clear().StartsWith(_Resources.Grid.StartsWith).Contains(_Resources.Grid.Contains)))

                                .Extra(false).Messages(m => m.Filter(_Resources.Grid.Filter).Clear(_Resources.Grid.Reset)));
                            }

                            if (filterableDdlAttribute != null)
                            {
                                column.Filterable(f => f.Multi(true).Extra(false).Messages(m => m.Filter(_Resources.Grid.Filter).Info(_Resources.Grid.Lock).SelectedItemsFormat(_Resources.Grid.SelectedItemsFormat).Clear(_Resources.Grid.Reset)));
                            }
                        }

                    }
                }
                else
                {
                    var column = columns.Bound(prop.Name).Title(columnTitle);

                    // Simple Columns with no filters
                    if (type == 1)
                    {
                        if (buttonsAttribute != null)
                        {
                            column.Width(widthAttribute.Value).Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(true);
                        }
                        else
                        {
                            column.ColumnMenu(false);

                            if (hideAttribute != null)
                            {
                                column.Hidden(true);
                                column.IncludeInMenu(false);
                            }

                            if (widthAttribute != null)
                            {
                                column.Width(widthAttribute.Value);
                            }


                            column.IncludeInMenu(true);
                            column.Hidden(true);
                        }
                    }

                    // Simple Columns with no filters and sorting
                    if (type == 1)
                    {
                        if (buttonsAttribute != null)
                        {
                            column.Width(widthAttribute.Value).Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(true);
                        }
                        else
                        {
                            column.ColumnMenu(false);
                            column.Sortable(true);

                            if (hideAttribute != null)
                            {
                                column.Hidden(true);
                                column.IncludeInMenu(false);
                            }

                            if (widthAttribute != null)
                            {
                                column.Width(widthAttribute.Value);
                            }

                            column.IncludeInMenu(true);
                            column.Hidden(true);
                        }
                    }

                    // Simple Columns with filters
                    if (type == 3)
                    {
                        if (buttonsAttribute != null)
                        {
                            column.Width(widthAttribute.Value).Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(false);
                        }
                        else
                        {
                            column.Sortable(true);

                            if (hideAttribute != null)
                            {
                                column.Hidden(true);
                                column.IncludeInMenu(false);
                            }

                            if (widthAttribute != null)
                            {
                                column.Width(widthAttribute.Value);
                            }

                            column.IncludeInMenu(true);

                            if (filterableAttribute != null)
                            {
                                column.Filterable(f => f.Enabled(true).Extra(false).Operators(o => o.ForString(c => c.Clear().StartsWith(_Resources.Grid.StartsWith).Contains(_Resources.Grid.Contains)))

                                .Extra(false).Messages(m => m.Filter(_Resources.Grid.Filter).Clear(_Resources.Grid.Reset)));
                            }

                            if (filterableDdlAttribute != null)
                            {
                                column.Filterable(f => f.Multi(true).Extra(false).Messages(m => m.Filter(_Resources.Grid.Filter).Info(_Resources.Grid.Lock).SelectedItemsFormat(_Resources.Grid.SelectedItemsFormat).Clear(_Resources.Grid.Reset)));
                            }
                        }
                    }
                }
            }
        }

        public static void GenerateGridColumns(
            this GridColumnFactory<dynamic> columns,
            DataTable dataTable,
            DatabaseResourceLocalisationProvider.ResourceRepository resourceRepo
            )
        {
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                string fullColumnName = dataColumn.ColumnName;
                string[] parts = fullColumnName.Split('_');
                string columnName;

                // Determine the base column name
                if (parts[0] == "" && parts.Length > 1) // Column name starts with an underscore
                {
                    columnName = parts[1];  // The actual base column name is the second part
                }
                else
                {
                    columnName = parts[0];  // Base column name is the first part
                }


                var column = columns.Bound(fullColumnName).Title(resourceRepo.GetResourceByName(columnName));

                column.Width("200px");

                if (columnName.Equals("Buttons", StringComparison.OrdinalIgnoreCase))
                {
                    column.Width("50px").Encoded(false).Exportable(false).Locked(true).Filterable(false).Sortable(false).IncludeInMenu(false).ColumnMenu(false);
                }


                // Parse and apply parameters
                for (int i = parts[0] == "" ? 2 : 1; i < parts.Length; i++)
                {
                    string param = parts[i].ToUpperInvariant(); // Convert to uppercase for case-insensitive comparison

                    // Handle width parameter (e.g., W200)
                    if (param.StartsWith("W") && int.TryParse(param.Substring(1), out int width))
                    {
                        column.Width($"{width}px");
                    }

                    if (param == "C")
                    {
                        column.HtmlAttributes(new { style = "text-align: center;" });
                    }

                    // Handle other parameters
                    switch (param)
                    {
                        case "H":  // Hide
                            column.Hidden(true);
                            break;
                        case "W":  // Width
                            string widthString = "";
                            int paramIndex = 1;
                            while (paramIndex < param.Length && char.IsDigit(param[paramIndex]))
                            {
                                widthString += param[paramIndex];
                                paramIndex++;
                            }
                            if (int.TryParse(widthString, out int columnWidth))
                            {
                                column.Width(columnWidth + "px");
                            }
                            break;
                        case "D":  // DropDownListFilter
                                   // Apply settings for DropDownListFilter
                            column.Filterable(f => f.Multi(true));  // Assuming Multi(true) is for DropDownList
                            break;
                        case "F":  // String Filter
                                   // Apply settings for String Filter
                                   // Add code as necessary for string filter
                            break;
                        case "M":  // Menu Column Show
                            column.ColumnMenu(true);
                            break;
                        case "I":  // Include in Menu
                            column.IncludeInMenu(true);
                            break;
                        case "X":  // Include in Menu
                            column.Filterable(false);
                            break;

                            // Add more cases as needed
                    }
                }

                if (columnName.Equals("Buttons", StringComparison.OrdinalIgnoreCase))
                {
                    column.Title(" ");
                }
            }
        }
    }
}
