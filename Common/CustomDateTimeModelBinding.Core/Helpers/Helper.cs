﻿using System;
using System.Globalization;
using System.Linq;

namespace UserManagement.Common.CustomDateTimeModelBinding.Core.Helpers
{
    public static class Helper
    {
        private static readonly string[] CUSTOM_DATE_FORMATS = new string[]
        {
            "yyyyMMddTHHmmssZ",
            "yyyyMMddTHHmmZ",
            "yyyyMMddTHHmmss",
            "yyyyMMddTHHmm",
            "yyyyMMddHHmmss",
            "yyyyMMddHHmm",
            "yyyyMMdd",
            "yyyy-MM-ddTHH-mm-ss",
            "yyyy-MM-dd-HH-mm-ss",
            "yyyy-MM-dd-HH-mm",
            "yyyy-MM-dd",
            "MM-dd-yyyy",
            "dd/MM/yyyy",
            "dd/MM/yyyy hh:mm:ss"
        };

        public static DateTime? ParseDateTime(
            string dateToParse,
            string[] formats = null,
            IFormatProvider provider = null,
            DateTimeStyles styles = DateTimeStyles.None)
        {
            if (formats == null || !formats.Any())
            {
                formats = CUSTOM_DATE_FORMATS;
            }

            DateTime validDate;

            foreach (var format in formats)
            {
                if (format.EndsWith("Z"))
                {
                    if (DateTime.TryParseExact(dateToParse, format, provider, DateTimeStyles.AssumeUniversal, out validDate))
                    {
                        return validDate;
                    }
                }

                if (DateTime.TryParseExact(dateToParse, format, provider, styles, out validDate))
                {
                    return validDate;
                }
            }

            return null;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
