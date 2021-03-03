using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Common
{
    public class ValidationAttribute_RKSID : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Validation_RKSID.IsValid(value?.ToString()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(_Resources.Common.ValidationKosovoPersonalNoAlgo);
            }
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-rksid", _Resources.Common.ValidationKosovoPersonalNoAlgo);
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
