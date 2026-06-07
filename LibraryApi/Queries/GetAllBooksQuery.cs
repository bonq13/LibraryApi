using LibraryApi.Models;
using MediatR;

namespace LibraryApi.Queries;

public record GetAllBooksQuery() : IRequest<List<Book>>;