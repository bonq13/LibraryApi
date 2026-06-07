using LibraryApi.Models;

namespace LibraryApi.Services;

public interface IBookService
{
    Task<List<Book>> GetAllBooks();
    Task<Book?> GetBookById(int id);
    Task<bool> AddBook(Book book);
}