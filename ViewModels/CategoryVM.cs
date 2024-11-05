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
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Category> AllCategories { get; set; }
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

        // Error properties for validation
        public string CatNameError { get; set; }
        public string ParentCatIdError { get; set; }

        public CategoryVM()
        {
            Categories = new ObservableCollection<Category>();
            AllCategories = new ObservableCollection<Category>();
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
                    Categories.Add(category);
                    AllCategories.Add(category);
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
            ClearErrorMessages();

            // Perform validation on the TemporaryCategory object
            if (!IsValidCategory(TemporaryCategory)) return;

            using (var context = new Prn212ProjectCapybookContext())
            {
                // Check if a category with the same CatName already exists, ignoring the ParentCatId
                bool nameExists = context.Categories.Any(c =>
                    c.CatName == TemporaryCategory.CatName &&
                    c.CatStatus != 0);

                if (nameExists)
                {
                    MessageBox.Show("A category with the same name already exists.", "Duplicate Category", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newCategory = new Category
                {
                    CatName = TemporaryCategory.CatName,
                    ParentCatId = TemporaryCategory.ParentCatId,
                    CatStatus = 1 // Active status
                };

                context.Categories.Add(newCategory);
                context.SaveChanges();
                LoadCategories();
                MessageBox.Show("Category added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ModifyCategory(object parameter)
        {
            ClearErrorMessages();

            // Check if a category is selected
            if (SelectedCategory == null)
            {
                MessageBox.Show("Please select a category to modify.", "Modify Category", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Perform validation on the TemporaryCategory object
            if (!IsValidCategory(TemporaryCategory))
            {
                // If there's a validation error, show a message and return
                if (!string.IsNullOrEmpty(CatNameError))
                {
                    MessageBox.Show(CatNameError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return;
            }

            using (var context = new Prn212ProjectCapybookContext())
            {
                // Check for duplicate category name and parent category combination
                bool categoryExists = context.Categories.Any(c =>
                    c.CatName == TemporaryCategory.CatName &&
                    c.ParentCatId == TemporaryCategory.ParentCatId &&
                    c.CatId != TemporaryCategory.CatId && // Exclude the current category from the check
                    c.CatStatus != 0);

                if (categoryExists)
                {
                    MessageBox.Show("A category with the same name and parent category already exists.", "Duplicate Category", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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

        private bool IsValidCategory(Category category)
        {
            bool isValid = true;
            CatNameError = string.Empty;

            // Check if Category Name is empty
            if (string.IsNullOrWhiteSpace(category.CatName))
            {
                CatNameError = "Category name cannot be empty.";
                isValid = false;
            }
            else
            {
                // Check if the Category Name already exists in the database
                using (var context = new Prn212ProjectCapybookContext())
                {
                    bool nameExists = context.Categories.Any(c =>
                        c.CatName == category.CatName && c.CatId != category.CatId && c.CatStatus != 0);
                    if (nameExists)
                    {
                        CatNameError = "Category name already exists.";
                        isValid = false;
                    }
                }
            }

            // Check if the category is trying to set itself as its parent
            if (category.ParentCatId == category.CatId)
            {
                CatNameError = "A category cannot be its own parent.";
                isValid = false;
            }
            else
            {
                // Check for circular parent-child relationship
                using (var context = new Prn212ProjectCapybookContext())
                {
                    // Check for direct circular reference
                    var parentCategory = context.Categories.FirstOrDefault(c => c.CatId == category.ParentCatId);
                    if (parentCategory != null && parentCategory.ParentCatId == category.CatId)
                    {
                        CatNameError = "Circular parent-child relationship detected.";
                        isValid = false;
                    }

                    // Extended check for indirect circular references
                    var currentCategory = parentCategory;
                    while (currentCategory != null)
                    {
                        if (currentCategory.ParentCatId == category.CatId)
                        {
                            CatNameError = "Extended circular reference detected in the category hierarchy.";
                            isValid = false;
                            break;
                        }
                        currentCategory = context.Categories.FirstOrDefault(c => c.CatId == currentCategory.ParentCatId);
                    }
                }
            }

            OnPropertyChanged(nameof(CatNameError));
            return isValid;
        }






        private void ClearErrorMessages()
        {
            CatNameError = ParentCatIdError = string.Empty;
            OnPropertyChanged(nameof(CatNameError));
            OnPropertyChanged(nameof(ParentCatIdError));
        }
    }
}
