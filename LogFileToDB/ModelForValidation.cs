using System.ComponentModel;

namespace LogFileToDB
{
    internal class ModelForValidation : IDataErrorInfo
    {
        //members are bound to Textboxes through Data Binding
        //date regex: ^[0-9]{2}[.:-][0-9]{2}[.:-][0-9]{4}\s[0-9]{2}[.:-][0-9]{2}[.:-][0-9]{2}$
        public string ValidateBegin { get; set; }
        public string ValidateEnd { get; set; }
        public ModelForValidation() { }

        public string this[string columnName] => throw new System.NotImplementedException();

        public string Error => throw new System.NotImplementedException();
    }
}
