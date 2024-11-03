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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Capybook.Views.Pages.Dashboard
{
    /// <summary>
    /// Interaction logic for OrderManagement.xaml
    /// </summary>
    public partial class OrderManagement : Page
    {
        public OrderManagement()
        {
            InitializeComponent();
            DataContext = new OrderVM();
        }

        private int _previousStatusIndex = -1;
        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            int selectedIndex = comboBox.SelectedIndex;

            // If the new index is lower than the previous one, reset it to the previous index
            if (_previousStatusIndex != -1 && selectedIndex < _previousStatusIndex)
            {
                comboBox.SelectedIndex = _previousStatusIndex;
                MessageBox.Show("You can only change the status in next state.", "Selection Restricted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Update the previous index to the newly selected one
                _previousStatusIndex = selectedIndex;
            }
        }
    }
}