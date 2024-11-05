using Capybook.Models;
using Capybook.Utilities;
using Microsoft.EntityFrameworkCore;
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
        private Voucher _selectedVoucher;

        public Voucher SelectedVoucher
        {
            get => _selectedVoucher;
            set
            {
                _selectedVoucher = value;
                OnPropertyChanged();
                if (_selectedVoucher != null)
                {
                    UpdateTemporaryVoucher(_selectedVoucher);
                }
            }
        }
        public Voucher TemporaryVoucher { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public VoucherVM()
        {
            Vouchers = new ObservableCollection<Voucher>();
            TemporaryVoucher = new Voucher();

            AddCommand = new RelayCommand(AddVoucher);
            ModifyCommand = new RelayCommand(ModifyVoucher);
            DeleteCommand = new RelayCommand(DeleteVoucher);
            SearchCommand = new RelayCommand(SearchVouchers);

            LoadVouchers();
        }

        private void LoadVouchers()
        {
            Vouchers.Clear();

            using (var context = new Prn212ProjectCapybookContext())
            {
                var vouchersFromDb = context.Vouchers
                    .Where(voucher => voucher.VouStatus != 0)
                    .ToList();
                foreach (var voucher in vouchersFromDb)
                {
                    Vouchers.Add(voucher);
                }
            }
        }
        private void UpdateTemporaryVoucher(Voucher selectedVoucher)
        {
            TemporaryVoucher = new Voucher
            {
                VouId = selectedVoucher.VouId,
                VouName = selectedVoucher.VouName,
                VouCode = selectedVoucher.VouCode,
                Discount = selectedVoucher.Discount,
                StartDate = selectedVoucher.StartDate,
                EndDate = selectedVoucher.EndDate,
                Quantity = selectedVoucher.Quantity,
                VouStatus = selectedVoucher.VouStatus
            };
            OnPropertyChanged(nameof(TemporaryVoucher));
        }
        private void AddVoucher(object parameter)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var newVoucher = new Voucher
                {
                    VouName = TemporaryVoucher.VouName,
                    VouCode = TemporaryVoucher.VouCode,
                    Discount = TemporaryVoucher.Discount,
                    StartDate = TemporaryVoucher.StartDate,
                    EndDate = TemporaryVoucher.EndDate,
                    Quantity = TemporaryVoucher.Quantity,
                    VouStatus = 1
                };

                context.Vouchers.Add(newVoucher);
                context.SaveChanges();
                LoadVouchers();
                MessageBox.Show("Voucher added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ModifyVoucher(object parameter)
        {
            if (SelectedVoucher != null)
            {
                using (var context = new Prn212ProjectCapybookContext())
                {
                    var voucherToUpdate = context.Vouchers.FirstOrDefault(v => v.VouId == SelectedVoucher.VouId);
                    if (voucherToUpdate != null)
                    {
                        voucherToUpdate.VouName = TemporaryVoucher.VouName;
                        voucherToUpdate.VouCode = TemporaryVoucher.VouCode;
                        voucherToUpdate.Discount = TemporaryVoucher.Discount;
                        voucherToUpdate.StartDate = TemporaryVoucher.StartDate;
                        voucherToUpdate.EndDate = TemporaryVoucher.EndDate;
                        voucherToUpdate.Quantity = TemporaryVoucher.Quantity;
                        voucherToUpdate.VouStatus = TemporaryVoucher.VouStatus;
                        context.Entry(voucherToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadVouchers();
                MessageBox.Show("Voucher modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteVoucher(object parameter)
        {
            if (SelectedVoucher != null)
            {
                using (var context = new Prn212ProjectCapybookContext())
                {
                    var voucherToDelete = context.Vouchers.FirstOrDefault(v => v.VouId == SelectedVoucher.VouId);
                    if (voucherToDelete != null)
                    {
                        voucherToDelete.VouStatus = 0;
                        context.Entry(voucherToDelete).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadVouchers();
                MessageBox.Show("Voucher deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SearchVouchers(object parameter)
        {
            int cnt = 0;
            using (var context = new Prn212ProjectCapybookContext())
            {
                var query = context.Vouchers.Where(voucher => voucher.VouStatus != 0).AsQueryable();

                if (!string.IsNullOrEmpty(TemporaryVoucher.VouName))
                {
                    query = query.Where(v => v.VouName.Contains(TemporaryVoucher.VouName));
                    cnt++;
                }

                if (!string.IsNullOrEmpty(TemporaryVoucher.VouCode))
                {
                    query = query.Where(v => v.VouCode.Contains(TemporaryVoucher.VouCode));
                    cnt++;
                }

                if (cnt == 0)
                {
                    LoadVouchers(); 
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
    }
}


