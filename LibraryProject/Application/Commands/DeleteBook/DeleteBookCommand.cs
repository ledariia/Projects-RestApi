using Application.Errors;
using Application.Interfaces;
using LanguageExt;
using MediatR;
using Unit = MediatR.Unit; 

namespace Application.Commands.DeleteBook;

public record DeleteBookCommand(Guid Id) : IRequest<Either<BookError, Unit>>;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Either<BookError, Unit>>
{
    private readonly IBookRepository _repository;

    public DeleteBookCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Either<BookError, Unit>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var bookOption = await _repository.GetByIdAsync(new Domain.BookId(request.Id), cancellationToken);

        return await bookOption.Match<Task<Either<BookError, Unit>>>(
            Some: async book =>
            {
                await _repository.DeleteAsync(book, cancellationToken);
                return Unit.Value;
            },
            None: () => Task.FromResult<Either<BookError, Unit>>(
                new BookNotFoundError(new Domain.BookId(request.Id), $"Книгу з ID {request.Id} не знайдено."))
        );
    }
}