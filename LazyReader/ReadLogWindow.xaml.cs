using LazyReader.Models;
using LazyReader.ViewModels;
using System;
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

namespace LazyReader
{
    /// <summary>
    /// ReadLogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReadLogWindow : Window
    {
        public static string BookName { get; set; }
        public static Guid BookId { get; set; }
        public static List<ReadHistoryVM> ReadHistories { get; set; }

        private static LazyReaderContext context = LazyReaderContext.Instance;

        public ReadLogWindow()
        {
            InitializeComponent();
            this.Title = BookName;
            ReadHistories.ForEach(hist => {
                hist.Summary = $"{hist.ReadTime.ToLongTimeString()} 第{hist.CurPageBlockIndex}个字符 {hist.Summary}";
            });
            ListReadLog.ItemsSource = ReadHistories;
        }

        private void ListReadLog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListReadLog.SelectedItem == null)
                return;
            ReadHistoryVM hist = (ReadHistoryVM)ListReadLog.SelectedItem;
            Book? book = context.Book.FirstOrDefault(x => x.Id.Equals(BookId));
            BookWindow bookWindow = (BookWindow)this.Owner;
            bookWindow.GoToIndex(hist.CurPageBlockIndex, book?.BaseDomain, hist.Path);
            this.Close();
        }
    }
}
