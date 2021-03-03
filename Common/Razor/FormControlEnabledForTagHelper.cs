using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;
using static SIDPSF.Common.DataAccess.DBContext;

namespace SIDPSF.Common.Razor
{
    [HtmlTargetElement("input", Attributes = "asp-for")]
    [HtmlTargetElement("select", Attributes = "asp-for")]
    public class FormControlEnabledForTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public FormControlEnabledForTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var currentCrudOp = _httpContextAccessor.HttpContext.Items["CurrentCrudOp"] as CrudOp?;
            if (currentCrudOp == null) return; // Current operation not set

            var propertyInfo = For.ModelExplorer.Container.ModelType.GetProperty(For.Name);

            // Assume disabling by default
            bool shouldDisable = true;

            // Check if the attribute exists
            var enabledForAttr = propertyInfo?.GetCustomAttribute<EnabledForAttribute>();
            if (enabledForAttr != null)
            {
                // Enable only if the current CrudOp is in the list
                shouldDisable = !enabledForAttr.Operations.Contains(currentCrudOp.Value);
            }

            // Apply the disable attribute based on the final decision
            if (shouldDisable)
            {
                output.Attributes.SetAttribute("disabled", "disabled");
            }
            else
            {
                // Optionally remove the disabled attribute if it was previously set
                output.Attributes.RemoveAll("disabled");
            }

        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EnabledForAttribute : Attribute
    {
        public CrudOp[] Operations { get; }

        public EnabledForAttribute(params CrudOp[] operations)
        {
            Operations = operations;
        }
    }
}