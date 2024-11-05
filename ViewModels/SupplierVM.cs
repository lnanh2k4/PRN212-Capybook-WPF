using Capybook.Models;
using Capybook.Utilities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Capybook.ViewModels
{
    public class SupplierVM : BaseVM
    {
        public ObservableCollection<Supplier> Suppliers { get; set; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SearchCommand { get; }

        private Supplier _selectedItem;
        public Supplier SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    NewItem = new Supplier
                    {
                        SupId = _selectedItem.SupId,
                        SupName = _selectedItem.SupName,
                        SupEmail = _selectedItem.SupEmail,
                        SupPhone = _selectedItem.SupPhone,
                        SupAddress = _selectedItem.SupAddress,
                        SupStatus = _selectedItem.SupStatus
                    };
                    OnPropertyChanged(nameof(NewItem));
                }
            }
        }

        private Supplier _newItem;
        public Supplier NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                OnPropertyChanged(nameof(NewItem));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public SupplierVM()
        {
            Suppliers = new ObservableCollection<Supplier>();
            Load();
            NewItem = new Supplier();
            AddCommand = new RelayCommand(ADD);
            UpdateCommand = new RelayCommand(UPDATE);
            DeleteCommand = new RelayCommand(DELETE);
            ClearCommand = new RelayCommand(CLEAR);
            SearchCommand = new RelayCommand(SEARCH);
        }

        private void CLEAR(object obj)
        {
            NewItem = new Supplier();
            ClearErrorMessages();
            MessageBox.Show("Form cleared!", "Clear Form", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DELETE(object obj)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a supplier to delete.", "Delete Supplier", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new Prn212ProjectCapybookContext())
            {
                context.Suppliers.Remove(SelectedItem);
                context.SaveChanges();
                Suppliers.Remove(SelectedItem);
                SelectedItem = null;
                NewItem = new Supplier();
                ClearErrorMessages();
                MessageBox.Show("Supplier deleted successfully!", "Delete Supplier", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ADD(object obj)
        {
            ClearErrorMessages();
            if (!IsValidSupplier(NewItem)) return;

            using (var context = new Prn212ProjectCapybookContext())
            {
                context.Suppliers.Add(NewItem);
                context.SaveChanges();
                Suppliers.Add(NewItem);
                NewItem = new Supplier();
                MessageBox.Show("Supplier added successfully!", "Add Supplier", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UPDATE(object obj)
        {
            ClearErrorMessages();
            if (SelectedItem == null)
            {
                MessageBox.Show("Please select a supplier to update.", "Update Supplier", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsValidSupplier(NewItem)) return;

            using (var context = new Prn212ProjectCapybookContext())
            {
                var supplierToUpdate = context.Suppliers.FirstOrDefault(s => s.SupId == SelectedItem.SupId);
                if (supplierToUpdate != null)
                {
                    supplierToUpdate.SupName = NewItem.SupName;
                    supplierToUpdate.SupEmail = NewItem.SupEmail;
                    supplierToUpdate.SupPhone = NewItem.SupPhone;
                    supplierToUpdate.SupAddress = NewItem.SupAddress;
                    supplierToUpdate.SupStatus = NewItem.SupStatus;
                    context.SaveChanges();

                    int index = Suppliers.IndexOf(SelectedItem);
                    Suppliers[index] = NewItem;
                    NewItem = new Supplier();
                    SelectedItem = null;
                    MessageBox.Show("Supplier updated successfully!", "Update Supplier", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Load()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var items = context.Suppliers.Where(s => s.SupStatus != 0).ToList();
                Suppliers.Clear();
                foreach (var item in items)
                {
                    Suppliers.Add(item);
                }
            }
        }

        private void SEARCH(object obj)
        {
            int cnt = 0;
            using (var context = new Prn212ProjectCapybookContext())
            {
                var query = context.Suppliers
                    .Where(s => s.SupStatus != 0) // Only active suppliers
                    .AsQueryable();

                // Apply filters based on filled-in fields
                if (!string.IsNullOrEmpty(NewItem.SupName))
                {
                    query = query.Where(s => s.SupName.Contains(NewItem.SupName));
                    cnt++;
                }
                if (!string.IsNullOrEmpty(NewItem.SupEmail))
                {
                    query = query.Where(s => s.SupEmail.Contains(NewItem.SupEmail));
                    cnt++;
                }
                if (!string.IsNullOrEmpty(NewItem.SupPhone))
                {
                    query = query.Where(s => s.SupPhone.Contains(NewItem.SupPhone));
                    cnt++;
                }
                if (!string.IsNullOrEmpty(NewItem.SupAddress))
                {
                    query = query.Where(s => s.SupAddress.Contains(NewItem.SupAddress));
                    cnt++;
                }

                // If no filters were applied, load all active suppliers
                if (cnt == 0)
                {
                    query = context.Suppliers.Where(s => s.SupStatus != 0);
                }

                var filteredSuppliers = query.ToList();
                Suppliers.Clear();
                foreach (var supplier in filteredSuppliers)
                {
                    Suppliers.Add(supplier);
                }
            }
        }


        private bool IsValidSupplier(Supplier supplier)
        {
            bool isValid = true;

            SupNameError = SupEmailError = SupPhoneError = SupAddressError = string.Empty;

            if (string.IsNullOrWhiteSpace(supplier.SupName))
            {
                SupNameError = "Supplier name cannot be empty.";
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(supplier.SupEmail) || !supplier.SupEmail.Contains("@") || !supplier.SupEmail.Contains("."))
            {
                SupEmailError = "Please enter a valid email.";
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(supplier.SupPhone) || supplier.SupPhone.Length < 10)
            {
                SupPhoneError = "Please enter a valid phone number. ";
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(supplier.SupAddress))
            {
                SupAddressError = "Address cannot be empty.";
                isValid = false;
            }

            OnPropertyChanged(nameof(SupNameError));
            OnPropertyChanged(nameof(SupEmailError));
            OnPropertyChanged(nameof(SupPhoneError));
            OnPropertyChanged(nameof(SupAddressError));

            return isValid;
        }

        private void ClearErrorMessages()
        {
            SupNameError = SupEmailError = SupPhoneError = SupAddressError = string.Empty;
            OnPropertyChanged(nameof(SupNameError));
            OnPropertyChanged(nameof(SupEmailError));
            OnPropertyChanged(nameof(SupPhoneError));
            OnPropertyChanged(nameof(SupAddressError));
        }

        // Error properties for UI binding
        public string SupNameError { get; set; }
        public string SupEmailError { get; set; }
        public string SupPhoneError { get; set; }
        public string SupAddressError { get; set; }
    }
}
