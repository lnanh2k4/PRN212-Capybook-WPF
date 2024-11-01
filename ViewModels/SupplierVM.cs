using Capybook.Models;
using Capybook.Utilities;
using System.Collections.ObjectModel;
using System.Linq;
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

        public SupplierVM()
        {
            Suppliers = new ObservableCollection<Supplier>();
            Load();
            NewItem = new Supplier();
            AddCommand = new RelayCommand(ADD);
             UpdateCommand = new RelayCommand(UPDATE);
            DeleteCommand = new RelayCommand(DELETE);
            ClearCommand = new RelayCommand(CLEAR);
        }

        private void CLEAR(object obj)
        {
            NewItem = new Supplier();
            System.Windows.MessageBox.Show("Form cleared!", "Clear Form", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        private void DELETE(object obj)
        {
            
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (_selectedItem != null)
                {
                    context.Suppliers.Remove(_selectedItem);
                    context.SaveChanges();
                    Suppliers.Remove(_selectedItem);
                    SelectedItem = null;
                    NewItem = new Supplier();

                    // Hiển thị thông báo xóa thành công
                    System.Windows.MessageBox.Show("Supplier deleted successfully!", "Delete Supplier", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }




        private void ADD(object obj)
        {
            ClearErrorMessages();
            if (!IsValidSupplier(NewItem)) return;
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (NewItem != null)
                {
                    context.Suppliers.Add(NewItem);
                    context.SaveChanges();
                    Suppliers.Add(NewItem);
                    NewItem = new Supplier();

                    // Hiển thị thông báo thêm thành công
                    System.Windows.MessageBox.Show("Supplier added successfully!", "Add Supplier", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }

        private void UPDATE(object obj)
        {
            ClearErrorMessages();
            if (!IsValidSupplier(NewItem)) return;
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (SelectedItem != null)
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

                        // Hiển thị thông báo cập nhật thành công
                        System.Windows.MessageBox.Show("Supplier updated successfully!", "Update Supplier", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                }
            }
        }
        private void Load()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var items = context.Suppliers.ToList();
                Suppliers.Clear();
                foreach (var item in items)
                {
                    if (item.SupStatus != 0)
                    {
                        Suppliers.Add(item);
                    }
                }
            }
        }

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
        // Các thuộc tính lưu thông báo lỗi cho từng trường
        public string SupNameError { get; set; }
        public string SupEmailError { get; set; }
        public string SupPhoneError { get; set; }
        public string SupAddressError { get; set; }

        private bool IsValidSupplier(Supplier supplier)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(supplier.SupName))
            {
                SupNameError = "This field cannot be left empty.";
                isValid = false;
            }

            
            if (string.IsNullOrWhiteSpace(supplier.SupEmail) ||  !supplier.SupEmail.Contains("@") || !supplier.SupEmail.Contains(".")  )
            {
                SupEmailError = "Please enter a valid email format.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(supplier.SupPhone) || supplier.SupPhone.Length < 10)
            {
                SupPhoneError = "Please enter a phone number with at least 10 digits.";
                isValid = false;
            }
           

            if (string.IsNullOrWhiteSpace(supplier.SupAddress))
            {
                SupAddressError = "Address cannot be empty. ";
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
    }
}
    


