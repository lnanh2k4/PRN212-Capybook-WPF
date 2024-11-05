using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Capybook.Models;
using Capybook.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Capybook.ViewModels
{
    public class CategoryVM : BaseVM
    {
        public ObservableCollection<Category> Categories { get; set; } // Used for ListView
        public ObservableCollection<Category> AllCategories { get; set; } // Used for ComboBox
        private Category _selectedCategory;

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

        public Category TemporaryCategory { get; set; }

        // Commands
        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public CategoryVM()
        {
            Categories = new ObservableCollection<Category>();
            AllCategories = new ObservableCollection<Category>(); // Initialize AllCategories
            TemporaryCategory = new Category();

            // Initialize commands
            AddCommand = new RelayCommand(AddCategory);
            ModifyCommand = new RelayCommand(ModifyCategory);
            DeleteCommand = new RelayCommand(DeleteCategory);
            SearchCommand = new RelayCommand(SearchCategories);

            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            AllCategories.Clear();

            using (var context = new Prn212ProjectCapybookContext())
            {
                var categoriesFromDb = context.Categories
                    .Where(category => category.CatStatus != 0)
                    .ToList();
                foreach (var category in categoriesFromDb)
                {
                    Categories.Add(category); // Used for ListView
                    AllCategories.Add(category); // Used for ComboBox
                }
            }
        }

        private void UpdateTemporaryCategory(Category selectedCategory)
        {
            TemporaryCategory = new Category
            {
                CatId = selectedCategory.CatId,
                CatName = selectedCategory.CatName,
                ParentCatId = selectedCategory.ParentCatId,
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
                        categoryToDelete.CatStatus = 0;
                        context.Entry(categoryToDelete).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadCategories();
                MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SearchCategories(object parameter)
        {
            int cnt = 0;
            using (var context = new Prn212ProjectCapybookContext())
            {
                var query = context.Categories.Where(category => category.CatStatus != 0).AsQueryable();

                if (!string.IsNullOrEmpty(TemporaryCategory.CatName))
                {
                    query = query.Where(c => c.CatName.Contains(TemporaryCategory.CatName));
                    cnt++;
                }

                if (TemporaryCategory.ParentCatId != null)
                {
                    query = query.Where(c => c.ParentCatId == TemporaryCategory.ParentCatId);
                    cnt++;
                }

                if (cnt == 0)
                {
                    Categories.Clear();
                    foreach (var category in AllCategories)
                    {
                        Categories.Add(category);
                    }
                }
                else
                {
                    var results = query.ToList();
                    Categories.Clear();
                    foreach (var category in results)
                    {
                        Categories.Add(category);
                    }
                }
            }
        }


    }
}
