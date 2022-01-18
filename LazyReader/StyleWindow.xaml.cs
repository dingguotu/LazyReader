using LazyReader.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace LazyReader
{
    /// <summary>
    /// StyleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StyleWindow : Window
    {
        public StyleWindow()
        {
            InitializeComponent();
            fonts.ItemsSource = Fonts.SystemFontFamilies;
            fontSize.ItemsSource = Enumerable.Range(10, 90);
            lineHeight.ItemsSource = Enumerable.Range(10, 90);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fonts.SelectedItem = new FontFamily(BookWindow.BookWindowStyle.FontFamily);
            fontSize.SelectedItem = BookWindow.BookWindowStyle.FontSize;
            lineHeight.SelectedItem = BookWindow.BookWindowStyle.LineHeight;
        }

        private void fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookWindow.BookWindowStyle.FontFamily = fonts.SelectedItem.ToString();
        }

        private void BtnDefaultFont_Click(object sender, RoutedEventArgs e)
        {
            BookWindow.BookWindowStyle.FontFamily = "Microsoft YaHei";
        }

        private void fontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookWindow.BookWindowStyle.FontSize = (int)fontSize.SelectedItem;
        }

        private void BtnDefaultFontSize_Click(object sender, RoutedEventArgs e)
        {
            BookWindow.BookWindowStyle.FontSize = 14;
        }

        private void lineHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookWindow.BookWindowStyle.LineHeight = (int)lineHeight.SelectedItem;
        }

        private void BtnDefaultLineHeight_Click(object sender, RoutedEventArgs e)
        {
            BookWindow.BookWindowStyle.LineHeight = 23;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            BookWindowStyleVM.SaveToFile(BookWindow.BookWindowStyle);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
