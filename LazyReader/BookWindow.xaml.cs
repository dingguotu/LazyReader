using LazyReader.Models;
using LazyReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UtfUnknown;

namespace LazyReader
{
    /// <summary>
    /// BookWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BookWindow : Window
    {
        public static BookWindowStyleVM BookWindowStyle { get; set; }

        public Book book = new Book();
        public List<ReadHistoryVM> ReadHistories { get; set; }

        private static Stack<PageStack> pageStack = new Stack<PageStack>();
        private static LazyReaderContext context = LazyReaderContext.Instance;

        private int rowCharCount = 0;
        private int pageCharCount = 0;
        public int curPageBlockIndex = 0;
        private int nextPageBlockIndex = 0;
        public string bookText = string.Empty;
        private int bookSize = 0;

        public BookWindow()
        {
            InitializeComponent();
            BookWindowStyle = BookWindowStyleVM.ReadFile();            
            BookWindowStyle.ResizeEvent += new BookWindowStyleVM.ResizeWindowEventHandle(ReloadBlockText);
            BookWindowStyle.TextDisplayChangeEvent += new BookWindowStyleVM.TextDisplayChangeEventHandle(ReloadMouseEvent);

            this.DataContext = BookWindowStyle;
            ReloadMouseEvent();
            ReadHistories = new List<ReadHistoryVM>();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = this.PointToScreen(Mouse.GetPosition(this));
            ToolWindow toolWindow = new ToolWindow();
            toolWindow.Topmost = true;
            toolWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            toolWindow.Top = point.Y;
            toolWindow.Left = point.X;
            toolWindow.Owner = this;
            ToolWindow.book = book;
            ToolWindow.currentIndex = curPageBlockIndex.ToString();
            toolWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReadHistory();
            switch (book.BaseDomain)
            {
                case "本地文件":
                    ReadFile();
                    break;
                default:
                    break;
            }
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && curPageBlockIndex > 0)
            {
                textBox.Text = String.Empty;
                if (pageStack.Any())
                {
                    var item = pageStack.Pop();
                    curPageBlockIndex = item.CurPageBlockIndex;
                    nextPageBlockIndex = item.CurPageBlockIndex;
                    PrintNextBlockText();
                }
                else
                {
                    nextPageBlockIndex = curPageBlockIndex;
                    curPageBlockIndex = nextPageBlockIndex - pageCharCount;
                    PrintPrevBlockText();
                }
            }

            if (e.Delta < 0 && nextPageBlockIndex < bookSize)
            {
                PushReadLog();
                textBox.Text = String.Empty;
                curPageBlockIndex = nextPageBlockIndex;
                PrintNextBlockText();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BookWindowStyle.Width = e.NewSize.Width;
            BookWindowStyle.Height = e.NewSize.Height;
            BookWindowStyleVM.SaveToFile(BookWindowStyle);

            ReloadBlockText();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveReadHistory();
        }

        /// <summary>
        /// 加载本地文件
        /// </summary>
        private async void ReadFile()
        {
            Encoding encoding = CharsetDetector.DetectFromFile(book.Path).Detected.Encoding;
            bookText = File.ReadAllText(book.Path, encoding).ReplaceLineEndings().Replace($"{Environment.NewLine}{Environment.NewLine}", Environment.NewLine);
            bookSize = bookText.Length;

            nextPageBlockIndex = curPageBlockIndex;
            textBox.Text = String.Empty;
            PrintNextBlockText();
            await LoadFileChapter();
        }

        /// <summary>
        /// 解析txt文件中的章节信息
        /// </summary>
        /// <returns></returns>
        private async Task LoadFileChapter()
        {
            if (context.BookChapter.Any(x => x.BookId == book.Id))
            {
                return;
            }
            List<BookChapter> bookChapters = new List<BookChapter>();
            Regex reg = new Regex("\\s第(.{0,20})(章|篇)(.*?)" + Environment.NewLine);
            MatchCollection mcs = reg.Matches(bookText);
            foreach (Match mc in mcs)
            {
                var chapter = new BookChapter();
                chapter.BookId = book.Id;
                chapter.Index = mc.Index + 1;
                chapter.Title = mc.Value.ReplaceLineEndings("").Trim();
                chapter.Path = book.Path;
                bookChapters.Add(chapter);
            }
            await context.BookChapter.AddRangeAsync(bookChapters);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 重新加载文字
        /// </summary>
        public void ReloadBlockText()
        {
            nextPageBlockIndex = curPageBlockIndex;
            rowCharCount = (int)(this.ActualWidth / textBox.FontSize);
            pageCharCount = (int)((this.ActualHeight / BookWindowStyle.LineHeight) * (this.ActualWidth / textBox.FontSize) * 2);
            pageStack.Clear();

            if (!string.IsNullOrWhiteSpace(bookText))
            {
                textBox.Text = String.Empty;
                PrintNextBlockText();
            }
        }

        private void ReloadMouseEvent()
        {
            this.MouseLeave -= new MouseEventHandler(HiddenText);
            this.MouseEnter -= new MouseEventHandler(ShowText);
            this.MouseDoubleClick -= new MouseButtonEventHandler(ShowText);
            switch (BookWindowStyle.TextDisplay)
            {
                case Enums.TextDisplayEnum.Normal:
                    textBox.Opacity = BookWindowStyle.Opacity;
                    break;
                case Enums.TextDisplayEnum.MoveUp:
                    this.MouseLeave += new MouseEventHandler(HiddenText);
                    this.MouseEnter += new MouseEventHandler(ShowText);
                    break;
                case Enums.TextDisplayEnum.Dblclick:
                    this.MouseLeave += new MouseEventHandler(HiddenText);
                    this.MouseDoubleClick += new MouseButtonEventHandler(ShowText);
                    break;
                default:
                    break;
            }
        }

        private void HiddenText(object sender, MouseEventArgs args)
        {
            textBox.Opacity = 0;
        }

        private void ShowText(object sender, MouseEventArgs args)
        {
            textBox.Opacity = BookWindowStyle.Opacity;
        }

        public void GoToIndex(int index, string? baseDomain, string path)
        {
            curPageBlockIndex = index;
            nextPageBlockIndex = index;
            pageStack.Clear();

            switch (baseDomain)
            {
                case "本地文件":
                    textBox.Text = String.Empty;
                    PrintNextBlockText();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 绘制当前页
        /// </summary>
        private void PrintNextBlockText()
        {
            double row = 1;
            double column = 1;
            StringBuilder sb = new StringBuilder();
            StringBuilder colSb = new StringBuilder();
            while (row * BookWindowStyle.LineHeight <= (this.ActualHeight - 15))
            {
                colSb.Clear();
                while (MeasureTextSize(colSb.ToString(), textBox).Width <= (this.ActualWidth - textBox.FontSize))
                {
                    if (nextPageBlockIndex >= bookSize)
                    {
                        break;
                    }
                    char character = bookText[nextPageBlockIndex];
                    nextPageBlockIndex++;

                    if (character == '\r')
                    {
                        continue;
                    }
                    else if (character == '\n')
                    {
                        break;
                    }
                    else if (character == ' ')
                    {
                        column += 0.5;
                    }
                    else
                    {
                        column++;
                    }
                    colSb.Append(character);
                }
                sb.Append(colSb);
                sb.Append(Environment.NewLine);
                column = 1;
                row++;
            }
            textBox.Text = sb.ToString();
            ReadHistories.Add(new ReadHistoryVM()
            {
                Path = book.Path,
                CurPageBlockIndex = curPageBlockIndex,
                Summary = textBox.Text[..40].ReplaceLineEndings(""),
                ReadTime = DateTime.Now,
            });
        }

        /// <summary>
        /// 翻页记录
        /// </summary>
        private void PushReadLog()
        {
            var item = new PageStack();
            item.CurPageBlockIndex = curPageBlockIndex;
            item.NextPageBlockIndex = nextPageBlockIndex;
            pageStack.Push(item);
        }

        /// <summary>
        /// 绘制上一页
        /// </summary>
        private void PrintPrevBlockText()
        {
            curPageBlockIndex = curPageBlockIndex >= 0 ? curPageBlockIndex : 0;
            string[] prevBlockText = bookText[curPageBlockIndex..nextPageBlockIndex]
                                        .ReplaceLineEndings()
                                        .Split(Environment.NewLine);
            int splitLenght = prevBlockText.Length;
            int row = 0;
            int needRow = (int)(this.ActualHeight / BookWindowStyle.LineHeight);
            List<string> blockTextSplit = new List<string>();
            for (int i = splitLenght - 1; i >= 0; i--)
            {
                string splitText = prevBlockText[i].Replace("  ", "空");
                int splitRow = (int)Math.Ceiling((double)splitText.Length / rowCharCount);
                if ((row + splitRow) > needRow)
                {
                    int needChar = (needRow - row) * rowCharCount;
                    blockTextSplit.Add(splitText[^needChar..]);
                    break;
                }
                else
                {
                    blockTextSplit.Add(prevBlockText[i]);
                    row += splitRow;
                }
            }
            string blockText = string.Join(Environment.NewLine, blockTextSplit);
            curPageBlockIndex = nextPageBlockIndex - blockText.Length;
            curPageBlockIndex = curPageBlockIndex >= 0 ? curPageBlockIndex : 0;
            nextPageBlockIndex = curPageBlockIndex;
            PrintNextBlockText();
        }

        /// <summary>
        /// 计算文字大小
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        public Size MeasureTextSize(string text, TextBlock textBlock)
        {
            FormattedText ft = new FormattedText(text,
                                                 CultureInfo.CurrentCulture,
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                                                 textBlock.FontSize,
                                                 Brushes.Black,
                                                 1);
            return new Size(ft.Width, ft.Height);
        }

        /// <summary>
        /// 加载阅读记录
        /// </summary>
        private void LoadReadHistory()
        {
            string json = string.Empty;
            string historyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Histories", $"{book.Name}.json");
            if (File.Exists(historyPath))
            {
                json = File.ReadAllText(historyPath);

                BookReadHistoryVM history = JsonSerializer.Deserialize<BookReadHistoryVM>(json);
                curPageBlockIndex = history.LastReadIndex;
                ReadHistories.AddRange(history.ReadHistories);
            }
        }

        /// <summary>
        /// 保存阅读记录
        /// </summary>
        private void SaveReadHistory()
        {
            var history = new BookReadHistoryVM
            {
                Title = book.Name,
                BaseDomain = book.BaseDomain,
                Path = book.Path,
                LastReadIndex = ReadHistories.LastOrDefault().CurPageBlockIndex,
                ReadHistories = ReadHistories
            };

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                AllowTrailingCommas = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
            string json = JsonSerializer.Serialize(history, options);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Histories");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string historyPath = Path.Combine(directory, $"{book.Name}.json");
            FileStream fs = File.Open(historyPath, FileMode.OpenOrCreate);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            fs.Dispose();
        }
    }

    public class PageStack
    {
        public int CurPageBlockIndex { get; set; }
        public int NextPageBlockIndex { get; set; }
    }
}
