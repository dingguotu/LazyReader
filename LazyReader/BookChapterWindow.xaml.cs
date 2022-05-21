using LazyReader.Models;
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
    /// BookChapterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BookChapterWindow : Window
    {
        public static string BookName { get; set; }
        public static string BaseDomain { get; set; }
        public static Guid BookId { get; set; }
        
        private static LazyReaderContext context = LazyReaderContext.Instance;

        public BookChapterWindow()
        {
            InitializeComponent();
            bookName.Content = BookName;
            ListChapter.ItemsSource = context.BookChapter.Where(x => x.BookId.Equals(BookId)).ToList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchWord.Text))
            {
                ListChapter.ItemsSource = context.BookChapter.Where(x => x.BookId.Equals(BookId) && x.Title.Contains(searchWord.Text.Trim())).ToList();
            }
            else
            {
                ListChapter.ItemsSource = context.BookChapter.Where(x => x.BookId.Equals(BookId)).ToList();
            }
        }

        private void ListChapter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListChapter.SelectedItem == null)
                return;
            BookChapter chapter = (BookChapter)ListChapter.SelectedItem;
            BookWindow bookWindow = (BookWindow)this.Owner;
            bookWindow.GoToIndex(chapter.Index, BaseDomain, chapter.Path);
            this.Close();
        }
    }
}
