﻿using System;
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
using Capybook.ViewModels;

namespace Capybook.Views.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for AccountManagement.xaml
    /// </summary>
    public partial class AccountManagement : Page
    {
        public AccountManagement()
        {
            InitializeComponent();
            DataContext = new AccountVM();
        }
    }
}
