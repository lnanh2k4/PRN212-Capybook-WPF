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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        AccountVM vm = new AccountVM();
        public Login()
        {
            InitializeComponent();
            DataContext = vm;
            var item = DataContext as AccountVM;
            if (item != null) {
                vm.RequestLogin += DoLogin;
            }
        }

        private void DoLogin()
        {
                Window dashboard = new Dashboard(vm.NewItem);
                dashboard.Show();
                this.Close();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Window register = new Register();
            register.ShowDialog();
        }
    }
}
