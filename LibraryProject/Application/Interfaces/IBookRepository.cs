using Domain;
using LanguageExt;

namespace Application.Interfaces;

public interface IBookRepository
{
    Task<Option<Book>> GetByIdAsync(BookId id, CancellationToken cancellationToken);
    Task<Option<Book>> GetByTitleAsync(string title, CancellationToken cancellationToken);
    Task AddAsync(Book book, CancellationToken cancellationToken);
    Task UpdateAsync(Book book, CancellationToken cancellationToken);
}