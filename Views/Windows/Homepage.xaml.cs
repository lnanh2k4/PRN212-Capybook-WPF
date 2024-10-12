using Capybook.Views.Pages.Homepage;
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
    /// Interaction logic for Homepage.xaml
    /// </summary>
    public partial class Homepage : Window
    {
        public Homepage()
        {
            InitializeComponent();
        }

       

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            homepageFrame.Content = new Login();
            
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            homepageFrame.Content = new Register();
            
        }

        private void btnHomepage_Click(object sender, RoutedEventArgs e)
        {
            
            homepageFrame.Content = "";
        }
    }
}
