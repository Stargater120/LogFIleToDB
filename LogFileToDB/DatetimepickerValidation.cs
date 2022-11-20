using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace LogFileToDB
{
    public class DatetimepickerValidation : ValidationRule
    {
        _
        public DatetimepickerValidation(): base()
        {

        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
