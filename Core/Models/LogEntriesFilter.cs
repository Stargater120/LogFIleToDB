using System.Collections.Generic;
using System.Windows;
using Core.Enums;

namespace Core.Models
{
    public class LogEntriesFilter
    {
        #nullable enable
        public TimeRange? TimeRange { get; set; }
        private List<string>? _ipAdresses { get; set; }
        public List<string>? Methods { get; set; }
        public List<int>? StatusCodes { get; set; }
        public OrderingProperties? OrderBy { get; set; }
        public Order? Order { get; set; }

        public List<string> IPAdresses
        {
            get
            {
                return _ipAdresses;
            }
            set
            {
                if (_ipAdresses is null)
                {
                    _ipAdresses = new List<string>();
                }
                try
                {
                    foreach (string ip in value)
                    {
                        Helper.ValidateIPInput(ip);
                    }
                    _ipAdresses = value;
                }
                catch (System.Exception)
                {
                    MessageBox.Show("IP addresse ist nicht in einer validen form", "IP input error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }
        public void AddIPAddress(string ipAddress)
        {
            if(_ipAdresses is null)
            {
                _ipAdresses = new List<string>();
            }
            try
            {
                Helper.ValidateIPInput(ipAddress);
                _ipAdresses.Add(ipAddress);
            }
            catch (System.Exception)
            {
                MessageBox.Show("IP addresse ist nicht in einer validen form", "IP input error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
