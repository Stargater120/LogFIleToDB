using Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core
{
    public class DisplayedLists
    {
        private ObservableCollection<LogEntry> _logEntrys = new ObservableCollection<LogEntry>();

        public ObservableCollection<LogEntry> LogEntrys
        {
            get => _logEntrys;
            set
            {
                _logEntrys = value;
            }
        }

        public void Clear(string listName)
        {
            switch (listName)
            {
                case "LogEntry":
                    _logEntrys.Clear();
                    break;
            }
        }
    }
}
