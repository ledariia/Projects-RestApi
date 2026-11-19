﻿﻿using Domain;
using LanguageExt;

namespace Application.Interfaces;

public interface IBookQueries
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken);
    Task<Option<Book>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Book>> FilterAsync(
        string? title,
        string? author,
        string? genre,
        int? pagesCountFrom,
        int? pagesCountTo,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken);
}
