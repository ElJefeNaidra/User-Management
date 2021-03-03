using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UserManagement.Common.Razor
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    [HtmlTargetElement("textarea", Attributes = "asp-for")]
    public class AdvancedInputTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var propertyInfo = For.Metadata.ContainerType.GetProperty(For.Metadata.PropertyName);

            // Check for the HiddenInput attribute
            var hiddenInputAttribute = propertyInfo.GetCustomAttribute<HiddenInputAttribute>();
            if (hiddenInputAttribute != null)
            {
                // If HiddenInput attribute is present, do not process further
                return;
            }

            // Check for the DataType attribute to determine if the field is a password
            var dataTypeAttribute = propertyInfo.GetCustomAttribute<DataTypeAttribute>();
            if (dataTypeAttribute != null && dataTypeAttribute.DataType == DataType.Password)
            {
                output.Attributes.SetAttribute("type", "password");
            }
            else if (For.Metadata.ModelType == typeof(string))
            {
                // Set to text if not a password and is a string
                output.Attributes.SetAttribute("type", "text");
            }
            else if (For.Metadata.ModelType == typeof(int) || For.Metadata.ModelType == typeof(int?))
            {
                output.Attributes.SetAttribute("type", "number");
                output.Attributes.SetAttribute("maxlength", "11");
            }

            // Disable autocomplete and spellcheck
            output.Attributes.SetAttribute("autocomplete", "off");
            output.Attributes.SetAttribute("spellcheck", "false");
            output.Attributes.SetAttribute("autocapitalize", "off");
            output.Attributes.SetAttribute("autocorrect", "off");

            // Retrieve StringLength, MaxLength, and MinLength attributes if present for strings
            if (For.Metadata.ModelType == typeof(string) && dataTypeAttribute?.DataType != DataType.Password)
            {
                var stringLength = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                var maxLength = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                var minLength = propertyInfo.GetCustomAttribute<MinLengthAttribute>();

                // Set maxlength and minlength attributes based on the model annotations
                if (stringLength != null)
                {
                    output.Attributes.SetAttribute("maxlength", stringLength.MaximumLength);
                    if (stringLength.MinimumLength > 0)
                    {
                        output.Attributes.SetAttribute("minlength", stringLength.MinimumLength);
                    }
                }
                else
                {
                    if (maxLength != null)
                    {
                        output.Attributes.SetAttribute("maxlength", maxLength.Length);
                    }
                    if (minLength != null)
                    {
                        output.Attributes.SetAttribute("minlength", minLength.Length);
                    }
                }
            }
        }

    }
}