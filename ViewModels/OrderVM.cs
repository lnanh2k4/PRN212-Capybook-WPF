using Capybook.Models;
using Capybook.Utilities;
using Capybook.Views.Pages.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Capybook.ViewModels
{
    public class OrderVM : BaseVM
    {
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Voucher> Vouchers { get; set; }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand SearchCommand { get; }

        private int _orderId;


        private string _vouName;
        public string VouName
        {
            get => _vouName;
            set
            {
                _vouName = value;
                OnPropertyChanged(nameof(VouName));
            }
        }


        public Order NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                OnPropertyChanged(nameof(NewItem));

                if (_newItem != null && _newItem.VouId.HasValue)
                {
                    var voucher = Vouchers.FirstOrDefault(v => v.VouId == _newItem.VouId.Value);
                    VouName = voucher?.VouName;
                }
                else
                {
                    VouName = null;
                }
            }
        }


        public OrderVM()
        {
            Orders = new ObservableCollection<Order>();
            Books = new ObservableCollection<Book>();
            Vouchers = new ObservableCollection<Voucher>();

            LoadOrders();

            LoadVouchers();

            NewItem = new Order();
            AddCommand = new RelayCommand(ADD);
            UpdateCommand = new RelayCommand(UPDATE);
            ClearCommand = new RelayCommand(CLEAR);
            ViewDetailCommand = new RelayCommand(ViewDetail);
            SearchCommand = new RelayCommand(SearchOrders);
        }

        private void LoadOrders()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var sortedOrders = context.Orders
                    .Include(o => o.UsernameNavigation)
                    .Include(o => o.Vou)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                Orders = new ObservableCollection<Order>(sortedOrders);
                OnPropertyChanged(nameof(Orders));
            }
        }

        private void SearchOrders(object obj)
        {
            int cnt = 0;
            using (var context = new Prn212ProjectCapybookContext())
            {
                var query = context.Orders
                    .Include(o => o.Vou)
                    .AsQueryable();

                if (NewItem.OrderId > 0)
                {
                    query = query.Where(o => o.OrderId == NewItem.OrderId);
                    cnt++;
                }
                if (!string.IsNullOrEmpty(NewItem.Username))
                {
                    query = query.Where(o => o.Username.Contains(NewItem.Username));
                    cnt++;
                }
                if (NewItem.OrderStatus.HasValue)
                {
                    query = query.Where(o => o.OrderStatus == NewItem.OrderStatus);
                    cnt++;
                }

                if (cnt == 0)
                {
                    LoadOrders();
                }
                else
                {
                    var results = query.ToList();
                    Orders.Clear();
                    foreach (var order in results)
                    {
                        Orders.Add(order);
                    }
                }
            }
        }

        private void LoadVouchers()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                Vouchers.Clear();
                var vouchers = context.Vouchers.ToList();
                foreach (var voucher in vouchers)
                {
                    Vouchers.Add(voucher);
                }
            }
        }

        private void ADD(object obj)
        {
            CLEAR(obj);
            var addOrderWindow = new AddOrder();
            var addOrderVM = new AddOrderVM(addOrderWindow);
            addOrderVM.OrderAdded += LoadOrders;
            addOrderWindow.DataContext = addOrderVM;
            addOrderWindow.ShowDialog();
        }

        private void UPDATE(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (SelectedItem != null)
                {
                    var item = context.Orders.Where(x => x.OrderId == _orderId).FirstOrDefault();
                    if (item != null)
                    {
                        item.OrderStatus = NewItem.OrderStatus;
                        context.SaveChanges();
                        LoadOrders();
                    }
                }
            }
        }

        private void CLEAR(object obj)
        {
            SelectedItem = null;
            NewItem = new Order();
            OnPropertyChanged(nameof(NewItem));
            OnPropertyChanged(nameof(SelectedItem));
        }

        private void ViewDetail(object parameter)
        {
            if (parameter is int orderId)
            {
                OpenOrderDetail(orderId);
            }
        }


        private void OpenOrderDetail(int orderId)
        {
            var orderDetailWindow = new OrderDetailWindow(orderId);
            orderDetailWindow.Show();
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
                    _orderId = _selectedItem.OrderId;
                    NewItem = new Order
                    {
                        OrderDate = _selectedItem.OrderDate,
                        OrderStatus = _selectedItem.OrderStatus,
                        OrderDetails = _selectedItem.OrderDetails,
                        OrderId = _selectedItem.OrderId,
                        VouId = _selectedItem.VouId,
                        Username = _selectedItem.Username,
                        UsernameNavigation = _selectedItem.UsernameNavigation
                    };
                    OnPropertyChanged(nameof(NewItem));
                }
            }
        }

        private Order _newItem;


        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
    }
}
