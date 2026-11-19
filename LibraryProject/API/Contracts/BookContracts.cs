﻿﻿using Domain;

namespace Api.Contracts;

public record BookResponse(Guid Id, string Title, string Author, string Genre, int PageCount)
{
    public static BookResponse FromDomain(Book book) =>
        new(book.Id.Value, book.Title, book.Author, book.Genre, book.PageCount);
}
public record SearchBookDto(
    string? Title = null,
    string? Author = null,
    string? Genre = null,
    int? PagesCountFrom = null,
    int? PagesCountTo = null,
    DateTime? CreatedAtFrom = null,
    DateTime? CreatedAtTo = null);

public record CreateBookRequest(string Title, string Author, string Genre, int PageCount);
public record UpdateBookRequest(string Title, string Author, string Genre, int PageCount);