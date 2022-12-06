using Core;
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
        public ListFilterControles()
        {
            InitializeComponent();
            MethodePicker.ItemsSource = DisplayedLists._methodEntrys;
            StatusPicker.ItemsSource = DisplayedLists._statusEntrys;
        }

        //private async void FillComboBoxes()
        //{
        //    var methodList = new List<string>();
        //    await foreach (var entry in MainWindow._queryRepository.GetOptionsForFilter(Core.Enums.OrderingProperties.Method))
        //    {
        //        methodList.Add(entry);
        //    }
        //    MethodePicker.ItemsSource = methodList;
        //}

        private void MethodePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StatusPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    FilterSelected(this, e);
        //}
    }
}
