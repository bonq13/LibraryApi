using LibraryApi.Domain.Entities;
using MediatR;

namespace LibraryApi.Application.Queries;

public record GetAllBooksQuery() : IRequest<List<Book>>;