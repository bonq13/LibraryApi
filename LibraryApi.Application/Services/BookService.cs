using LibraryApi.Domain.Entities;
using LibraryApi.Application.Repositories;

namespace LibraryApi.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public async Task<List<Book>> GetAllBooks()
    {
        return await _bookRepository.GetAllBooks();
    }
    
    public async Task<Book?> GetBookById(int id)
    {
        return await _bookRepository.GetBookById(id);
    }
    
    public async Task<bool> AddBook(Book book)
    {
        var existingBook = await _bookRepository.GetAllBooks();
        if (existingBook.Any(b => b.Title == book.Title)) return false;
        
        await _bookRepository.AddBook(book);
        return true;
    }
}