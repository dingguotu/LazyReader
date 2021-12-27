using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyReader.ViewModels
{
    public class BookWindowStyleVM : INotifyPropertyChanged
    {
        private double width = 530;
        private double height = 350;
        private double lineHeight = 23;
        private double opacity = 1;
        private double backgroudOpacity = 0.01;
        private double fontSize = 14;
        private string? fontFamily = "Microsoft YaHei";

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
        public double LineHeight
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
        public double FontSize
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

            if (resizeEvent != null && (
                PropertyName == "FontSize" ||
                PropertyName == "FontFamily" ||
                PropertyName == "LineHeight"))
            {
                resizeEvent();
            }
        }
        #endregion
    }
}
