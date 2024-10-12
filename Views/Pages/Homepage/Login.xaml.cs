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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Capybook.Views.Windows;

namespace Capybook.Views.Pages.Homepage
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Window dashboard = new Views.Windows.Dashboard();
            dashboard.Show();
            Window.GetWindow(this).Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Page register = new Register();
            this.NavigationService.Navigate(register);
            
        }
    }
}
