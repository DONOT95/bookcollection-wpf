using BookCollection.Model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BookCollection.Repositories
{
    // Clean repository: set auto id and manage Books uses Interface
    internal class BookRepository : IBookRepository
    {
        // Data
        private readonly List<Book> _books = new List<Book>();

        // Dinamic ID for book
        private int _nextId = 1;

        // Constructor
        public BookRepository()
        {
            // Dummy data
            SetDefaultData();
            RecalculateNextId();
        }

        // CRUD

        // Create:
        public Book Create(Book book)
        {
            // Set current id, increase value for next
            book.Id = _nextId++;
            // Add object to list
            _books.Add(book);
           
            return book;
        }

        // Read All:
        public IEnumerable<Book> GetAll()
        {
            // data copy
            return _books.ToList();
        }

        // Read (id):
        public Book GetById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        // Delete:
        public bool Delete(int id)
        {
            var existing = GetById(id);

            if (existing == null) return false;

            _books.Remove(existing);
            return true;
        }

        // Update:
        public bool Update(Book newValues)
        {
            var existing = GetById(newValues.Id);
            if (existing == null) return false;

            existing.Title = newValues.Title;
            existing.Author = newValues.Author;
            existing.Genre = newValues.Genre;
            existing.PublicationYear = newValues.PublicationYear;
            return true;
        }

        // Help methods
        private void SetDefaultData()
        {
            string title = "Title";
            string author = "Author";
            string genre = "Genre";
            int year = 2000;

            for (int i = 1; i <= 20; i++)
            {
                _books.Add(new Book($"{title}{i}", $"{author}{i}", $"{genre}{i}", year + i) { Id = i });
            }
            
        }
        
        private void RecalculateNextId()
        {
            // Calc id 1x in constructor call
            _nextId = _books.Count == 0 ? 1 : _books.Max(b => b.Id) + 1;
        }



    }
}
