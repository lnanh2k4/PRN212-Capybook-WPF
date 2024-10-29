using Microsoft.Win32; // Thêm thư viện này để mở file dialog
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
        }

        private void AttachImage_Click(object sender, RoutedEventArgs e)
        {
            // Mở hộp thoại để chọn file hình ảnh
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // Lấy đường dẫn file đã chọn
                string sourceFilePath = openFileDialog.FileName;

                // Tạo đường dẫn đến thư mục ImagesUpload
                string folderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImagesUpload");

                // Kiểm tra và tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Tạo tên file mới duy nhất để tránh trùng lặp
                string fileName = System.IO.Path.GetFileName(sourceFilePath);
                string destinationFilePath = System.IO.Path.Combine(folderPath, fileName);

                // Copy file đến thư mục ImagesUpload
                File.Copy(sourceFilePath, destinationFilePath, true);

                // Hiển thị hình ảnh đã tải lên trong Image control
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(destinationFilePath);
                bitmap.EndInit();
                CoverImage.Source = bitmap;

                // Lưu chuỗi đường dẫn hình ảnh vào cơ sở dữ liệu
                string imagePathInDb = "ImagesUpload/" + fileName;
                SaveImagePathToDatabase(imagePathInDb);
            }
        }

        // Hàm lưu đường dẫn hình ảnh vào database
        private void SaveImagePathToDatabase(string imagePath)
        {
            if (DataContext is BookVM viewModel && viewModel.SelectedBook != null)
            {
                viewModel.SelectedBook.Image = imagePath; // Lưu đường dẫn hình ảnh vào thuộc tính Image của sách
                viewModel.SaveChanges(); // Lưu thông tin vào cơ sở dữ liệu
            }
        }
    }
}
