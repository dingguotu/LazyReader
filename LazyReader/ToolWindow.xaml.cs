﻿using LazyReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// ToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToolWindow : Window
    {
        public static Book book { get; set; }


        public ToolWindow()
        {
            InitializeComponent();
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Owner.Show();
            this.Owner.Close();
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Close();
            this.Close();
            App.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnChapter_Click(object sender, RoutedEventArgs e)
        {
            BookChapterWindow.BookName = book.Name;
            BookChapterWindow.BookId = book.Id;
            BookChapterWindow bookChapterWindow = new BookChapterWindow();
            bookChapterWindow.Topmost = true;
            bookChapterWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            bookChapterWindow.Top = this.Top;
            bookChapterWindow.Left = this.Left;
            bookChapterWindow.Owner = this.Owner;
            bookChapterWindow.Show();
            this.Close();
        }
    }
}