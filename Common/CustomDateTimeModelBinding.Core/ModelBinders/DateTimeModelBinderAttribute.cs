using Microsoft.AspNetCore.Mvc;

namespace SIDPSF.Common.CustomDateTimeModelBinding.Core.ModelBinders
{
    public class DateTimeModelBinderAttribute : ModelBinderAttribute
    {
        public string DateFormat { get; set; }

        public DateTimeModelBinderAttribute()
            : base(typeof(DateTimeModelBinder))
        {
        }
    }
}
