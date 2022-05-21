using LazyReader.Enums;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace LazyReader.ViewModels
{
    public class BookWindowStyleVM : INotifyPropertyChanged
    {
        private double width = 530;
        private double height = 350;
        private int lineHeight = 23;
        private double opacity = 1;
        private double backgroudOpacity = 0.01;
        private int fontSize = 14;
        private string? fontFamily = "Microsoft YaHei";
        private FontWeight fontWeight = FontWeights.Normal;
        private FontStyle fontStyle = FontStyles.Normal;
        private string? brush = new SolidColorBrush(Colors.Black).ToString();
        private TextDisplayEnum textDisplay = TextDisplayEnum.Normal;
        private bool showInTaskbar = false;
        private bool topmost = true;

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged("Width", null);
            }
        }

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged("Height", null);
            }
        }

        public int LineHeight
        {
            get { return lineHeight; }
            set
            {
                lineHeight = value;
                OnPropertyChanged("LineHeight", GetResizeEvent());
            }
        }

        public double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                OnPropertyChanged("Opacity", null);
            }
        }

        public double BackgroudOpacity
        {
            get { return backgroudOpacity; }
            set
            {
                backgroudOpacity = value;
                OnPropertyChanged("BackgroudOpacity", null);
            }
        }

        public int FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                OnPropertyChanged("FontSize", GetResizeEvent());
            }
        }

        public string? FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                OnPropertyChanged("FontFamily", GetResizeEvent());
            }
        }

        [JsonIgnore]
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                fontWeight = value;
                OnPropertyChanged("FontWeight", GetResizeEvent());
            }
        }

        [JsonIgnore]
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                fontStyle = value;
                OnPropertyChanged("FontStyle", GetResizeEvent());
            }
        }

        public bool IsBold
        {
            get
            {
                return FontWeight == FontWeights.Bold;
            }
            set
            {
                FontWeight = value ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public bool IsItalic
        {
            get
            {
                return FontStyle == FontStyles.Italic;
            }
            set
            {
                FontStyle = value ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        public string Brush
        {
            get { return brush; }
            set
            {
                brush = value.ToString();
                OnPropertyChanged("Brush", null);
            }
        }

        public TextDisplayEnum TextDisplay
        {
            get
            {
                return textDisplay;
            }
            set
            {
                textDisplay = value;
                SaveToFile(this);
            }
        }

        public bool ShowInTaskbar
        {
            get
            {
                return showInTaskbar;
            }
            set
            {
                showInTaskbar = value;
                SaveToFile(this);
            }
        }

        public bool Topmost
        {
            get
            {
                return topmost;
            }
            set
            {
                topmost = value;
                SaveToFile(this);
            }
        }

        #region 属性变化通知事件
        public delegate void ResizeWindowEventHandle();
        public event ResizeWindowEventHandle? ResizeEvent;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 属性变化通知
        /// </summary>
        /// <param name="e"></param>
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public ResizeWindowEventHandle? GetResizeEvent()
        {
            return ResizeEvent;
        }

        public void OnPropertyChanged(string PropertyName, ResizeWindowEventHandle? resizeEvent)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(PropertyName);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (resizeEvent != null)
            {
                resizeEvent();
            }
        }
        #endregion

        public static void SaveToFile(BookWindowStyleVM BookWindowStyle)
        {
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string settingFile = Path.Combine(directory, "StyleSettings.json");
            if (!File.Exists(settingFile))
            {
                File.Create(settingFile).Close();
            }
            using (StreamWriter sw = new StreamWriter(settingFile, false))
            {
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    AllowTrailingCommas = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                string json = JsonSerializer.Serialize(BookWindowStyle, options);
                sw.WriteLine(json);
                sw.Close();
            }
        }

        public static BookWindowStyleVM ReadFile()
        {
            string styleJson = string.Empty;
            string stylePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "StyleSettings.json");
            if (File.Exists(stylePath))
            {
                using (StreamReader stream = File.OpenText(stylePath))
                {
                    styleJson = stream.ReadToEnd();
                }
            }
            if (!string.IsNullOrWhiteSpace(styleJson))
            {
                return JsonSerializer.Deserialize<BookWindowStyleVM>(styleJson);
            }
            else
            {
                return new BookWindowStyleVM();
            }
        }
    }
}
