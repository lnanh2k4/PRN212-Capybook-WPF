using Capybook.Models;
using Capybook.Utilities;
using Capybook.Views.Pages.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Capybook.ViewModels
{
    public class OrderVM : BaseVM
    {
        public ObservableCollection<Order> Orders { get; set; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ViewDetailCommand { get; }

        public OrderVM()
        {
            Orders = new ObservableCollection<Order>();
            load();
            NewItem = new Order();
            AddCommand = new RelayCommand(ADD);
            UpdateCommand = new RelayCommand(UPDATE);
            DeleteCommand = new RelayCommand(DELETE);
            ClearCommand = new RelayCommand(CLEAR);
            ViewDetailCommand = new RelayCommand(ViewDetail);

        }

        private void ADD(object obj)
        {
            throw new NotImplementedException();
        }

        private void UPDATE(object obj)
        {
            throw new NotImplementedException();
        }

        private void DELETE(object obj)
        {
            throw new NotImplementedException();
        }

        private void CLEAR(object obj)
        {
            throw new NotImplementedException();
        }
        private void ViewDetail(object parameter)
        {
            if (parameter is int orderId)
            {
                // Gọi hàm để mở trang OrderDetail và truyền OrderId
                OpenOrderDetail(orderId);
            }
        }

        private void OpenOrderDetail(int orderId)
        {
            var orderDetailWindow = new OrderDetailWindow(orderId); 
            orderDetailWindow.Show();
        }


        public void load()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var items = context.Orders
                    .Include(o => o.UsernameNavigation) 
                    .ToList();
                Orders.Clear();
                foreach (var item in items)
                {
                    if (item.OrderStatus != 0)
                    {
                        Orders.Add(item);
                    }
                }
            }

        }

        private Order _selectedItem;
        public Order SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    NewItem = new Order
                    {
                        OrderDate = _selectedItem.OrderDate,
                        OrderStatus = _selectedItem.OrderStatus,
                        OrderDetails = _selectedItem.OrderDetails,
                        OrderId = _selectedItem.OrderId,
                        StaffId = _selectedItem.StaffId,
                        VouId = _selectedItem.VouId,
                        Username = _selectedItem.Username,
                        UsernameNavigation = _selectedItem.UsernameNavigation
                    };
                    OnPropertyChanged(nameof(NewItem));
                }


            }
        }

        private Order _newItem;
        public Order NewItem
        {
            get
            {
                return _newItem;
            }
            set
            {
                _newItem = value;
                OnPropertyChanged(nameof(NewItem));
            }
        }

    }
}
