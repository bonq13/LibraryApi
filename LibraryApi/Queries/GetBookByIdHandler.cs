using LibraryApi.Models;
using LibraryApi.Repositories;
using MediatR;

namespace LibraryApi.Queries;

public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Book?>
{
    private readonly IBookRepository _bookRepository;
    public GetBookByIdHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public async Task<Book?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetBookById(request.Id);
    }
}