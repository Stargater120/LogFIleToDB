using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace Core.Models
{
    public class ModelForValidation : IDataErrorInfo
    {
        //members are bound to Textboxes through Data Binding
        private string Value = DateTime.Now.ToString();
        public string ValidateBegin { get => Value; set => Value = value; }
        public string ValidateEnd { get => Value; set => Value = value; }
        public TimeRange _timeRangeToAnalyse { get; set; }


        private readonly Regex _datePattern = new Regex(@"^[0-9]{2}[.:-][0-9]{2}[.:-][0-9]{4}\s[0-9]{2}[.:-][0-9]{2}[.:-][0-9]{2}$");
        public ModelForValidation(TimeRange timeRange)
        {
            _timeRangeToAnalyse = timeRange;
        }

        public string this[string propertyName]
        {
            get
            {
                
                if (propertyName == "ValidateBegin" )
                {
                    if (!_datePattern.IsMatch(ValidateBegin))
                    {
                        return "Bitte gib den Zeitpunkt in einem validem deutschem Format ein";
                    }

                    if (DateTime.TryParse(ValidateBegin, out DateTime beginDate))
                    {
                        
                        if (beginDate > _timeRangeToAnalyse.End)
                        {
                            return "Es gibt keine Daten nach diesem Zeitpunkt";
                        }

                        if (DateTime.TryParse(ValidateEnd, out DateTime endDate) && beginDate >= endDate) 
                        {
                            return "Bitte wähle ein Enddatum das nach dem Startdatum liegt.";
                        }
                    }
                    else
                    {
                        return "Bitte gib den Zeitpunkt in einem validen Format ein";
                    }
                }
                if (propertyName == "ValidateEnd")
                {
                    if (!_datePattern.IsMatch(ValidateEnd))
                    {
                        return "Bitte gib den Zeitpunkt in einem validem deutschem Format ein";
                    }

                    if (DateTime.TryParse(ValidateEnd, out DateTime endDate))
                    {
                        if (endDate < _timeRangeToAnalyse.Begin)
                        {
                            return "Es gibt keine Daten vor diesem Zeitpunkt";
                        }

                        if (DateTime.TryParse(ValidateBegin, out DateTime beginDate) && beginDate >= endDate)
                        {
                            return "Bitte wähle ein Enddatum das nach dem Startdatum liegt.";
                        }
                    }
                    else
                    {
                        return "Bitte gib den Zeitpunkt in einem validen Format ein";
                    }
                }

                return "";
            }
        }

        public string Error => throw new System.NotImplementedException();      
       
    }
}
