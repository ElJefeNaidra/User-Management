using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SIDPSF.Common
{
    public class ValidationAttribute_FileTypeMaxSize : ValidationAttribute, IClientModelValidator
    {
        private readonly int _maxFileSize;
        private readonly List<string> _allowedExtensions;

        // Constructor now takes file size in megabytes
        public ValidationAttribute_FileTypeMaxSize(int maxFileSizeInMB, string[] allowedExtensions)
        {
            // Convert megabytes to bytes
            _maxFileSize = maxFileSizeInMB * 1024;
            _allowedExtensions = allowedExtensions.ToList();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage("size"));
                }

                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    return new ValidationResult(GetErrorMessage("type"));
                }
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-filetypemaxsize", _Resources.Common.ValidationFileMaxSizeType);
            MergeAttribute(context.Attributes, "data-val-filetypemaxsize-maxsize", _maxFileSize.ToString());
            MergeAttribute(context.Attributes, "data-val-filetypemaxsize-extensions", string.Join(",", _allowedExtensions));
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }

        private string GetErrorMessage(string errorType)
        {
            return errorType switch
            {
                "size" => $"{_Resources.Common.ValidationMaxFileSize} {_maxFileSize}.",
                "type" => $"{_Resources.Common.ValidationFileType}{string.Join(", ", _allowedExtensions)}.",
                _ => _Resources.Common.ValidationInvalidFile
            };
        }
    }

}