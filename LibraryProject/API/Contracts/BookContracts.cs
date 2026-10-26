using Domain;

namespace Api.Contracts;

public record BookResponse(Guid Id, string Title, string Author, string Genre, int PageCount)
{
    public static BookResponse FromDomain(Book book) =>
        new(book.Id.Value, book.Title, book.Author, book.Genre, book.PageCount);
}

public record CreateBookRequest(string Title, string Author, string Genre, int PageCount);
public record UpdateBookRequest(string Title, string Author, string Genre, int PageCount);