using LibraryApi.Models;
using LibraryApi.Repositories;
using LibraryApi.Services;
using MediatR;

namespace LibraryApi.Commands;

public class AddBookHandler : IRequestHandler<AddBookCommand, bool>
{
    private readonly IBookService _bookService;

    public AddBookHandler(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    public async Task<bool> Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        return await _bookService.AddBook(new Book 
        { 
            Title = request.Title, 
            Author = request.Author, 
            IsAvailable = request.IsAvailable 
        });
    }
}