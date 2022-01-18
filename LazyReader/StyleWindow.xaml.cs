using LazyReader.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fonts.SelectedItem = new FontFamily(BookWindow.BookWindowStyle.FontFamily);
        }

        private void fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookWindow.BookWindowStyle.FontFamily = fonts.SelectedItem.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            BookWindowStyleVM.SaveToFile(BookWindow.BookWindowStyle);
        }
    }
}
