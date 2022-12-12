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

        public MainWindow(CommandRepository repository, QueryRepository queryRepository)
        {
            _repository = repository;
            _queryRepository = queryRepository;
            FillComboBoxes();
            InitializeComponent();
            InitializeLists();
            _queryRepository.GetTimeRangeForFilterAsync();
        }

        private async void FillComboBoxes()
        {
            DisplayedLists._methodEntries.Clear();
            DisplayedLists._statusEntries.Clear();
            DisplayedLists._methodEntries.Add(" ");
            DisplayedLists._statusEntries.Add(" ");
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
            DisplayedLists._logEntrys.Clear();
            await _queryRepository.GetAllLogEntriesAsync();
            await _queryRepository.GetAllAttributeValuesWithCountsAsync(new LogEntriesFilter());
            requestsGrid.ItemsSource = DisplayedLists._logEntrys;
            IP_Datagrid.ItemsSource = DisplayedLists._ipTabEntries;
            Methoden_DataGrid.ItemsSource = DisplayedLists._methodenTabEntries;
            Status_DataGrid.ItemsSource = DisplayedLists._statusTabEntries;
            Files_DataGrid.ItemsSource = DisplayedLists._loadedFilesEntries;
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
                    MessageBox.Show("Die Daten wurden erfolgreich hinzugefügt.", "Daten hinzufügen",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    FillComboBoxes();
                    InitializeLists();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Daten hinzufügen", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ProtokolFilters_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("LogEntry");
            try
            {
                await foreach (var entry in _queryRepository.GetFilteredLogEntriesAsync(e.logEntries))
                {
                    DisplayedLists._logEntrys.Add(entry);
                }
            }
            catch
            {
                MessageBox.Show("Für diesen Filter gibt es keine Ergebnisse.", "Ok");
            }
        }

        private async void IPFilters_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("IPEntries");
            try
            {
                await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(
                                   Core.Enums.OrderingProperties.IP, e.logEntries))
                {
                    DisplayedLists._ipTabEntries.Add(entry);
                }
            }
            catch
            {
                MessageBox.Show("Für diese Filter gibt es keine Ergebnisse");
            }

            IP_Datagrid.ItemsSource = DisplayedLists._ipTabEntries;
        }

        private async void MethodFilters_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("MethodEntries");
            try
            {
                await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(
                                   Core.Enums.OrderingProperties.Method, e.logEntries))
                {
                    DisplayedLists._methodenTabEntries.Add(entry);
                }
            }
            catch
            {
                MessageBox.Show("Für diesen Filter gibt es keine Ergebnisse", "Ok");
            }
        }

        private async void StatusFilter_FilterSelected(object sender, EmitEvent e)
        {
            DisplayedLists.Clear("StatusEntries");
            try
            {
                await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(
                                   Core.Enums.OrderingProperties.Code, e.logEntries))
                {
                    DisplayedLists._statusTabEntries.Add(entry);
                }
            }
            catch
            {
                MessageBox.Show("Für diesen Filter gibt es keine Ergebnisse", "Ok.");
            }
        }

        private async void IP_StackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DisplayedLists._ipTabEntries.Count > 0)
            {
                return;
            }

            await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(
                               Core.Enums.OrderingProperties.IP, new LogEntriesFilter()))
            {
                DisplayedLists._ipTabEntries.Add(entry);
            }
        }

        private async void Methoden_Tab(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DisplayedLists._methodenTabEntries.Count > 0)
            {
                return;
            }

            await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(
                               Core.Enums.OrderingProperties.Method, new LogEntriesFilter()))
            {
                DisplayedLists._methodenTabEntries.Add(entry);
            }
        }

        private async void Status_TabItem_MouseLeftButtonDown(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DisplayedLists._statusTabEntries.Count > 0)
            {
                return;
            }

            await foreach (var entry in _queryRepository.GetAttributeValueWithCountAsync(
                               Core.Enums.OrderingProperties.Code, new LogEntriesFilter()))
            {
                DisplayedLists._statusTabEntries.Add(entry);
            }

            Status_DataGrid.ItemsSource = DisplayedLists._statusTabEntries;
        }

        private async void Files_TabItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DisplayedLists._loadedFilesEntries.Count > 0)
            {
                return;
            }

            await foreach (var entry in _queryRepository.GetAllPreviouslyLoadedFiles())
            {
                DisplayedLists._loadedFilesEntries.Add(entry);
            }
        }
    }
}