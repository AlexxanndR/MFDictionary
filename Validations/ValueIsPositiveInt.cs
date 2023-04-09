using System;
using System.Windows.Controls;

namespace MFDictionary.Validations
{
    internal class ValueIsPositiveInt : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            int number = 0;

            if (!Int32.TryParse(str, out number))
                return new ValidationResult(false, "Incorrect number!");

            if (number <= 0)
                return new ValidationResult(false, "Number must be greater than 0!");

            return ValidationResult.ValidResult;
        }
    }
}
