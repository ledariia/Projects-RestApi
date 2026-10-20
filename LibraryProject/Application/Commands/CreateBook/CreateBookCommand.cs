using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using LanguageExt;
using MediatR;

namespace Application.Commands.CreateBook;

public record CreateBookCommand(string Title, string Author, string Genre, int PageCount) 
    : IRequest<Either<BookError, Book>>;

public class CreateBookValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PageCount).GreaterThan(0);
    }
}

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Either<BookError, Book>>
{
    private readonly IBookRepository _repository;

    public CreateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<BookError, Book>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingBook = await _repository.GetByTitleAsync(request.Title, cancellationToken);
            if (existingBook.IsSome)
            {
                return new BookAlreadyExistsError(BookId.Empty(), $"Книга з назвою '{request.Title}' вже існує.");
            }

            var book = Book.Create(request.Title, request.Author, request.Genre, request.PageCount);
            await _repository.AddAsync(book, cancellationToken);

            return book;
        }
        catch (Exception ex)
        {
            return new BookUnhandledError(BookId.Empty(), "Сталася непередбачена помилка під час створення.", ex);
        }
    }
}