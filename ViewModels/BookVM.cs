using Capybook.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Capybook.ViewModels
{
    public class BookVM : BaseVM
    {
        private Book _selectedBook;
        private readonly Prn212ProjectCapybookContext _dbContext;

        public ObservableCollection<Book> Books { get; set; }

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if (_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged();  // Notify the UI when SelectedBook changes

                    if (_selectedBook != null)
                    {
                        BookId = _selectedBook.BookId;
                        BookTitle = _selectedBook.BookTitle;
                        Author = _selectedBook.Author;
                        Translator = _selectedBook.Translator;
                        Publisher = _selectedBook.Publisher;
                        Isbn = _selectedBook.Isbn;
                        BookDescription = _selectedBook.BookDescription;
                        Dimension = _selectedBook.Dimension;
                        Hardcover = _selectedBook.Hardcover;
                        Weight = _selectedBook.Weight;
                        BookStatus = _selectedBook.BookStatus;
                    }
                }
            }
        }

        private int _bookId;
        public int BookId
        {
            get { return _bookId; }
            set
            {
                _bookId = value;
                OnPropertyChanged(nameof(BookId));
            }
        }

        private string _bookTitle;
        public string BookTitle
        {
            get { return _bookTitle; }
            set
            {
                _bookTitle = value;
                OnPropertyChanged(nameof(BookTitle));
            }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        private string _translator;
        public string Translator
        {
            get { return _translator; }
            set
            {
                _translator = value;
                OnPropertyChanged(nameof(Translator));
            }
        }

        private string _publisher;
        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }

        private string _isbn;
        public string Isbn
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(Isbn));
            }
        }

        private string _bookDescription;
        public string BookDescription
        {
            get { return _bookDescription; }
            set
            {
                _bookDescription = value;
                OnPropertyChanged(nameof(BookDescription));
            }
        }

        private string _dimension;
        public string Dimension
        {
            get { return _dimension; }
            set
            {
                _dimension = value;
                OnPropertyChanged(nameof(Dimension));
            }
        }

        private int? _hardcover;
        public int? Hardcover
        {
            get { return _hardcover; }
            set
            {
                _hardcover = value;
                OnPropertyChanged(nameof(Hardcover));
            }
        }

        private double? _weight;
        public double? Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        private int? _bookStatus;
        public int? BookStatus
        {
            get { return _bookStatus; }
            set
            {
                _bookStatus = value;
                OnPropertyChanged(nameof(BookStatus));
            }
        }

        // Commands for CRUD actions
        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        // Constructor
        public BookVM()
        {
            _dbContext = new Prn212ProjectCapybookContext();  // Initialize DbContext
            LoadBooks();  // Load data from the database

            AddCommand = new RelayCommand(AddBook);
            ModifyCommand = new RelayCommand(ModifyBook, CanModifyOrDelete);
            DeleteCommand = new RelayCommand(DeleteBook, CanModifyOrDelete);

            SelectedBook = new Book();  // Initialize new book
        }

        // Load all books from the database
        private void LoadBooks()
        {
            Books = new ObservableCollection<Book>(_dbContext.Books.ToList());  // Load list of books from the database
            OnPropertyChanged(nameof(Books));  // Notify UI that Books have changed
        }

        // Add a new book
        private void AddBook(object parameter)
        {
            try
            {
                if (SelectedBook != null && !string.IsNullOrWhiteSpace(SelectedBook.BookTitle))
                {
                    SelectedBook.BookStatus = 1;  // Ensure BookStatus is set to 1 (active)

                    _dbContext.Books.Add(SelectedBook);  // Add new book to the database
                    _dbContext.SaveChanges();  // Save changes to the database

                    LoadBooks();  // Reload the list of books
                    MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    SelectedBook = new Book();  // Reset the selected book for the next input
                    OnPropertyChanged(nameof(SelectedBook));
                }
                else
                {
                    MessageBox.Show("Please enter valid book information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Modify an existing book
        private void ModifyBook(object parameter)
        {
            try
            {
                if (SelectedBook != null && SelectedBook.BookId > 0)
                {
                    _dbContext.Books.Update(SelectedBook);  // Update the selected book
                    _dbContext.SaveChanges();  // Save changes to the database

                    LoadBooks();  // Reload the list of books
                    MessageBox.Show("Book modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    SelectedBook = new Book();  // Reset the selected book after modifying
                    OnPropertyChanged(nameof(SelectedBook));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error modifying book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete a book
        private void DeleteBook(object parameter)
        {
            try
            {
                if (SelectedBook != null && SelectedBook.BookId > 0)
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the book '{SelectedBook.BookTitle}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _dbContext.Books.Remove(SelectedBook);  // Remove the selected book
                        _dbContext.SaveChanges();  // Save changes to the database

                        LoadBooks();  // Reload the list of books
                        MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        SelectedBook = new Book();  // Reset the selected book after deleting
                        OnPropertyChanged(nameof(SelectedBook));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Can execute modify and delete commands only when a book is selected
        private bool CanModifyOrDelete(object parameter)
        {
            return SelectedBook != null && SelectedBook.BookId > 0;
        }

        // Method to save any pending changes (e.g., image updates)
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple implementation of RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
