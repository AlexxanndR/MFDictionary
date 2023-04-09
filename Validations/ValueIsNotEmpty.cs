using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MFDictionary.Validations
{
    internal class ValueIsNotEmpty : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;

            if (String.IsNullOrWhiteSpace(str))
                return new ValidationResult(false, "This field is required!");

            return ValidationResult.ValidResult;
        }
    }
}
