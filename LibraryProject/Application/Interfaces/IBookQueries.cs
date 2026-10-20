using Domain;
using LanguageExt;

namespace Application.Interfaces;

public interface IBookQueries
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken);
    Task<Option<Book>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
