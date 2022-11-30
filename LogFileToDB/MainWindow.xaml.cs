using Core;
using Core.Models;
using Database;
using Database.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandRepository _repository;
        private readonly QueryRepository _queryRepository;
        private readonly DisplayedLists _displayedLists;
        public MainWindow(CommandRepository repository, QueryRepository queryRepository, DisplayedLists displayedLists)
        {
            InitializeComponent();
            _repository = repository;
            _queryRepository = queryRepository;
            _displayedLists = displayedLists;
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

        private void ListFilterControles_FilterSelected(object sender, EventArgs e)
        {

        }

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
