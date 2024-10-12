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
        public Dashboard()
        {
            InitializeComponent();
        }

        private void stytleBtn(Button parameter)
        {
            parameter.Background = new SolidColorBrush(Colors.DodgerBlue);
            parameter.HorizontalContentAlignment = HorizontalAlignment.Right;
            parameter.FontWeight = FontWeights.Bold;
            parameter.Foreground = new SolidColorBrush((Color)Colors.White);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Window homepage = new Views.Windows.Homepage();
            homepage.Show();
            this.Close();
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.AccountManagement();
            stytleBtn(btnAccount);
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.OrderManagement();
            stytleBtn(btnOrder);
        }
    }
}
