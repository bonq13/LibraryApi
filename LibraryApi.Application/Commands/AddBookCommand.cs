using LibraryApi.Domain.Entities;
using MediatR;

namespace LibraryApi.Application.Commands;

public record AddBookCommand(string Title, string Author, bool IsAvailable) : IRequest<Book?>;