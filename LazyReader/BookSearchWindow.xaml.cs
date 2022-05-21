using LazyReader.Models;
using LazyReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class BookSearchWindow : Window
    {
        public static string BookName { get; set; }
        public static string BaseDomain { get; set; }
        public static Guid BookId { get; set; }

        public BookSearchWindow()
        {
            InitializeComponent();
            bookName.Content = BookName;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<SearchResultVM> searchResult = new List<SearchResultVM>();
            if (!string.IsNullOrWhiteSpace(searchWord.Text))
            {
                Regex regex = new Regex($"\\s{searchWord.Text}(.{{0,15}})");
                BookWindow bookWindow = (BookWindow)this.Owner;
                ListSearch.ItemsSource = regex.Matches(bookWindow.bookText)
                                                .Select(x => new SearchResultVM()
                                                {
                                                    Index = x.Index,
                                                    Summary = x.Value.Trim()
                                                })
                                                .ToList();
            }
            else
            {
                ListSearch.ItemsSource = searchResult;
            }
        }

        private void ListSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListSearch.SelectedItem == null)
                return;
            SearchResultVM searchResult = (SearchResultVM)ListSearch.SelectedItem;
            BookWindow bookWindow = (BookWindow)this.Owner;
            bookWindow.GoToIndex(searchResult.Index, BaseDomain, "本地文件");
            this.Close();
        }
    }
}
