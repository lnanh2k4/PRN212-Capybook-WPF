using Capybook.Models;
using Capybook.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Capybook.ViewModels
{
    public class AddOrderVM : BaseVM
    {
        public ObservableCollection<Account> Accounts { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<SelectedBookItem> SelectedBooks { get; set; } = new ObservableCollection<SelectedBookItem>();
        public ObservableCollection<Book> FilteredBooks { get; set; }
        public ObservableCollection<Voucher> Vouchers { get; set; }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
            }
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        private Voucher _selectedVoucher;
        public Voucher SelectedVoucher
        {
            get => _selectedVoucher;
            set
            {
                _selectedVoucher = value;
                OnPropertyChanged();
                Discount = _selectedVoucher?.Discount ?? 0;
                UpdateTotalPrice();
            }
        }

        private double _discount;
        public double Discount
        {
            get => _discount;
            set
            {
                _discount = value;
                OnPropertyChanged();
            }
        }

        public class SelectedBookItem : INotifyPropertyChanged
        {
            public Book Book { get; set; }
            private int _quantity = 1;
            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (_quantity != value)
                    {
                        _quantity = value;
                        OnPropertyChanged(nameof(Quantity));
                        OnPropertyChanged(nameof(Subtotal));
                    }
                }
            }
            public decimal Subtotal => (Book.BookPrice ?? 0) * Quantity;
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddBookCommand { get; set; }
        public ICommand RemoveBookCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand AddOrderCommand { get; set; } // Command to add an order

        private Window _currentWindow;
        public AddOrderVM(Window currentWindow)
        {
            _currentWindow = currentWindow;
            LoadData();
            AddBookCommand = new RelayCommand(_ => AddBook());
            RemoveBookCommand = new RelayCommand(param => RemoveBook((SelectedBookItem)param));
            ClearCommand = new RelayCommand(_ => Clear());
            AddOrderCommand = new RelayCommand(_ => AddOrder()); // Initialize AddOrderCommand
            UpdateFilteredBooks();
        }

        private void LoadData()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                Accounts = new ObservableCollection<Account>(context.Accounts.ToList());
                Books = new ObservableCollection<Book>(context.Books.Where(b => b.BookStatus == 1).ToList());
                Vouchers = new ObservableCollection<Voucher>(context.Vouchers.Where(v => v.VouStatus == 1).ToList());
            }
            UpdateFilteredBooks();
        }

        private void AddBook()
        {
            if (SelectedBook != null && !SelectedBooks.Any(b => b.Book.BookId == SelectedBook.BookId))
            {
                var newBookItem = new SelectedBookItem { Book = SelectedBook, Quantity = 1 };
                newBookItem.PropertyChanged += SelectedBookItem_PropertyChanged;
                SelectedBooks.Add(newBookItem);

                SelectedBook = null;
                UpdateFilteredBooks();
                UpdateTotalPrice();
            }
        }

        private void RemoveBook(SelectedBookItem bookItem)
        {
            if (SelectedBooks.Contains(bookItem))
            {
                bookItem.PropertyChanged -= SelectedBookItem_PropertyChanged;
                SelectedBooks.Remove(bookItem);
                UpdateFilteredBooks();
                UpdateTotalPrice();
            }
        }

        private void SelectedBookItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedBookItem.Subtotal) || e.PropertyName == nameof(SelectedBookItem.Quantity))
            {
                UpdateTotalPrice();
            }
        }

        private void UpdateFilteredBooks()
        {
            FilteredBooks = new ObservableCollection<Book>(Books.Except(SelectedBooks.Select(b => b.Book)));
            OnPropertyChanged(nameof(FilteredBooks));
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = SelectedBooks.Sum(b => b.Subtotal) * (1 - (decimal)Discount / 100);
        }

        private void Clear()
        {
            SelectedBooks.Clear();
            SelectedBook = null;
            SelectedVoucher = null;
            SelectedAccount = null;
            Discount = 0;
            TotalPrice = 0;
            UpdateFilteredBooks();
        }

        public event Action OrderAdded;
        private void AddOrder()
        {
            if (SelectedAccount == null)
            {
                MessageBox.Show("Please select a Username.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedBooks == null || !SelectedBooks.Any())
            {
                MessageBox.Show("Please add at least one book to the order.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new Prn212ProjectCapybookContext())
            {
                var newOrder = new Order
                {
                    VouId = SelectedVoucher?.VouId,
                    Username = SelectedAccount.Username,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    OrderStatus = 0
                };

                context.Orders.Add(newOrder);
                context.SaveChanges();

                int orderId = newOrder.OrderId;

                foreach (var selectedBook in SelectedBooks)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        BookId = selectedBook.Book.BookId,
                        Quantity = selectedBook.Quantity
                    };
                    context.OrderDetails.Add(orderDetail);
                }

                context.SaveChanges();
            }

            // Clear data and close the window after adding order
            Clear();
            OrderAdded?.Invoke(); // Trigger the event
            _currentWindow.Close();
        }

    }
}
