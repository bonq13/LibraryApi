using LibraryApi.Models;
using MediatR;

namespace LibraryApi.Queries;

public record GetBookByIdQuery(int Id) : IRequest<Book?>;