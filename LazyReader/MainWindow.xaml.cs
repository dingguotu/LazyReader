using LazyReader.Models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        private void Window_Activated(object sender, EventArgs e)
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
                Book book = new Book()
                {
                    Name = Path.GetFileNameWithoutExtension(sourcePath),
                    BaseDomain = baseDomain,
                    Path = Path.Combine("Books", fileName),
                    LastReadTime = DateTime.Now,
                };

                if (!context.Book.Any(x => fileName.Equals(x.Name) && baseDomain.Equals(x.BaseDomain)))
                {
                    context.Book.Add(book);

                    await context.SaveChangesAsync();
                }

                OpenBook(book);
            }
        }

        private void OpenBook(Book book)
        {
            BookWindow bookWindow = new BookWindow();
            bookWindow.book = book;
            bookWindow.Show();
            bookWindow.Owner = this;

            this.Hide();
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
            if (bookList.SelectedItem == null)
                return;
            Book book = (Book)bookList.SelectedItem;
            context.Book.Remove(book);
            await context.SaveChangesAsync();
            bookList.ItemsSource = context.Book.OrderByDescending(x => x.LastReadTime).ToList();
        }

        private async void bookList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (bookList.SelectedItem == null)
                return;
            Book book = (Book)bookList.SelectedItem;
            OpenBook(book);
            book.LastReadTime = DateTime.Now;
            context.Book.Update(book);
            await context.SaveChangesAsync();
        }
    }
}
