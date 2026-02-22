using BookCollection.Commands;
using BookCollection.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using BookCollection.Repositories;

namespace BookCollection.ViewModel
{
    // Enum for color state of input values
    public enum InputState
    {
        Normal,
        Warning,
        Critical
    }
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        //////////////////////////////////         WINDOW TITEL         //////////////////////////////////
        // At start APP titel than either Loadbooks or FilterBooks
        private string _appHeading = "Private Book collection";
        public string AppHeading
        {
            get { return _appHeading; }
            set
            {
                if (_appHeading != value)
                {
                    _appHeading = value;
                    OnPropertyChanged();
                }
            }
        }

        //////////////////////////////////         DATA -> Repo object        //////////////////////////////////
        private readonly BookRepository _repo;

        // VM display
        private readonly ObservableCollection<Book> _books = new ObservableCollection<Book>();
        public ObservableCollection<Book> Books => _books;


        //////////////////////////////////         SELECTED ELEMENT         //////////////////////////////
        private Book _selectedBook;
        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                if(_selectedBook != value)
                {
                    _selectedBook = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsSelected));

                    FillFormFromSelectedBook();
                }
            }
        }

        // EDIT and DELETE BUTTON VISIBILITY
        public bool IsSelected => _selectedBook != null;

        //////////////////////////////////         VIEW FORM         //////////////////////////////////
        // FORM elements value placeholders from View of Book attributes
        private string _publicationYear = string.Empty;
        private string _title = string.Empty;
        private string _author = string.Empty;
        private string _genre = string.Empty;

        public string PublicationYear
        {
            get { return _publicationYear; }
            set
            {
                if (_publicationYear != value)
                {
                    _publicationYear = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if(_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                    // Update the counter properties when Title changes
                    OnPropertyChanged(nameof(TitleLength));
                    OnPropertyChanged(nameof(TitleCounter));
                    // Color state for Title input
                    OnPropertyChanged(nameof(TitleState));
                }
            }
        }
        public string Author
        {
            get { return _author; }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    OnPropertyChanged();
                    // Update the counter properties when Author changes
                    OnPropertyChanged(nameof(AuthorLength));
                    OnPropertyChanged(nameof(AuthorCounter));
                    // Color state for Author input
                    OnPropertyChanged(nameof(AuthorState));
                }
            }
        }
        public string Genre
        {
            get { return _genre; }
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged();
                    // Update the counter properties when Genre changes
                    OnPropertyChanged(nameof(GenreLength));
                    OnPropertyChanged(nameof(GenreCounter));
                    // Color state for Genre input
                    OnPropertyChanged(nameof(GenreState));
                }
            }
        }
        //////////////////////////////////         COUNTERS         //////////////////////////////////

        // Color state properties for View display
        public InputState TitleState => GetState(TitleLength);
        public InputState AuthorState => GetState(AuthorLength);
        public InputState GenreState => GetState(GenreLength);

        // Counter values for Input values length check
        public int TitleLength => Title?.Length ?? 0;
        public int AuthorLength => Author?.Length ?? 0;
        public int GenreLength => Genre?.Length ?? 0;

        // Counter properties for View display
        public string TitleCounter => $"{TitleLength}/{MaxInputLength}";
        public string AuthorCounter => $"{AuthorLength}/{MaxInputLength}";
        public string GenreCounter => $"{GenreLength}/{MaxInputLength}";

        // Max length for input values
        public const int MAX_INPUT_LENGTH = 100;
        public static int MaxInputLength => MAX_INPUT_LENGTH;
        public static int MaxYearLength => 4;

        //////////////////////////////////         STATUS MESSAGE         //////////////////////////////////
        private string _notifyMessage = string.Empty;
        public string NotifyMessage 
        {
            get { return _notifyMessage; }
            set
            {
                if (_notifyMessage != value)
                {
                    _notifyMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        //////////////////////////////////         COMMANDS         //////////////////////////////////
        public ICommand CreateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DisplayAllCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ExitAppCommand { get; }
        public ICommand ModifyCommand { get; }
        public ICommand ClearFormCommand { get; }

        //////////////////////////////////         CONSTRUCTOR         //////////////////////////////////
        public MainWindowViewModel()
        {
            // Create repo for VM
            _repo = new BookRepository();

            // COMMANDS with RULES
            ExitAppCommand = new RelayCommand(o => Application.Current.Shutdown());
            ClearFormCommand = new RelayCommand(o => { ClearMessage(); SelectedBook = null; ResetValues(); });

            FilterCommand = new RelayCommand(o => { ClearMessage(); SelectedBook = null; FilterBooks(); });

            DisplayAllCommand = new RelayCommand(o => { ClearMessage(); SelectedBook = null; LoadBooks();});
            CreateCommand = new RelayCommand(o => { ClearMessage(); CreateBook();});
            ModifyCommand = new RelayCommand(o => { ClearMessage(); ModifyBook(); }, o=> SelectedBook != null);
            DeleteCommand = new RelayCommand(o => { ClearMessage(); DeleteBook(); }, o => SelectedBook != null);

            // Fill View with current data at start
            LoadBooks();
        }

        //////////////////////////////////         METHODS         //////////////////////////////////
        // CRUD METHODS

        // READ: SELECT ALL
        public void LoadBooks()
        {
            AppHeading = "Display all Book";

            // Clear the result list
            _books.Clear();

            // Insert every Book from Books repo into result
            foreach(var b in _repo.GetAll())
            {
               _books.Add(b);
            }
        }

        // CREATE: NEW
        public void CreateBook()
        {
            int year;

            // Sanitize the input values
            string title = Title?.Trim();
            string author = Author?.Trim();
            string genre = Genre?.Trim();

            // Check Form
            if (!IsFormValid(title, author, genre, out year)) return;

            // Create an obj from input values
            Book b = new Book(title, author, genre, year);

            // Check unique
            bool isBookUnique = Validator.IsBookUnique(b, _repo.GetAll());

            if (!isBookUnique)
            {
                SetMessage("The Book with these Title + Author from the same year is already exists.");
                return;
            }

            // Add obj to our DATA, save resulted book in variable (for id)
            Book createdBook = _repo.Create(b);

            // Empty view elements
            ResetValues();
            // Refresh list
            LoadBooks();

            // Nofify user from succeed
            SetMessage($"New Book with id {createdBook.Id} was created!");

        }

        // UPDATE: MODIFY
        public void ModifyBook()
        {
            if (SelectedBook == null) return;
            int year;
            // Sanitize the input values
            string title = Title?.Trim();
            string author = Author?.Trim();
            string genre = Genre?.Trim();
            // Check Form
            if (!IsFormValid(title, author, genre, out year)) return;

            var id = SelectedBook.Id;

            var updated = new Book(title, author, genre, year) { Id = id };

            // For simpler variable name
            Book sb = SelectedBook;

            // If the selected book and the input values are identical, return
            bool unchanged = 
                sb.Title == updated.Title &&
                sb.Author == updated.Author &&
                sb.Genre == updated.Genre &&
                sb.PublicationYear == updated.PublicationYear;

            if (unchanged)
            {
                SetMessage("Selected book and input values are the same. No changes executed.");
                return;
            }

            // Check unique
            bool isBookUnique = Validator.IsBookUnique(updated, _repo.GetAll().Where(b => b.Id != updated.Id));

            if (!isBookUnique) 
            {
                SetMessage("The Book with these Title + Author from the same year is already exists.");
                return;
            }

            _repo.Update(updated);

            // Reset selected Object
            SelectedBook = null;

            // Empty view form elements
            ResetValues();

            // Refresh list
            LoadBooks();

            // Nofify user from succeed
            SetMessage($"Book with the id {id} updated!");
        }

        // DELETE
        public void DeleteBook()
        {
            // Guard
            if (SelectedBook == null) return;

            // Save id for message
            var id = SelectedBook.Id;

            // Remove book
            _repo.Delete(SelectedBook.Id);

            // Reset selected
            SelectedBook = null;
            // Empty view elements
            ResetValues();
            // Reload data
            LoadBooks();

            // Nofify user from succeed
            SetMessage($"Book with the id {id} deleted!");

        }

        // READ: SELECT FILTERED
        public void FilterBooks()
        {
            // Clear the result list
            _books.Clear();

            // Sanitize the input values
            string title = Title?.Trim();
            string author = Author?.Trim();
            string genre = Genre?.Trim();

            // Create copy from all Book to an object
            IEnumerable<Book> resultBooks = _repo.GetAll();


            if (!string.IsNullOrWhiteSpace(title))
                resultBooks = resultBooks.Where(b => Match(b.Title, title));

            if (!string.IsNullOrWhiteSpace(author))
                resultBooks = resultBooks.Where(b => Match(b.Author, author));

            if (!string.IsNullOrWhiteSpace(genre))
                resultBooks = resultBooks.Where(b => Match(b.Genre, genre));

            // Check Year if not empty
            if (!string.IsNullOrWhiteSpace(PublicationYear) && int.TryParse(PublicationYear, out int y))
            {
                resultBooks = resultBooks.Where(book => book.PublicationYear == y);
            }
            // Materialise the IEnumerable
            var resultList = resultBooks.ToList();

            // Loop through the resultList, add each Obj to _books(view)
            foreach (var b in resultList)
            {
                _books.Add(b);
            }

            // Nofify user from succeed
            AppHeading = "Filtered list:";

            // Empty result
            if(_books.Count() == 0)
            {
                SetMessage("No match!");
            }
        }

        // Helpers
        private InputState GetState(int length)
        {
            double ratio = (double)length / MaxInputLength;

            if (ratio < 0.7)
                return InputState.Normal;

            if (ratio < 0.9)
                return InputState.Warning;

            return InputState.Critical;
        }
        private void FillFormFromSelectedBook()
        {
            if (SelectedBook == null) return;

            Title = SelectedBook.Title;
            Author = SelectedBook.Author;
            Genre = SelectedBook.Genre;
            PublicationYear = SelectedBook.PublicationYear.ToString();


        }

        // Validate Input values
        public bool IsFormValid(string title,string author, string genre, out int year )
        {
            year = 0;

            if (!Validator.ValidateEmpty(title))
            {
                SetMessage("Please enter Title");
                return false;
            }

            if (!Validator.ValidateEmpty(author))
            {
                SetMessage("Please enter Author");
                return false;
            }

            if (!Validator.ValidateEmpty(genre))
            {
                SetMessage("Please enter Genre");
                return false;
            }

            if (!Validator.ValidateEmpty(PublicationYear))
            {
                SetMessage("Please enter Year");
                return false;
            }

            if (!int.TryParse(PublicationYear, out year))
            {
                SetMessage("Year must be a whole number.");
                return false;
            }

            if (!Validator.ValidateYear(PublicationYear))
            {
                SetMessage("Please enter a valid year (0 < and < Future)");
                return false;
            }

            return true;
        }

        // Filter helper
        private static bool Match(string source, string filter)
        {
            // no Filter skipp filter for selected property
            if (string.IsNullOrWhiteSpace(filter))
                return true; 

            // Source check before method call
            return source?.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Clear the elements in Form
        public void ResetValues()
        {
            Title = string.Empty;
            Author = string.Empty;
            Genre = string.Empty;
            PublicationYear = string.Empty;
        }

        // Set, clear Notify msg
        private void SetMessage(string msg)
        {
            NotifyMessage = msg;
        }

        private void ClearMessage()
        {
            NotifyMessage = string.Empty;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler PropertyChanged;

        // Give caller name as argument, no need for nameof...
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
