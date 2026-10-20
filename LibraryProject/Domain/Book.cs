using System;

namespace Domain;

public class Book
{
    public BookId Id { get; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Genre { get; private set; }
    public int PageCount { get; private set; }
    public DateTime CreatedAt { get; }
    private Book(BookId id, string title, string author, string genre, int pageCount, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Author = author;
        Genre = genre;
        PageCount = pageCount;
        CreatedAt = createdAt;
    }
    public static Book Create(string title, string author, string genre, int pageCount)
    {
        return new Book(
            BookId.New(),
            title,
            author,
            genre,
            pageCount,
            DateTime.UtcNow
        );
    }
    public void UpdateDetails(string title, string author, string genre, int pageCount)
    {
        Title = title;
        Author = author;
        Genre = genre;
        PageCount = pageCount;
    }
}