using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using Core;
using Core.Models;

namespace LogFileToDB
{
    public class DatetimepickerValidation : ValidationRule
    {
        private readonly QueryRepository _queryRepository;
        private TimeRange _completeTimeRange;
        public TextBox otherControl { get; set; }
        public DatetimepickerValidation(QueryRepository queryRepository): base()
        {
            _queryRepository = queryRepository;
        }

        public async void GetTimeRange()
        {
            _completeTimeRange = await _queryRepository.GetTimeRangeForFilterAsync();
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string dateString = value.ToString();
            
            if(!DateTime.TryParse(dateString, cultureInfo, DateTimeStyles.None, out DateTime inputDate))
            {
                return new ValidationResult(false, "Bitte gib einen validen String im lokalen Format ein");
            }

            bool validatingBegin = otherControl.Name == "endDate" ? true : false;

            if (validatingBegin)
            {
                string endString = otherControl.Text;
                DateTime endValue = DateTime.Parse(endString, cultureInfo);
                if (endValue < inputDate)
                {
                    return new ValidationResult(false, "Bitte einen Zeitpunkt vor dem Ende wählen");
                }
                if (inputDate < _completeTimeRange.Begin)
                {
                    return new ValidationResult(false, "Dieser Zeitpunkt liegt vor dem Beginn der Analysen");
                }
                if (inputDate > _completeTimeRange.End)
                {
                    return new ValidationResult(false, "Dieser Zeitpunkt liegt nach dem Ende der Analysen");
                }
            }
            else
            {
                return new ValidationResult(true, "Zu erledigen");
            }
            return new ValidationResult(true, "Valid");
        }
    }
}
