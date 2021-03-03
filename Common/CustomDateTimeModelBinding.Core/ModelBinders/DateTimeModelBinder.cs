using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserManagement.Common.CustomDateTimeModelBinding.Core.Helpers;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UserManagement.Common.CustomDateTimeModelBinding.Core.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        public static readonly Type[] SUPPORTED_TYPES = new Type[] { typeof(DateTime), typeof(DateTime?) };

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            DateTime dateTime;

            var isDate = DateTime.TryParse(value.FirstValue, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out dateTime);

            if (isDate)
            {
                string result = Convert.ToDateTime(value.FirstValue, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);


                var date = DateTime.Parse(result);
                dateTime = date;
                //bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Is not a date");
            }
            else
            {
                ModelBindingResult.Success(null);
            }

            //dateTime = ParseDate(bindingContext, dateToParse);

            if (!isDate)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(dateTime);
            }



            await Task.CompletedTask;
        }

        private DateTime? ParseDate(ModelBindingContext bindingContext, string dateToParse)
        {
            var attribute = GetDateTimeModelBinderAttribute(bindingContext);
            var dateFormat = attribute?.DateFormat;

            if (string.IsNullOrEmpty(dateFormat))
            {
                return Helper.ParseDateTime(dateToParse);
            }

            return Helper.ParseDateTime(dateToParse, new string[] { dateFormat });
        }

        private DateTimeModelBinderAttribute GetDateTimeModelBinderAttribute(ModelBindingContext bindingContext)
        {
            var modelName = GetModelName(bindingContext);

            var paramDescriptor = bindingContext.ActionContext.ActionDescriptor.Parameters
                .Where(x => x.ParameterType == typeof(DateTime?))
                .Where((x) =>
                {
                    // See comment in GetModelName() on why we do this.
                    var paramModelName = x.BindingInfo?.BinderModelName ?? x.Name;
                    return paramModelName.Equals(modelName);
                })
                .FirstOrDefault();

            if (!(paramDescriptor is ControllerParameterDescriptor ctrlParamDescriptor))
            {
                return null;
            }

            var attribute = ctrlParamDescriptor.ParameterInfo
                .GetCustomAttributes(typeof(DateTimeModelBinderAttribute), false)
                .FirstOrDefault();

            return (DateTimeModelBinderAttribute)attribute;
        }

        private string GetModelName(ModelBindingContext bindingContext)
        {
            // The "Name" property of the ModelBinder attribute can be used to specify the
            // route parameter name when the action parameter name is different from the route parameter name.
            // For instance, when the route is /api/{birthDate} and the action parameter name is "date".
            // We can add this attribute with a Name property [DateTimeModelBinder(Name ="birthDate")]
            // Now bindingContext.BinderModelName will be "birthDate" and bindingContext.ModelName will be "date"
            if (!string.IsNullOrEmpty(bindingContext.BinderModelName))
            {
                return bindingContext.BinderModelName;
            }

            return bindingContext.ModelName;
        }
    }
}
