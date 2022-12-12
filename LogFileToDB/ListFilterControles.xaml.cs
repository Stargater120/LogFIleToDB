using Core;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
        public bool DisableFilterButton { get; set; }

        public ListFilterControles()
        {
            InitializeComponent();
            DisableFilterButton = false;
            DataContext = this;
            logEntries = new LogEntriesFilter();
            MethodePicker.ItemsSource = DisplayedLists._methodEntries;
            StatusPicker.ItemsSource = DisplayedLists._statusEntries;
        }

        private void MethodePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logEntries.Method = MethodePicker.SelectedItem as String;
        }

        private void StatusPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StatusPicker.SelectedItem as String))
            {
                logEntries.StatusCode = int.Parse(StatusPicker.SelectedItem as String);
            }
        }

        private void IPInput_EmitIP(object sender, EmitEvent e)
        {
            if (!supressFilterUpdate)
            {
                logEntries.IPAdresses = e.IPAddress;
            }
        }

        private void StartInput_EmitDateTime(object sender, EmitDateTime e)
        {
            logEntries.Begin = e.selectedTime;
            if (logEntries.End.HasValue)
            {
                if (logEntries.End <= logEntries.Begin)
                {
                    DisableFilterButton = true;
                    MessageBox.Show("Das Ende des Zeitraums muss nach dem Anfang sein.");
                }
            }

            if (e.selectedTime >= DisplayedLists.rangeForAnalysis.End)
            {
                DisableFilterButton = true;
                MessageBox.Show(
                    "Mit diesem Anfangszeitpunkt wirst du keine Daten finden, der späteste in der Datenbank vorhandene Zeitpunkt ist: {0}",
                    DisplayedLists.rangeForAnalysis.End.ToLongTimeString());
            }
        }

        private void EndInput_EmitDateTime(object sender, EmitDateTime e)
        {
            logEntries.End = e.selectedTime;
            if (logEntries.Begin.HasValue)
            {
                if (logEntries.End <= logEntries.Begin)
                {
                    DisableFilterButton = true;
                    MessageBox.Show("Das Ende des Zeitraums muss nach dem Anfang sein.");
                }
            }

            if (e.selectedTime <= DisplayedLists.rangeForAnalysis.Begin)
            {
                DisableFilterButton = true;
                MessageBox.Show(
                    "Mit diesem Endzeitpunkt wirst du keine Daten finden, der früheste in der Datenbank vorhandene Zeitpunkt ist: {0}",
                    DisplayedLists.rangeForAnalysis.Begin.ToLongTimeString());
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
            supressFilterUpdate = false;
            LogEntriesFilter emptyFilter = new LogEntriesFilter();
            var emit = new EmitEvent();
            emit.logEntries = emptyFilter;
            FilterSelected(this, emit);
        }
    }
}