using Core.Models;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core
{
    public class DisplayedLists
    {
        public static ObservableCollection<LogEntry> _logEntrys = new ObservableCollection<LogEntry>();
        public static ObservableCollection<AttributeWithCount> _ipTabEntries = new ObservableCollection<AttributeWithCount>();
        public static ObservableCollection<AttributeWithCount> _methodenTabEntries = new ObservableCollection<AttributeWithCount>();
        public static ObservableCollection<AttributeWithCount> _statusTabEntries = new ObservableCollection<AttributeWithCount>();
        public static ObservableCollection<LogFile> _loadedFilesEntries = new ObservableCollection<LogFile>();
        public static ObservableCollection<string> _methodEntries = new ObservableCollection<string>();
        public static ObservableCollection<string> _statusEntries = new ObservableCollection<string>();

        public static void Clear(string listName)
        {
            switch (listName)
            {
                case "LogEntry":
                    _logEntrys.Clear();
                    break;
                case "IPEntries":
                    _ipTabEntries.Clear();
                    break;
                case "MethodEntries":
                    _methodenTabEntries.Clear();
                    break;
                case "StatusEntries":
                    _statusTabEntries.Clear();
                    break;
            }
        }
    }
}
