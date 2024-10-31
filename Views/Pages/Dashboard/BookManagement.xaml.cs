using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Capybook.ViewModels;

namespace Capybook.Views.Pages.Dashboard
{
    public partial class BookManagement : Page
    {
        public BookManagement()
        {
            InitializeComponent();
            DataContext = new BookVM();

            // Load image for the selected book when the page is loaded
            if (DataContext is BookVM viewModel && viewModel.TemporaryBook != null)
            {
                LoadImage(viewModel.TemporaryBook.Image);
            }

            // Subscribe to property changes to update the image
            if (DataContext is BookVM vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(vm.TemporaryBook))
                    {
                        LoadImage(vm.TemporaryBook?.Image);
                    }
                };
            }
        }

        private void AttachImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string sourceFilePath = openFileDialog.FileName;
                    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImagesUpload");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Path.GetFileName(sourceFilePath);
                    string uniqueFileName = Guid.NewGuid() + "_" + fileName;
                    string destinationFilePath = Path.Combine(folderPath, uniqueFileName);

                    File.Copy(sourceFilePath, destinationFilePath, true);

                    // Update the TemporaryBook image path
                    if (DataContext is BookVM viewModel)
                    {
                        viewModel.TemporaryBook.Image = "ImagesUpload/" + uniqueFileName;
                        LoadImage(viewModel.TemporaryBook.Image);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error attaching image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadImage(string imagePath)
        {
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);

                if (File.Exists(fullPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    CoverImage.Source = bitmap;
                }
                else
                {
                    CoverImage.Source = null; // Clear the image if the file doesn't exist
                }
            }
            else
            {
                CoverImage.Source = null; // Clear the image if the path is empty
            }
        }
    }
}