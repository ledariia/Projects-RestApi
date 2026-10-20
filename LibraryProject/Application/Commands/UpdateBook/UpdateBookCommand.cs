using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using LanguageExt;
using MediatR;

namespace Application.Commands.UpdateBook;

public record UpdateBookCommand(Guid Id, string Title, string Author, string Genre, int PageCount) 
    : IRequest<Either<BookError, Book>>;

public class UpdateBookValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PageCount).GreaterThan(0);
    }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Either<BookError, Book>>
{
    private readonly IBookRepository _repository;

    public UpdateBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<BookError, Book>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bookId = new BookId(request.Id);
            var bookOption = await _repository.GetByIdAsync(bookId, cancellationToken);

            if (bookOption.IsNone)
            {
                return new BookNotFoundError(bookId, $"Книгу з ID {request.Id} не знайдено.");
            }

            var existingWithSameTitle = await _repository.GetByTitleAsync(request.Title, cancellationToken);
            var isTitleTakenByOther = existingWithSameTitle.Match(
                Some: b => b.Id.Value != request.Id,
                None: () => false
            );

            if (isTitleTakenByOther)
            {
                return new BookAlreadyExistsError(bookId, $"Книга з назвою '{request.Title}' вже існує.");
            }

            var book = bookOption.IfNone(() => throw new InvalidOperationException());
            book.UpdateDetails(request.Title, request.Author, request.Genre, request.PageCount);
            
            await _repository.UpdateAsync(book, cancellationToken);

            return book;
        }
        catch (Exception ex)
        {
            return new BookUnhandledError(new BookId(request.Id), "Сталася непередбачена помилка під час оновлення.", ex);
        }
    }
}