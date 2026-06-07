using MediatR;

namespace LibraryApi.Commands;

public record AddBookCommand(string Title, string Author, bool IsAvailable) : IRequest<bool>;