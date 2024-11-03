using Capybook.Models;
using Capybook.Utilities;
using Capybook.Views.Pages.Dashboard;
using Microsoft.EntityFrameworkCore;
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
        private int _orderId;
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public Order NewItem { get; set; }

        public OrderVM()
        {
            Orders = new ObservableCollection<Order>();
            Books = new ObservableCollection<Book>();
            Vouchers = new ObservableCollection<Voucher>();

            LoadOrders();
            LoadBooks();
            LoadVouchers();

            NewItem = new Order();
            AddCommand = new RelayCommand(ADD);
            UpdateCommand = new RelayCommand(UPDATE);
            ClearCommand = new RelayCommand(CLEAR);
            ViewDetailCommand = new RelayCommand(ViewDetail);
        }

        private void LoadOrders()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var sortedOrders = context.Orders
                    .Include(o => o.UsernameNavigation)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                Orders = new ObservableCollection<Order>(sortedOrders);
                OnPropertyChanged(nameof(Orders)); // Notify UI of change
            }
        }



        private void LoadBooks()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                Books.Clear();
                var books = context.Books.ToList();
                foreach (var book in books)
                {
                    Books.Add(book);
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
                        StaffId = _selectedItem.StaffId,
                        VouId = _selectedItem.VouId,
                        Username = _selectedItem.Username,
                        UsernameNavigation = _selectedItem.UsernameNavigation
                    };
                    OnPropertyChanged(nameof(NewItem));
                }
            }
        }

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
