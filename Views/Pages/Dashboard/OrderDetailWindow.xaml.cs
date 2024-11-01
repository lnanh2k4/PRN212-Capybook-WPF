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

namespace Capybook.Views.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for OrderDetailWindow.xaml
    /// </summary>
    public partial class OrderDetailWindow : Window
    {
        public OrderDetailWindow(int orderId)
        {
            InitializeComponent();
            DataContext = new OrderDetailVM(orderId);
        }
    }
}
