﻿using Core;
using System;
using System.Collections.Generic;
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
        public MainWindow(CommandRepository repository)
        {
            InitializeComponent();
            _repository = repository;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var directory = Directory.GetCurrentDirectory();
            var path = System.IO.Path.Combine(directory, "LogFile.txt");
            await _repository.CreateLogEntrys(path);
        }
    }
}
