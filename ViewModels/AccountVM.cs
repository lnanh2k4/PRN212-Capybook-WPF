﻿using Capybook.Models;
using Capybook.Utilities;
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
    public class AccountVM : BaseVM
    {
        public ObservableCollection<Account> Accounts { get; set; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ResetCommand { get; }
        private string _username;
        public AccountVM()
        {
            Accounts = new ObservableCollection<Account>();
            Load();
            NewItem = new Account();
            AddCommand = new RelayCommand(ADD);
            UpdateCommand = new RelayCommand(UPDATE);
            DeleteCommand = new RelayCommand(DELETE);
            ClearCommand = new RelayCommand(CLEAR);
            RegisterCommand = new RelayCommand(REGISTER);
            ResetCommand = new RelayCommand(CLEAR);
        }



        private void REGISTER(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (NewItem != null && isValidate() && isValidateUsername())
                {
                    NewItem.AccStatus = 1;
                    NewItem.Role = 1;
                    context.Accounts.Add(NewItem);
                    context.SaveChanges();
                    Accounts.Add(NewItem);
                    NewItem = new Account();
                    MessageBox.Show("Register successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    if(obj is Window currentWindow)
                    {
                        currentWindow.Close();
                    }
                }
            }
        }

        private void CLEAR(object obj)
        {
            NewItem = new Account();
        }

        private void DELETE(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (_selectedItem != null)
                {
                    var item = context.Accounts.Where(x => x.Username == _selectedItem.Username).FirstOrDefault();
                    item.AccStatus = 1;
                    context.SaveChanges();
                    Accounts.Remove(_selectedItem);
                    SelectedItem = null;
                    NewItem = new Account();
                    MessageBox.Show("Account is deleted successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Data is not found! Please select any data row before doing this function!", "No Data", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void UPDATE(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (SelectedItem != null) {
                    var item = context.Accounts.Where(x => x.Username == _username).FirstOrDefault();
                    if (item != null && isValidate())
                    {
                        item.Address = NewItem.Address;
                        item.Password = NewItem.Password;
                        item.Email = NewItem.Email;
                        item.FirstName = NewItem.FirstName;
                        item.LastName = NewItem.LastName;
                        item.Address = NewItem.Address;
                        item.Dob = NewItem.Dob;
                        item.Phone = NewItem.Phone;
                        item.Role = NewItem.Role;
                        item.Sex = NewItem.Sex;
                        context.SaveChanges();
                        Load();
                        NewItem = new Account();
                        MessageBox.Show("Account is updated successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Data is not found! Please select any data row before doing this function!", "No Data", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private void ADD(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (NewItem != null && isValidate() && isValidateUsername())
                {
                    NewItem.AccStatus = 1;
                    context.Accounts.Add(NewItem);
                    context.SaveChanges();
                    Accounts.Add(NewItem);
                    NewItem = new Account();
                    MessageBox.Show("Account is added successfully!", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Load()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var items = context.Accounts.ToList();
                Accounts.Clear();
                foreach (var item in items)
                {
                    if (item.AccStatus != 0)
                    {
                        Accounts.Add(item);
                    }
                }
            }
        }

        private Account _selectedItem;
        public Account SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (_selectedItem != null)
                {
                    _username = _selectedItem.Username;
                    NewItem = new Account
                    {
                        AccStatus = _selectedItem.AccStatus,
                        Address = _selectedItem.Address,
                        Dob = _selectedItem.Dob,
                        Email = _selectedItem.Email,
                        FirstName = _selectedItem.FirstName,
                        LastName = _selectedItem.LastName,
                        Orders = _selectedItem.Orders,
                        Password = _selectedItem.Password,
                        Phone = _selectedItem.Phone,
                        Role = _selectedItem.Role,
                        Sex = _selectedItem.Sex,
                        Username = _selectedItem.Username,
                    };
                    OnPropertyChanged(nameof(NewItem));
                }
            }
        }

        private Account _newItem;
        public Account NewItem
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

        public bool isValidateUsername()
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var item = context.Accounts.Where(x => x.Username == NewItem.Username).FirstOrDefault();
                if (item != null) {
                    MessageBox.Show("Username is existed! Please change other username!","Validate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }
        public bool isValidate()
        {
            if (string.IsNullOrWhiteSpace(NewItem.Username))
            {
                MessageBox.Show("Username must be not empty! Please enter username!", "Validate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(NewItem.Password))
            {
                MessageBox.Show("Password must be not empty! Please enter password!", "Validate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(NewItem.Email))
            {
                MessageBox.Show("Email must be not empty! Please enter email!", "Validate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(NewItem.Address))
            {
                MessageBox.Show("Address must be not empty! Please enter address!", "Validate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (NewItem.Dob > DateOnly.FromDateTime(DateTime.Now))
            {
                MessageBox.Show("Date of birth must not be greater than moment time! Please select orther date!", "Validate Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
    }
}
