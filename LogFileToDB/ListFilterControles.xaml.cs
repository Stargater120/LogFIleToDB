using Core;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogFileToDB
{
    /// <summary>
    /// Interaktionslogik für ListFilterControle.xaml
    /// </summary>
    public partial class ListFilterControles : UserControl
    {
        public event EventHandler FilterSelected;
        public LogEntriesFilter logEntries;
        public ListFilterControles()
        {
            InitializeComponent();
            MethodePicker.ItemsSource = DisplayedLists._methodEntries;
            StatusPicker.ItemsSource = DisplayedLists._statusEntries;
            logEntries = new LogEntriesFilter();
        }

        private void MethodePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StatusPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Search(object sender, RoutedEventArgs e)
        {
            var emit = new EmitEvent();
            emit.logEntries = logEntries;
            FilterSelected(this, emit);
        }
    }
}
