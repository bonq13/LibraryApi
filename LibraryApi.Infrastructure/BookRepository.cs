using LibraryApi.Application.Repositories;
using LibraryApi.Application.Specifications;
using LibraryApi.Domain.Entities;
using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllBooks()
    {
        var books = await _context.Books.ToListAsync();
        return books;
    }
    
    public async Task<Book?> GetBookById(int id)
    {
        var book = await _context.Books.FindAsync(id);
        return book;
    }
    
    public async Task AddBook(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Book>> GetBySpecification(ISpecification<Book> spec)
    {
        return await _context.Books.Where(spec.Criteria).ToListAsync();
    }
}