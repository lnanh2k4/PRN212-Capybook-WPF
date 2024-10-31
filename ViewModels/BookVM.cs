using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Capybook.Models;
using Capybook.Utilities; // Assuming you have a RelayCommand or similar utility
using Microsoft.EntityFrameworkCore;

namespace Capybook.ViewModels
{
    public class BookVM : BaseVM
    {
        public ObservableCollection<Book> Books { get; set; }
        private Book _selectedBook;

        // SelectedBook is used for the ListView selection
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

        // TemporaryBook is used for editing details in the UI
        public Book TemporaryBook { get; set; }

        // Commands for Add, Edit, and Delete
        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public BookVM()
        {
            Books = new ObservableCollection<Book>();
            TemporaryBook = new Book();

            // Initialize commands
            AddCommand = new RelayCommand(AddBook);
            ModifyCommand = new RelayCommand(ModifyBook);
            DeleteCommand = new RelayCommand(DeleteBook);

            LoadBooks(); // Load books from the database
        }

        private void LoadBooks()
        {
            Books.Clear();
            using (var context = new Prn212ProjectCapybookContext())
            {
                var booksFromDb = context.Books
                    .Where(book => book.BookStatus != 0) // Only include books with BookStatus != 0
                    .ToList();
                foreach (var book in booksFromDb)
                {
                    Books.Add(book);
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
                Image = selectedBook.Image
            };
            OnPropertyChanged(nameof(TemporaryBook));
        }

        private void AddBook(object parameter)
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
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
                    Image = TemporaryBook.Image
                };

                context.Books.Add(newBook);
                context.SaveChanges();
                LoadBooks();
                MessageBox.Show("Book added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ModifyBook(object parameter)
        {
            if (SelectedBook != null)
            {
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

                        context.Entry(bookToUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                LoadBooks();
                MessageBox.Show("Book modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
    }
}