using LazyReader.Models;
using LazyReader.ViewModels;
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
        public static string currentIndex { get; set; }

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
            BookChapterWindow.BaseDomain = book.BaseDomain;
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

        private void BtnReadLog_Click(object sender, RoutedEventArgs e)
        {
            ReadLogWindow.BookName = book.Name;
            ReadLogWindow.BookId = book.Id;
            ReadLogWindow.ReadHistories = ((BookWindow)this.Owner).ReadHistories;
            ReadLogWindow readLogWindow = new ReadLogWindow();
            readLogWindow.Topmost = true;
            readLogWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            readLogWindow.Top = this.Top;
            readLogWindow.Left = this.Left;
            readLogWindow.Owner = this.Owner;
            readLogWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StyleWindow styleWindow = new StyleWindow();
            styleWindow.Topmost = true;
            styleWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            styleWindow.Top = this.Top;
            styleWindow.Left = this.Left;
            styleWindow.Owner = this.Owner;
            styleWindow.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Owner.ShowInTaskbar = BookWindow.BookWindowStyle.ShowInTaskbar;
            this.Owner.Topmost = BookWindow.BookWindowStyle.Topmost;
        }

        private void BtnChangeIndex_Click(object sender, RoutedEventArgs e)
        {
            var owner = (BookWindow)this.Owner;
            owner.curPageBlockIndex = int.Parse(currentIndex);
            owner.ReloadBlockText();
            this.Close();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (book.BaseDomain != "本地文件")
            {
                MessageBox.Show("搜索功能只支持本地文件");
                return;
            }
            BookSearchWindow.BookName = book.Name;
            BookSearchWindow.BaseDomain = book.BaseDomain;
            BookSearchWindow.BookId = book.Id;
            BookSearchWindow bookSearchWindow = new BookSearchWindow();
            bookSearchWindow.Topmost = true;
            bookSearchWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            bookSearchWindow.Top = this.Top;
            bookSearchWindow.Left = this.Left;
            bookSearchWindow.Owner = this.Owner;
            bookSearchWindow.Show();
            this.Close();
        }
    }
}
