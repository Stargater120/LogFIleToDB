using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

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
        }

        private void FillComboBoxes()
        {
        }

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