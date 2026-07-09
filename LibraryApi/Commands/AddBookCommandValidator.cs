using FluentValidation;

namespace LibraryApi.Commands;

public class AddBookCommandValidator : AbstractValidator<AddBookCommand>
{
    public AddBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Tytuł jest wymagany")
            .MinimumLength(2)
            .WithMessage("Tytuł za krótki - minimum 2 znaki")
            .MaximumLength(200)
            .WithMessage("Tytuł za długi - max 200 znaków");

        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage("Autor jest wymagany")
            .MinimumLength(2)
            .WithMessage("Autor za krótki - minimum 2 znaki")
            .MaximumLength(200)
            .WithMessage("Autor za długi - max 200 znaków");
        
        

    }
}