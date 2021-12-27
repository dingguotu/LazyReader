﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LazyReader
{
    /// <summary>
    /// StyleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StyleWindow : Window
    {
        public StyleWindow()
        {
            InitializeComponent();
            fonts.ItemsSource = Fonts.SystemFontFamilies;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fonts.SelectedItem = new FontFamily(BookWindow.BookWindowStyle.FontFamily);
        }

        private void fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookWindow.BookWindowStyle.FontFamily = fonts.SelectedItem.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string settingFile = System.IO.Path.Combine(directory, "BookWindowStyle.json");
            if (!File.Exists(settingFile))
            {
                File.Create(settingFile).Close();
            }
            using (StreamWriter sw = new StreamWriter(settingFile, false))
            {
                string json = JsonSerializer.Serialize(BookWindow.BookWindowStyle);
                sw.WriteLine(json);
                sw.Close();
            }
        }
    }
}