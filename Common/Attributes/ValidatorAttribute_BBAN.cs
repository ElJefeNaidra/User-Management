using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Common
{
    public class ValidationAttribute_BBAN : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Validation_BBAN.IsValid(value?.ToString()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(_Resources.Common.ValidationKosovoBBAN);
            }
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-bban", _Resources.Common.ValidationKosovoBBAN);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
                return true;
            }

            return false;
        }
    }
}
