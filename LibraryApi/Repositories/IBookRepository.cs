using LibraryApi.Models;
using LibraryApi.Specifications;

namespace LibraryApi.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllBooks();
    Task<Book?> GetBookById(int id);
    Task AddBook(Book book);
    Task<List<Book>> GetBySpecification(ISpecification<Book> spec);

}