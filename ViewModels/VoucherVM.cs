using Capybook.Models;
using Capybook.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Capybook.ViewModels
{
    public class VoucherVM : BaseVM
    {
        public ObservableCollection<Voucher> Vouchers { get; set; } 
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }

        private Voucher _newItem;
        public Voucher NewItem
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

        private Voucher _selectedItem;
        public Voucher SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    NewItem = new Voucher
                    {
                      VouId = _selectedItem.VouId,
                      VouName = _selectedItem.VouName,
                      EndDate = _selectedItem.EndDate,
                      StartDate = _selectedItem.StartDate,
                      Discount = _selectedItem.Discount,
                      Orders = _selectedItem.Orders,
                      Quantity = _selectedItem.Quantity,
                      VouCode = _selectedItem.VouCode,
                      VouStatus = _selectedItem.VouStatus,
                    };
                    OnPropertyChanged(nameof(NewItem));
                }
            }
        }

        public VoucherVM()
        {
            Vouchers = new ObservableCollection<Voucher>();
            Load();
            NewItem = new Voucher();
            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update);
            DeleteCommand = new RelayCommand(Delete);
            SearchCommand = new RelayCommand(Search);
            ClearCommand = new RelayCommand(Clear);
        }

       

        private void Load()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var items = context.Vouchers.ToList();
                Vouchers.Clear();
                foreach (var item in items)
                {
                    if (item.VouStatus != 0)
                    {
                        Vouchers.Add(item);
                    }
                }
            }
        }

        private void Add(object parameter)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (NewItem != null && ValidateVoucher(NewItem))
                {
                    NewItem.VouStatus = 1;
                    context.Vouchers.Add(NewItem);
                    context.SaveChanges();
                    Vouchers.Add(NewItem);
                    NewItem = new Voucher();
                    MessageBox.Show("Voucher is added successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Update(object parameter)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (SelectedItem != null && ValidateVoucher(NewItem))
                {
                    var item = context.Vouchers.FirstOrDefault(v => v.VouId == SelectedItem.VouId);
                    if (item != null)
                    {
                        item.VouName = NewItem.VouName;
                        item.VouCode = NewItem.VouCode;
                        item.Discount = NewItem.Discount;
                        item.StartDate = NewItem.StartDate;
                        item.EndDate = NewItem.EndDate;
                        item.Quantity = NewItem.Quantity;
                        item.VouStatus = NewItem.VouStatus;
                        context.SaveChanges();
                        Load();
                        NewItem = new Voucher();
                        MessageBox.Show("Voucher is updated successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select any data row before doing this function!", "No Data", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void Delete(object parameter)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (_selectedItem != null && ValidateVoucher(NewItem))
                {
                    var item = context.Vouchers.FirstOrDefault(v => v.VouId == SelectedItem.VouId);
                    item.VouStatus = 0;
                    context.SaveChanges();
                    Vouchers.Remove(_selectedItem);
                    SelectedItem = null;
                    NewItem = new Voucher();
                    MessageBox.Show("Voucher is deleted successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Data is not found! Please select any data row before doing this function!", "No Data", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void Search(object parameter)
        {
            int cnt = 0;
            using (var context = new Prn212ProjectCapybookContext())
            {
                var query = context.Vouchers.Where(voucher => voucher.VouStatus != 0).AsQueryable();

                if (!string.IsNullOrEmpty(NewItem.VouName))
                {
                    query = query.Where(v => v.VouName.Contains(NewItem.VouName));
                    cnt++;
                }

                if (!string.IsNullOrEmpty(NewItem.VouCode))
                {
                    query = query.Where(v => v.VouCode.Contains(NewItem.VouCode));
                    cnt++;
                }

                if (cnt == 0)
                {
                    Load(); 
                }
                else
                {
                    var results = query.ToList();
                    Vouchers.Clear();
                    foreach (var voucher in results)
                    {
                        Vouchers.Add(voucher);
                    }
                }
            }
        }
        private void Clear(object parameter)
        {
            NewItem = new Voucher();
        }

        private bool ValidateVoucher(Voucher voucher)
        {
            if (string.IsNullOrWhiteSpace(voucher.VouName))
            {
                MessageBox.Show("Voucher Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(voucher.VouCode))
            {
                MessageBox.Show("Voucher Code cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (voucher.Discount < 0)
            {
                MessageBox.Show("Discount cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (voucher.StartDate > voucher.EndDate)
            {
                MessageBox.Show("Start Date cannot be after End Date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (voucher.Quantity < 0)
            {
                MessageBox.Show("Quantity cannot be negative.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}


