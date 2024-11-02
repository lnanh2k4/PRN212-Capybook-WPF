using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Capybook.Models;
using Capybook.Utilities; // Assuming you have a RelayCommand or similar utility
using Microsoft.EntityFrameworkCore;

namespace Capybook.ViewModels
{
    public class CategoryVM : BaseVM
    {
        public ObservableCollection<Category> Categories { get; set; }
        private Category _selectedCategory;

        // SelectedCategory is used for the ListView selection
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                if (_selectedCategory != null)
                {
                    UpdateTemporaryCategory(_selectedCategory);
                }
            }
        }

        // TemporaryCategory is used for editing details in the UI
        public Category TemporaryCategory { get; set; }

        // Commands for Add, Edit, and Delete
        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public CategoryVM()
        {
            Categories = new ObservableCollection<Category>();
            TemporaryCategory = new Category();

            // Initialize commands
            AddCommand = new RelayCommand(AddCategory);
            ModifyCommand = new RelayCommand(ModifyCategory);
            DeleteCommand = new RelayCommand(DeleteCategory);

            LoadCategories(); // Load categories from the database
        }

        private void LoadCategories()
        {
            Categories.Clear();
            using (var context = new Prn212ProjectCapybookContext())
            {
                var categoriesFromDb = context.Categories
                    .Where(category => category.CatStatus != 0) // Only include categories with CategoryStatus != 0
                    .ToList();
                foreach (var category in categoriesFromDb)
                {
                    Categories.Add(category);
                }
            }
        }

        private void UpdateTemporaryCategory(Category selectedCategory)
        {
            // Create a copy of SelectedCategory for editing in TemporaryCategory
            TemporaryCategory = new Category
            {
                CatId = selectedCategory.CatId,
                CatName = selectedCategory.CatName,
                ParentCatId= selectedCategory.ParentCatId,
                CatStatus = selectedCategory.CatStatus,
            };
            OnPropertyChanged(nameof(TemporaryCategory));
        }

        private void AddCategory(object parameter)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var newCategory = new Category
                {
                    CatId = TemporaryCategory.CatId,
                    CatName = TemporaryCategory.CatName,
                    ParentCatId = TemporaryCategory.ParentCatId,
                    CatStatus = 1
                };

                context.Categories.Add(newCategory);
                context.SaveChanges();
                LoadCategories();
                MessageBox.Show("Category added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ModifyCategory(object parameter)
        {
            if (SelectedCategory != null)
            {
                using (var context = new Prn212ProjectCapybookContext())
                {
                    var categoryToUpdate = context.Categories.FirstOrDefault(c => c.CatId == SelectedCategory.CatId);
                    if (categoryToUpdate != null)
                    {
                        categoryToUpdate.CatId = TemporaryCategory.CatId;
                        categoryToUpdate.CatName = TemporaryCategory.CatName;
                        categoryToUpdate.ParentCatId = TemporaryCategory.ParentCatId;
                        categoryToUpdate.CatStatus = TemporaryCategory.CatStatus;
                        context.Entry(categoryToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadCategories();
                MessageBox.Show("Category modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteCategory(object parameter)
        {
            if (SelectedCategory != null)
            {
                using (var context = new Prn212ProjectCapybookContext())
                {
                    var categoryToDelete = context.Categories.FirstOrDefault(c => c.CatId == SelectedCategory.CatId);
                    if (categoryToDelete != null)
                    {
                        categoryToDelete.CatStatus = 0; // Mark as inactive
                        context.Entry(categoryToDelete).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadCategories();
                MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
