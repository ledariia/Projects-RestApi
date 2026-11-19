using System.Net;
using System.Net.Http.Json;
using Api.Contracts;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class BooksControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/books";
    private readonly Book _firstBook;
    private readonly Book _secondBook;

    public BooksControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstBook = BooksTestData.FirstTestBook();
        _secondBook = BooksTestData.SecondTestBook();
    }

    public async Task InitializeAsync()
    {
        await Context.Books.AddRangeAsync(_firstBook, _secondBook);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Books.RemoveRange(Context.Books);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    // --- POST /books ---

    [Fact]
    public async Task ShouldCreateBook()
    {
        // Arrange
        var request = new CreateBookRequest("Refactoring", "Martin Fowler", "Programming", 448);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var created = await response.Content.ReadFromJsonAsync<BookResponse>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBeEmpty();

        var dbBook = await Context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == new BookId(created.Id));
        dbBook.Should().NotBeNull();
        dbBook!.Title.Should().Be(request.Title);
        dbBook.Author.Should().Be(request.Author);
        dbBook.Genre.Should().Be(request.Genre);
        dbBook.PageCount.Should().Be(request.PageCount);
    }

    [Fact]
    public async Task ShouldNotCreateBook_WhenTitleAlreadyExists()
    {
        // Arrange
        var request = new CreateBookRequest(_firstBook.Title, "Another Author", "Drama", 300);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var booksWithTitle = await Context.Books.AsNoTracking()
            .Where(b => b.Title == _firstBook.Title)
            .ToListAsync();
        booksWithTitle.Should().ContainSingle();
        booksWithTitle[0].Id.Should().Be(_firstBook.Id);
    }

    [Fact]
    public async Task ShouldNotCreateBook_WhenDataIsInvalid()
    {
        // Arrange
        var request = new CreateBookRequest("", "", "", -10);

        // Act
        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var count = await Context.Books.AsNoTracking().CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task ShouldUpdateBook()
    {
        // Arrange
        var request = new UpdateBookRequest("Updated Title", "Updated Author", "Sci-Fi", 512);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{_firstBook.Id.Value}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var dbBook = await Context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == _firstBook.Id);
        dbBook.Should().NotBeNull();
        dbBook!.Title.Should().Be(request.Title);
        dbBook.Author.Should().Be(request.Author);
        dbBook.Genre.Should().Be(request.Genre);
        dbBook.PageCount.Should().Be(request.PageCount);
    }

    [Fact]
    public async Task ShouldUpdateBook_WhenTitleIsUnchanged()
    {
        // Arrange
        var request = new UpdateBookRequest(_firstBook.Title, "New Author", "New Genre", 333);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{_firstBook.Id.Value}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var dbBook = await Context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == _firstBook.Id);
        dbBook.Should().NotBeNull();
        dbBook!.Title.Should().Be(_firstBook.Title);
        dbBook.Author.Should().Be(request.Author);
        dbBook.PageCount.Should().Be(request.PageCount);
    }

    [Fact]
    public async Task ShouldNotUpdateBook_WhenTitleBelongsToAnotherBook()
    {
        // Arrange
        var request = new UpdateBookRequest(_secondBook.Title, "Some Author", "Some Genre", 200);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{_firstBook.Id.Value}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var dbBook = await Context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == _firstBook.Id);
        dbBook!.Title.Should().Be(_firstBook.Title);
        dbBook.Author.Should().Be(_firstBook.Author);
        dbBook.PageCount.Should().Be(_firstBook.PageCount);
    }

    [Fact]
    public async Task ShouldNotUpdateBook_WhenBookDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateBookRequest("Ghost Book", "Ghost Author", "Horror", 100);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{nonExistentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldNotUpdateBook_WhenDataIsInvalid()
    {
        // Arrange
        var request = new UpdateBookRequest("", "", "", 0);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{_firstBook.Id.Value}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var dbBook = await Context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == _firstBook.Id);
        dbBook!.Title.Should().Be(_firstBook.Title);
        dbBook.Author.Should().Be(_firstBook.Author);
        dbBook.PageCount.Should().Be(_firstBook.PageCount);
    }

    [Fact]
    public async Task ShouldGetAllBooks()
    {
        // Act
        var response = await Client.GetAsync(BaseRoute);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books.Should().NotBeNull();
        books!.Should().HaveCount(2);
        books.Select(b => b.Id).Should().BeEquivalentTo(new[] { _firstBook.Id.Value, _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldGetBookById()
    {
        // Act
        var response = await Client.GetAsync($"{BaseRoute}/{_firstBook.Id.Value}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var book = await response.Content.ReadFromJsonAsync<BookResponse>();
        book.Should().NotBeNull();
        book!.Id.Should().Be(_firstBook.Id.Value);
        book.Title.Should().Be(_firstBook.Title);
        book.Author.Should().Be(_firstBook.Author);
        book.Genre.Should().Be(_firstBook.Genre);
        book.PageCount.Should().Be(_firstBook.PageCount);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Act
        var response = await Client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}