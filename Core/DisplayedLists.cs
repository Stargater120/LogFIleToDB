using Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core
{
    public class DisplayedLists
    {
        private ObservableCollection<LogEntry> _logEntrys = new ObservableCollection<LogEntry>();
        public ObservableCollection<LogEntry> _ipTabEntrys = new ObservableCollection<LogEntry>();
        public ObservableCollection<LogEntry> _methodenTabEntrys = new ObservableCollection<LogEntry>();
        public ObservableCollection<LogEntry> _statusTabEntrys = new ObservableCollection<LogEntry>();
        public ObservableCollection<LogFile> _loadedFilesEntrys = new ObservableCollection<LogFile>();

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
