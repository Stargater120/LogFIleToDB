using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core;
using Core.Models;
using Microsoft.Win32;

namespace LogFileToDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandRepository _repository;
        private readonly QueryRepository _queryRepository;
        private ModelForValidation validationClass;

        //holds the min and max values of the time_stamp column to use as placeholders in DateTime filters
        private TimeRange _timeRangeForAnalysis;

        public MainWindow(CommandRepository repository, QueryRepository queryRepository)
        {
            InitializeComponent();

            #region set members

            _repository = repository;
            _queryRepository = queryRepository;
            GetTimeRange();
            DataContext = new ModelForValidation(_timeRangeForAnalysis);

            #endregion set members
        }

        private async void GetTimeRange()
        {
            _timeRangeForAnalysis = await _queryRepository.GetTimeRangeForFilterAsync();
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

        private void validateDateTimeFilterInput(object sender, TextCompositionEventArgs e)
        {
            #region prepare Validation

            TextBox tb = sender as TextBox;
            string fullText = tb.Text + e.Text;
            Regex regexAllowedSymbols = new Regex("^[0-9:.-]+$");
            bool isAllowed = regexAllowedSymbols.IsMatch(e.Text);
            Regex datePattern = new Regex(@"^[0-9]{2}[.:-][0-9]{2}[.:-][0-9]{4}\s[0-9]{2}[:][0-9]{2}[:][0-9]{2}$");
            bool isDateTimeFormat = datePattern.IsMatch(fullText);
            bool hasDateTimeLength = fullText.Length == 19;
            bool hasDateLength = fullText.Length == 9;
            bool isTooLong = fullText.Length > 19;

            #endregion prepare Validation

            #region inform user

            if (!isAllowed)
            {
                MessageBox.Show("Bitte nur folgende Symbole eingeben: [1-9]:.-");
                e.Handled = true;
            }

            if (hasDateLength && !DateTime.TryParse(tb.Text, out DateTime asDate))
            {
                MessageBox.Show("Bitte den Zeitpunkt in einem validen Format eingeben");
            }

            if (hasDateTimeLength && !isDateTimeFormat)
            {
                MessageBox.Show("Bitte den Zeitpunkt in einem validen deutschen Format eingeben");
            }

            if (isTooLong)
            {
                MessageBox.Show("Das ist ein Zeichen zu viel.");
                e.Handled = true;
            }

            #endregion inform user
        }
    }
}