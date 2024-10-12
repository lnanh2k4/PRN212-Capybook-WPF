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

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Window homepage = new Views.Windows.Homepage();
            homepage.Show();
            this.Close();
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            dashboardFrame.Content = new Views.Pages.Dashboard.AccountManagement();
            btnAccount.Background = new SolidColorBrush(Colors.DodgerBlue);
            btnAccount.HorizontalContentAlignment = HorizontalAlignment.Right;
            btnAccount.FontWeight = FontWeights.Bold;
            btnAccount.Foreground = new SolidColorBrush((Color)Colors.White);
        }
    }
}
