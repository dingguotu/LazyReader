using Microsoft.Win32;
using ReadBookWPF.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;

namespace LazyReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string? TargetDir { get; set; }
        private static LazyReaderContext context = LazyReaderContext.Instance;

        public MainWindow()
        {
            InitializeComponent();
            TargetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Books");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bookList.ItemsSource = context.Book.OrderByDescending(x => x.LastReadTime).ToList();
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string baseDomain = "本地文件";
            // Create OpenFileDialog 
            OpenFileDialog fileDialog = new OpenFileDialog();

            // Set filter for file extension and default file extension 
            fileDialog.DefaultExt = ".txt";
            fileDialog.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            if (fileDialog.ShowDialog() == true)
            {
                string sourcePath = fileDialog.FileName;
                string fileName = Path.GetFileName(sourcePath);
                CopyFileToDir(fileName, sourcePath);

                if (!context.Book.Any(x => fileName.Equals(x.Name) && baseDomain.Equals(x.BaseDomain)))
                {
                    context.Book.Add(new Book()
                    {
                        Name = fileName,
                        BaseDomain = baseDomain,
                        Path = Path.Combine("Books", fileName),
                        LastReadTime = DateTime.Now,
                    });
                    await context.SaveChangesAsync();
                }

                OpenBook(baseDomain, fileName, Path.Combine("Books", fileName));
            }
        }

        private void OpenBook(string baseDomain, string fileName, string path)
        {
            BookWindow bookWindow = new BookWindow();
            bookWindow.baseDomain = baseDomain;
            bookWindow.path = path;
            bookWindow.bookName = fileName;
            bookWindow.Show();

            this.Close();
        }

        public void CopyFileToDir(string fileName, string sourcePath)
        {
            if (!Directory.Exists(TargetDir))
            {
                Directory.CreateDirectory(TargetDir);
            }

            //路径不存在则失败
            if (File.Exists(sourcePath))
            {
                var savePath = Path.Combine(TargetDir, fileName);
                File.Copy(sourcePath, savePath, true);
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Book book = (Book)bookList.SelectedItem;
            context.Book.Remove(book);
            await context.SaveChangesAsync();
            bookList.ItemsSource = context.Book.OrderByDescending(x => x.LastReadTime).ToList();
        }

        private async void bookList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Book book = (Book)bookList.SelectedItem;
            OpenBook(book.BaseDomain, book.Name, book.Path);
            book.LastReadTime = DateTime.Now;
            context.Book.Update(book);
            await context.SaveChangesAsync();
        }
    }
}
