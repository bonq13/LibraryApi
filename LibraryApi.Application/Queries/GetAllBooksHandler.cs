using LibraryApi.Application.Repositories;
using LibraryApi.Domain.Entities;
using MediatR;

namespace LibraryApi.Application.Queries;

public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, List<Book>>
{
    private readonly IBookRepository _bookRepository;
    public GetAllBooksHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public async Task<List<Book>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetAllBooks();
    }
}