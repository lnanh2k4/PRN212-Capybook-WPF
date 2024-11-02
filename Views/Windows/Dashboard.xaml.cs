using Capybook.Models;
using Capybook.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Capybook.Views.Windows
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        AccountVM accountVM;
        public Dashboard(Account account)
        {
            InitializeComponent();
            if (account != null) {
                accountVM.NewItem = account;
            }
            DataContext = accountVM;
        }

        // Biến để lưu giữ tham chiếu đến nút đã thay đổi màu
        private Button lastStyledButton = null;

        // Hàm để thay đổi màu nút
        private void styleBtn(Button parameter)
        {
            // Nếu có nút trước đó đã được đổi màu, khôi phục trạng thái ban đầu
            if (lastStyledButton != null)
            {
                ResetButtonStyle(lastStyledButton);
            }

            // Thay đổi màu nút hiện tại
            parameter.Background = new SolidColorBrush(Colors.DodgerBlue);
            parameter.HorizontalContentAlignment = HorizontalAlignment.Right;
            parameter.FontWeight = FontWeights.Bold;
            parameter.Foreground = new SolidColorBrush(Colors.White);

            // Lưu lại tham chiếu đến nút đã đổi màu
            lastStyledButton = parameter;
        }

        // Hàm để khôi phục trạng thái ban đầu của nút
        private void ResetButtonStyle(Button parameter)
        {
            parameter.Background = new SolidColorBrush(Colors.LightGray); // Ví dụ màu nền ban đầu
            parameter.HorizontalContentAlignment = HorizontalAlignment.Center; // Căn lề ban đầu
            parameter.FontWeight = FontWeights.Normal; // FontWeight ban đầu
            parameter.Foreground = new SolidColorBrush(Colors.Black); // Màu chữ ban đầu
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Window login = new Views.Windows.Login();
            login.Show();
            this.Close();
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.AccountManagement();
            styleBtn(btnAccount);
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.OrderManagement();
            styleBtn(btnOrder);
        }

        private void btnVoucher_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSupplier_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.SupplierManagement();
            styleBtn(btnSupplier);
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.BookManagement();
            styleBtn(btnBook);
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCategory_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.CategoryManagement();
            styleBtn(btnCategory);
        }
    }
}
