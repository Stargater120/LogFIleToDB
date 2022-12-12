using Core;
using Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LogFileToDB
{
    /// <summary>
    /// Interaktionslogik für ListFilterControle.xaml
    /// </summary>
    public partial class ListFilterControles : UserControl
    {
        public event EventHandler<EmitEvent> FilterSelected;
        public LogEntriesFilter logEntries;
        private bool supressFilterUpdate;

        public ListFilterControles()
        {
            InitializeComponent();
            DataContext = this;
            logEntries = new LogEntriesFilter();
            MethodPicker.ItemsSource = DisplayedLists._methodEntries;
            StatusPicker.ItemsSource = DisplayedLists._statusEntries;
            //Filter Button will stay disabled as long as there are no valid Inputs
            Filter_Button.IsEnabled = false;
        }

        private void MethodePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logEntries.Method = MethodPicker.SelectedItem as String;
            Filter_Button.IsEnabled = true;
        }

        private void StatusPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StatusPicker.SelectedItem as String))
            {
                logEntries.StatusCode = int.Parse(StatusPicker.SelectedItem as String);
                Filter_Button.IsEnabled = true;
            }
        }

        private void IPInput_EmitIP(object sender, EmitEvent e)
        {
            if (!supressFilterUpdate)
            {
                logEntries.IPAdresses = e.IPAddress;
                Filter_Button.IsEnabled = true;
            }
        }

        private void StartInput_EmitDateTime(object sender, EmitDateTime e)
        {
            if (e.invalidDate)
            {
                Filter_Button.IsEnabled = false;
                MessageBox.Show("Der Startzeitpunkt enthält invalide Daten.", "Ok.");
            }
            else
            {
                logEntries.Begin = e.selectedTime;
                Filter_Button.IsEnabled = true;
                if (logEntries.End.HasValue)
                {
                    if (logEntries.End <= logEntries.Begin)
                    {
                        MessageBox.Show("Das Ende des Zeitraums muss nach dem Anfang sein.");
                        Filter_Button.IsEnabled = false;
                    }
                }

                if (e.selectedTime >= DisplayedLists.rangeForAnalysis.End)
                {
                    MessageBox.Show(
                        $"Mit diesem Anfangszeitpunkt wirst du keine Daten finden, der späteste in der Datenbank vorhandene Zeitpunkt ist: {DisplayedLists.rangeForAnalysis.End.ToLongTimeString()}",
                        "Ok"
                    );
                    Filter_Button.IsEnabled = false;
                }
            }
        }

        private void EndInput_EmitDateTime(object sender, EmitDateTime e)
        {
            if (e.invalidDate)
            {
                Filter_Button.IsEnabled = false;
                MessageBox.Show("Der Startzeitpunkt enthält invalide Daten.", "Ok.");
            }
            else
            {
                logEntries.End = e.selectedTime;
                Filter_Button.IsEnabled = true;
                if (logEntries.Begin.HasValue)
                {
                    if (logEntries.End <= logEntries.Begin)
                    {
                        MessageBox.Show("Das Ende des Zeitraums muss nach dem Anfang sein.");
                        Filter_Button.IsEnabled = false;
                    }
                }

                if (e.selectedTime <= DisplayedLists.rangeForAnalysis.Begin)
                {
                    MessageBox.Show(
                        $"Mit diesem Endzeitpunkt wirst du keine Daten finden, der früheste in der Datenbank vorhandene Zeitpunkt ist: {DisplayedLists.rangeForAnalysis.Begin.ToLongTimeString()}",
                        "Ok."
                    );
                    Filter_Button.IsEnabled = false;
                }
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            var emit = new EmitEvent();
            emit.logEntries = logEntries;
            FilterSelected(this, emit);
        }

        private void Clear_Search(object sender, RoutedEventArgs e)
        {
            supressFilterUpdate = true;
            IpInput.Clear();
            StartInput.Clear();
            EndInput.Clear();
            StatusPicker.SelectedIndex = -1;
            MethodPicker.SelectedIndex = -1;
            supressFilterUpdate = false;
            logEntries = new LogEntriesFilter();
            var emit = new EmitEvent();
            emit.logEntries = logEntries;
            FilterSelected(this, emit);
        }
    }
}