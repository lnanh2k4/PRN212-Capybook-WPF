﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Capybook.Models;
using Capybook.Utilities;
using Microsoft.EntityFrameworkCore; // Assuming you have a RelayCommand or similar utility

namespace Capybook.ViewModels
{
    public class BookVM : BaseVM
    {
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Category> Categories { get; set; } // Collection for categories
        private ObservableCollection<Book> _allBooks;
        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
                if (_selectedBook != null)
                {
                    UpdateTemporaryBook(_selectedBook);
                }
            }
        }

        public Book TemporaryBook { get; set; }

        // Commands for Add, Edit, and Delete
        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public string BookTitleError { get; set; }
        public string AuthorError { get; set; }
        public string IsbnError { get; set; }
        public string PublisherError { get; set; }
        public string PriceError { get; set; }
        public string QuantityError { get; set; }
        public string CategoryError { get; set; }
        public BookVM()
        {
            Books = new ObservableCollection<Book>();
            Categories = new ObservableCollection<Category>(); // Initialize categories
            TemporaryBook = new Book();
            _allBooks = new ObservableCollection<Book>();
            // Initialize commands
            AddCommand = new RelayCommand(AddBook);
            ModifyCommand = new RelayCommand(ModifyBook);
            DeleteCommand = new RelayCommand(DeleteBook);
            SearchCommand = new RelayCommand(SearchBooks);
            LoadBooks();
            LoadCategories(); // Load categories from the database
        }

        private void LoadBooks()
        {
            Books.Clear();
            using (var context = new Prn212ProjectCapybookContext())
            {
                var booksFromDb = context.Books
                    .Include(b => b.Cat) // Include category details
                    .Where(book => book.BookStatus != 0)
                    .ToList();
                foreach (var book in booksFromDb)
                {
                    Books.Add(book);
                }
            }
        }

        private void LoadCategories()
        {
            Categories.Clear();
            using (var context = new Prn212ProjectCapybookContext())
            {
                var categoriesFromDb = context.Categories
                    .Where(cat => cat.CatStatus != 0) // Only active categories
                    .ToList();
                foreach (var category in categoriesFromDb)
                {
                    Categories.Add(category);
                }
            }
        }

        private void UpdateTemporaryBook(Book selectedBook)
        {
            // Create a copy of SelectedBook for editing in TemporaryBook
            TemporaryBook = new Book
            {
                BookId = selectedBook.BookId,
                BookTitle = selectedBook.BookTitle,
                Translator = selectedBook.Translator,
                Author = selectedBook.Author,
                Publisher = selectedBook.Publisher,
                PublicationYear = selectedBook.PublicationYear,
                Isbn = selectedBook.Isbn,
                Dimension = selectedBook.Dimension,
                Hardcover = selectedBook.Hardcover,
                Weight = selectedBook.Weight,
                BookStatus = selectedBook.BookStatus,
                BookDescription = selectedBook.BookDescription,
                Image = selectedBook.Image,
                BookPrice = selectedBook.BookPrice,
                BookQuantity = selectedBook.BookQuantity,
                Cat = Categories.FirstOrDefault(cat => cat.CatId == selectedBook.CatId)
            };
            OnPropertyChanged(nameof(TemporaryBook));
        }

        private void AddBook(object parameter)
        {
            ClearErrorMessages();

            // Check if the book is valid (excluding the unique ISBN check)
            if (!IsValidBook(TemporaryBook)) return;

            // Check if the ISBN already exists in the database
            using (var context = new Prn212ProjectCapybookContext())
            {
                bool isbnExists = context.Books.Any(b => b.Isbn == TemporaryBook.Isbn && b.BookStatus != 0);
                if (isbnExists)
                {
                    MessageBox.Show("ISBN already exists in the database.", "Duplicate ISBN", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newBook = new Book
                {
                    BookTitle = TemporaryBook.BookTitle,
                    Translator = TemporaryBook.Translator,
                    Author = TemporaryBook.Author,
                    Publisher = TemporaryBook.Publisher,
                    PublicationYear = TemporaryBook.PublicationYear,
                    Isbn = TemporaryBook.Isbn,
                    Dimension = TemporaryBook.Dimension,
                    Hardcover = TemporaryBook.Hardcover,
                    Weight = TemporaryBook.Weight,
                    BookStatus = 1, // Active status
                    BookDescription = TemporaryBook.BookDescription,
                    Image = TemporaryBook.Image,
                    BookPrice = TemporaryBook.BookPrice,
                    BookQuantity = TemporaryBook.BookQuantity,
                    CatId = TemporaryBook.Cat?.CatId // Save the category ID
                };

                context.Books.Add(newBook);
                context.SaveChanges();
                LoadBooks();
                MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool IsValidBook(Book book)
        {
            bool isValid = true;
            BookTitleError = AuthorError = IsbnError = PublisherError = PriceError = QuantityError = CategoryError = string.Empty;

            if (string.IsNullOrWhiteSpace(book.BookTitle))
            {
                BookTitleError = "Book title cannot be empty.";
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(book.Author))
            {
                AuthorError = "Author cannot be empty.";
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(book.Isbn))
            {
                IsbnError = "ISBN cannot be empty.";
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(book.Publisher))
            {
                PublisherError = "Publisher cannot be empty.";
                isValid = false;
            }
            if (book.BookPrice == null || book.BookPrice <= 0)
            {
                PriceError = string.IsNullOrWhiteSpace(book.BookPrice.ToString())
                             ? "Price is required."
                             : "Price must be a positive number.";
                isValid = false;
            }
            if (book.BookQuantity == null || book.BookQuantity < 0)
            {
                QuantityError = string.IsNullOrWhiteSpace(book.BookQuantity.ToString())
                                ? "Quantity is required."
                                : "Quantity cannot be negative.";
                isValid = false;
            }
            if (book.Cat == null)
            {
                CategoryError = "Please select a category.";
                isValid = false;
            }

            // Notify UI about error messages
            OnPropertyChanged(nameof(BookTitleError));
            OnPropertyChanged(nameof(AuthorError));
            OnPropertyChanged(nameof(IsbnError));
            OnPropertyChanged(nameof(PublisherError));
            OnPropertyChanged(nameof(PriceError));
            OnPropertyChanged(nameof(QuantityError));
            OnPropertyChanged(nameof(CategoryError));

            return isValid;
        }

        private void ClearErrorMessages()
        {
            BookTitleError = AuthorError = IsbnError = PublisherError = PriceError = QuantityError = CategoryError = string.Empty;
            OnPropertyChanged(nameof(BookTitleError));
            OnPropertyChanged(nameof(AuthorError));
            OnPropertyChanged(nameof(IsbnError));
            OnPropertyChanged(nameof(PublisherError));
            OnPropertyChanged(nameof(PriceError));
            OnPropertyChanged(nameof(QuantityError));
            OnPropertyChanged(nameof(CategoryError));
        }

        private void ModifyBook(object parameter)
        {
            ClearErrorMessages();
            if (SelectedBook == null)
            {
                MessageBox.Show("Please select a book to modify.", "Modify Book", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Perform validation on the TemporaryBook object
            if (!IsValidBook(TemporaryBook)) return;

            using (var context = new Prn212ProjectCapybookContext())
            {
                var bookToUpdate = context.Books.FirstOrDefault(b => b.BookId == SelectedBook.BookId);
                if (bookToUpdate != null)
                {
                    bookToUpdate.BookTitle = TemporaryBook.BookTitle;
                    bookToUpdate.Translator = TemporaryBook.Translator;
                    bookToUpdate.Author = TemporaryBook.Author;
                    bookToUpdate.Publisher = TemporaryBook.Publisher;
                    bookToUpdate.PublicationYear = TemporaryBook.PublicationYear;
                    bookToUpdate.Isbn = TemporaryBook.Isbn;
                    bookToUpdate.Dimension = TemporaryBook.Dimension;
                    bookToUpdate.Hardcover = TemporaryBook.Hardcover;
                    bookToUpdate.Weight = TemporaryBook.Weight;
                    bookToUpdate.BookStatus = TemporaryBook.BookStatus;
                    bookToUpdate.BookDescription = TemporaryBook.BookDescription;
                    bookToUpdate.Image = TemporaryBook.Image;
                    bookToUpdate.BookPrice = TemporaryBook.BookPrice;
                    bookToUpdate.BookQuantity = TemporaryBook.BookQuantity;
                    bookToUpdate.CatId = TemporaryBook.Cat?.CatId; // Update the category ID

                    context.Entry(bookToUpdate).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }

            LoadBooks();
            MessageBox.Show("Book modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteBook(object parameter)
        {
            if (SelectedBook != null)
            {
                using (var context = new Prn212ProjectCapybookContext())
                {
                    var bookToDelete = context.Books.FirstOrDefault(b => b.BookId == SelectedBook.BookId);
                    if (bookToDelete != null)
                    {
                        bookToDelete.BookStatus = 0;
                        context.Entry(bookToDelete).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadBooks();
                MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SearchBooks(object obj)
        {
            int cnt = 0;
            using (var context = new Prn212ProjectCapybookContext())
            {
                var query = context.Books
                    .Include(b => b.Cat) // Include category details
                    .Where(book => book.BookStatus != 0) // Only active books
                    .AsQueryable();

                // Apply filters based on filled-in fields
                if (!string.IsNullOrEmpty(TemporaryBook.BookTitle))
                {
                    query = query.Where(b => b.BookTitle.Contains(TemporaryBook.BookTitle));
                    cnt++;
                }
                if (!string.IsNullOrEmpty(TemporaryBook.Author))
                {
                    query = query.Where(b => b.Author.Contains(TemporaryBook.Author));
                    cnt++;
                }
                if (!string.IsNullOrEmpty(TemporaryBook.Translator))
                {
                    query = query.Where(b => b.Translator.Contains(TemporaryBook.Translator));
                    cnt++;
                }
                if (!string.IsNullOrEmpty(TemporaryBook.Publisher))
                {
                    query = query.Where(b => b.Publisher.Contains(TemporaryBook.Publisher));
                    cnt++;
                }
                if (TemporaryBook.PublicationYear != null)
                {
                    query = query.Where(b => b.PublicationYear == TemporaryBook.PublicationYear);
                    cnt++;
                }
                if (!string.IsNullOrEmpty(TemporaryBook.Isbn))
                {
                    query = query.Where(b => b.Isbn.Contains(TemporaryBook.Isbn));
                    cnt++;
                }
                if (TemporaryBook.Cat != null)
                {
                    query = query.Where(b => b.CatId == TemporaryBook.Cat.CatId);
                    cnt++;
                }

                // If no filters were applied, reload all books
                if (cnt == 0)
                {
                    LoadBooks();
                }
                else
                {
                    // Get filtered results and update the Books collection
                    var results = query.ToList();
                    Books.Clear();
                    foreach (var book in results)
                    {
                        Books.Add(book);
                    }
                }
//final
            }

            
        }

    }
}
