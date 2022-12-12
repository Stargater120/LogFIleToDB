using System;
using System.Collections.Generic;
using System.Windows;
using Core.Enums;

namespace Core.Models
{
    public class LogEntriesFilter
    {
#nullable enable
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
        private string? _ipAddress { get; set; }
        public string? Method { get; set; }
        public int? StatusCode { get; set; }
        public OrderingProperties? OrderBy { get; set; }
        public Order? Order { get; set; }

        public string? IPAdresses
        {
            get { return _ipAddress; }
            set
            {
                if (_ipAddress is null)
                {
                    _ipAddress = "";
                }

                try
                {
                    Helper.ValidateIPInput(value);
                    _ipAddress = value;
                }
                catch (System.Exception)
                {
                    MessageBox.Show("IP addresse ist nicht in einer validen form", "IP input error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}