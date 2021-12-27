using LazyReader.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// BookWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BookWindow : Window
    {
        public static BookWindowStyleVM BookWindowStyle { get; set; }
        //public static double windowHeight = 250;
        //public static double windowWidth = 430;
        //public static double lineHeight = 23;

        public string baseDomain = string.Empty;
        public string path = string.Empty;
        public string bookName = string.Empty;

        private static Stack<PageStack> pageStack = new Stack<PageStack>();
        private static LazyReaderContext context = LazyReaderContext.Instance;

        private int rowCharCount = 0;
        private int pageCharCount = 0;
        private int curPageBlockIndex = 0;
        private int nextPageBlockIndex = 0;
        private string bookText = string.Empty;
        private int bookSize = 0;

        public BookWindow()
        {
            InitializeComponent();
            BookWindowStyle = new BookWindowStyleVM();
            string styleJson = string.Empty;
            string stylePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "BookWindowStyle.json");
            if (File.Exists(stylePath))
            {
                using (StreamReader stream = File.OpenText(stylePath))
                {
                    styleJson = stream.ReadToEnd();
                }
            }
            if (!string.IsNullOrWhiteSpace(styleJson))
            {
                BookWindowStyle = JsonSerializer.Deserialize<BookWindowStyleVM>(styleJson);
            }
            BookWindowStyle.ResizeEvent += new BookWindowStyleVM.ResizeWindowEventHandle(ReloadBlockText);

            this.DataContext = BookWindowStyle;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = this.PointToScreen(Mouse.GetPosition(this));
            StyleWindow styleWindow = new StyleWindow();
            styleWindow.Topmost = true;
            styleWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            styleWindow.Top = point.Y - styleWindow.Height;
            styleWindow.Left = point.X;
            styleWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (baseDomain)
            {
                case "本地文件":
                    ReadFile();
                    break;
                default:
                    break;
            }
        }

        private void ReadFile()
        {
            using (StreamReader stream = File.OpenText(path))
            {
                bookText = stream.ReadToEnd();
                bookSize = bookText.Length;
            }

            curPageBlockIndex = 0;
            nextPageBlockIndex = 0;
            textBox.Text = String.Empty;
            PrintNextBlockText();
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

            ReloadBlockText();
        }

        private void ReloadBlockText()
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
        }

        private void PushReadLog()
        {
            var item = new PageStack();
            item.CurPageBlockIndex = curPageBlockIndex;
            item.NextPageBlockIndex = nextPageBlockIndex;
            pageStack.Push(item);
        }

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
    }

    public class PageStack
    {
        public int CurPageBlockIndex { get; set; }
        public int NextPageBlockIndex { get; set; }
    }

    public class ReadHistory
    {
    }
}
