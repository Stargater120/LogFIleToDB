using Core;
using Core.Models;
using Microsoft.Win32;
using System;
using System.Windows;

namespace LogFileToDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandRepository _repository;
        public readonly QueryRepository _queryRepository;
        public static LogEntriesFilter entriesFilter = new LogEntriesFilter();
        public MainWindow(CommandRepository repository, QueryRepository queryRepository)
        {
            _repository = repository;
            _queryRepository = queryRepository;
            //FillComboBoxes();
            InitializeComponent();
        }

        private async void FillComboBoxes()
        {
            await foreach (var entry in _queryRepository.GetOptionsForFilter(Core.Enums.OrderingProperties.Method))
            {
                DisplayedLists._methodEntries.Add(entry);
            }
            await foreach (var entry in _queryRepository.GetOptionsForFilter(Core.Enums.OrderingProperties.Code))
            {
                DisplayedLists._statusEntries.Add(entry);
            }
        }

        private async void InitializeLists()
        {
            await _queryRepository.GetAllLogEntriesAsync();
            requestsGrid.ItemsSource = DisplayedLists._logEntrys;
        }

        private async void AddDataThroughFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".log";
            openFileDialog.Filter = "log files (.log) | *.log";
            openFileDialog.CheckFileExists = true;

            bool? result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(filePath);
                try
                {
                    await _repository.CreateLogEntrys(filePath, fileName);
                    MessageBox.Show("Die Daten wurden erfolgreich hinzugefügt.", "Daten hinzufügen");
                }
                catch
                {
                    MessageBox.Show("Diese Datei wurde bereits hinzugefügt", "Daten hinzufügen");
                }
            }
        }

        private async void ProtokolFilters_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("LogEntry");
            await foreach (var entry in _queryRepository.GetFilteredLogEntriesAsync(e.logEntries))
            {
                DisplayedLists._logEntrys.Add(entry);
            }
        }

        private async void IPFilters_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("IPEntries");
            await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(Core.Enums.OrderingProperties.IP, e.logEntries))
            {
                DisplayedLists._ipTabEntries.Add(entry);
            }
        }

        private async void MethodFilters_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("MethodEntries");
            await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(Core.Enums.OrderingProperties.Method, e.logEntries))
            {
                DisplayedLists._methodenTabEntries.Add(entry);
            }
        }

        private async void StatusFilter_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("StatusEntries");
            await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(Core.Enums.OrderingProperties.Code, e.logEntries))
            {
                DisplayedLists._statusTabEntries.Add(entry);
            }
        }

        private void ProtokolFilters_Loaded(object sender, RoutedEventArgs e)
        {

        }


        //private void ListFilterControles_FilterSelected(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var text = e.NewValue as String;
        //}

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    await _queryRepository.GetAllLogEntriesAsync();
        //    Tester_Liste.ItemsSource = _displayedLists.LogEntrys;
        //}

        //private void Button_Click1(object sender, RoutedEventArgs e)
        //{
        //    var templist = _displayedLists.LogEntrys.Skip(15).ToList();
        //    //var temp2 = templist.Skip(15).ToList();
        //    _displayedLists.Clear("LogEntry");

        //    foreach (var entry in templist)
        //    {
        //        _displayedLists.LogEntrys.Add(entry);
        //    }
        //}
    }
}
