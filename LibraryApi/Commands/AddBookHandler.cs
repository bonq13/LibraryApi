using LibraryApi.Models;
using LibraryApi.Repositories;
using LibraryApi.Services;
using MediatR;

namespace LibraryApi.Commands;

public class AddBookHandler : IRequestHandler<AddBookCommand, Book?>
{
    private readonly IBookService _bookService;

    public AddBookHandler(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<Book?> Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            IsAvailable = request.IsAvailable
        };

        var success = await _bookService.AddBook(book);
        return success ? book : null;
    }
}
