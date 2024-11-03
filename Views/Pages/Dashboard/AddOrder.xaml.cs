using Capybook.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Capybook.Views.Pages.Dashboard
{
    public partial class AddOrder : Window
    {
        public AddOrder()
        {
            InitializeComponent();
             DataContext = new AddOrderVM(this);
        }
    }
}
