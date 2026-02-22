using BookCollection.Model;
using System.Collections.Generic;


namespace BookCollection.Repositories
{
    internal interface IBookRepository
    {
        Book Create(Book book);
        IEnumerable<Book> GetAll();
        Book GetById(int id);
        bool Update(Book book);
        bool Delete(int id);

    }
}
