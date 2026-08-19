using LibraryApi.Domain.Entities;
using MediatR;

namespace LibraryApi.Application.Queries;

public record GetBookByIdQuery(int Id) : IRequest<Book?>;