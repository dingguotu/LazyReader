using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// BookWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BookWindow : Window
    {
        public static double lineHeight = 23;
        public string baseDomain = string.Empty;
        public string path = string.Empty;
        public string bookName = string.Empty;

        private static int rowCharCount = 0;
        private static int pageCharCount = 0;
        private static Stack<ReadLog> readLogs = new Stack<ReadLog>();

        private int curBlockReadCharIndex = 0;
        private int nextBlockReadCharIndex = 0;
        private string bookText = string.Empty;
        private int bookSize = 0;

        public BookWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

            curBlockReadCharIndex = 0;
            nextBlockReadCharIndex = 0;
            textBox.Text = String.Empty;
            PrintNextBlockText();
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && curBlockReadCharIndex > 0)
            {
                textBox.Text = String.Empty;
                if (readLogs.Any())
                {
                    var log = readLogs.Pop();
                    curBlockReadCharIndex = log.CurPageBlockIndex;
                    nextBlockReadCharIndex = log.CurPageBlockIndex;
                    PrintNextBlockText();
                }
                else
                {
                    nextBlockReadCharIndex = curBlockReadCharIndex;
                    curBlockReadCharIndex = nextBlockReadCharIndex - pageCharCount;
                    PrintPrevBlockText();
                }
            }

            if (e.Delta < 0 && nextBlockReadCharIndex < bookSize)
            {
                PushReadLog();
                textBox.Text = String.Empty;
                curBlockReadCharIndex = nextBlockReadCharIndex;
                PrintNextBlockText();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            nextBlockReadCharIndex = curBlockReadCharIndex;
            rowCharCount = (int)(this.ActualWidth / textBox.FontSize);
            pageCharCount = (int)((this.ActualHeight / lineHeight) * (this.ActualWidth / textBox.FontSize) * 2);
            readLogs.Clear();

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
            while (row * lineHeight <= this.ActualHeight)
            {
                colSb.Clear();
                while (MeasureTextSize(colSb.ToString(), textBox).Width <= (this.ActualWidth - textBox.FontSize))
                {
                    if (nextBlockReadCharIndex >= bookSize)
                    {
                        break;
                    }
                    char character = bookText[nextBlockReadCharIndex];
                    nextBlockReadCharIndex++;

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
            var log = new ReadLog();
            log.BookName = bookName;
            log.ActiveTime = DateTime.Now;
            log.Summary = textBox.Text[0..rowCharCount];
            log.CurPageBlockIndex = curBlockReadCharIndex;
            log.NextPageBlockIndex = nextBlockReadCharIndex;
            readLogs.Push(log);
        }

        private void PrintPrevBlockText()
        {
            curBlockReadCharIndex = curBlockReadCharIndex >= 0 ? curBlockReadCharIndex : 0;
            string[] prevBlockText = bookText[curBlockReadCharIndex..nextBlockReadCharIndex]
                                        .ReplaceLineEndings()
                                        .Split(Environment.NewLine);
            int splitLenght = prevBlockText.Length;
            int row = 0;
            int needRow = (int)(this.ActualHeight / lineHeight);
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
            curBlockReadCharIndex = nextBlockReadCharIndex - blockText.Length;
            curBlockReadCharIndex = curBlockReadCharIndex >= 0 ? curBlockReadCharIndex : 0;
            nextBlockReadCharIndex = curBlockReadCharIndex;
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

    public class ReadLog
    {
        public int CurPageBlockIndex { get; set; }
        public int NextPageBlockIndex { get; set; }
        public string? Summary { get; set; }
        public string? BookName { get; set; }
        public DateTime ActiveTime { get; set; }
    }
}
