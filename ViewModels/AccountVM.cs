﻿using Capybook.Models;
using Capybook.Utilities;
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
                    context.Accounts.Remove(_selectedItem);
                    context.SaveChanges();
                    Accounts.Remove(_selectedItem);
                    SelectedItem = null;
                    NewItem = new Account();
                }
            }
        }

        private void UPDATE(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (SelectedItem != null) {
                    var item = context.Accounts.Where(x => x.Username == _username).FirstOrDefault();
                    if (item != null)
                    {
                        item.Address = NewItem.Address;
                        item.Password = NewItem.Password;
                        item.AccStatus = NewItem.AccStatus;
                        item.Email = NewItem.Email;
                        item.FirstName = NewItem.FirstName;
                        item.LastName = NewItem.LastName;
                        item.Address = NewItem.Address;
                        item.Dob = NewItem.Dob;
                        item.Phone = NewItem.Phone;
                        item.Role = NewItem.Role;
                        context.SaveChanges();
                        Load();
                    }
                }
            }
        }

        private void ADD(object obj)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                if (NewItem != null)
                {
                    context.Accounts.Add(NewItem);
                    context.SaveChanges();
                    Accounts.Add(NewItem);
                    NewItem = new Account();
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
    }
}
