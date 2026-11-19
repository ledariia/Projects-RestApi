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

public class BooksFilterTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string FilterRoute = "/books/filter";

    private readonly Book _firstBook;
    private readonly Book _secondBook;
    private readonly Book _thirdBook;
    private readonly Book _fourthBook;

    private DateTime _secondCreatedAt;
    private DateTime _thirdCreatedAt;

    public BooksFilterTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _firstBook = BooksTestData.FirstTestBook();
        Thread.Sleep(50);
        _secondBook = BooksTestData.SecondTestBook();
        Thread.Sleep(50);
        _thirdBook = BooksTestData.ThirdTestBook();
        Thread.Sleep(50);
        _fourthBook = BooksTestData.FourthTestBook();
    }

    public async Task InitializeAsync()
    {
        await Context.Books.AddRangeAsync(_firstBook, _secondBook, _thirdBook, _fourthBook);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var dbBooks = await Context.Books.AsNoTracking().ToListAsync();
        _secondCreatedAt = dbBooks.Single(b => b.Id == _secondBook.Id).CreatedAt;
        _thirdCreatedAt = dbBooks.Single(b => b.Id == _thirdBook.Id).CreatedAt;
    }

    public async Task DisposeAsync()
    {
        Context.Books.RemoveRange(Context.Books);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ShouldFilterByTitle_PartialMatch()
    {
        // Arrange
        var filter = new SearchBookDto(Title: "Clean");

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _firstBook.Id.Value, _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByAuthor_PartialMatch()
    {
        // Arrange
        var filter = new SearchBookDto(Author: "Martin");

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _firstBook.Id.Value, _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByGenre_ExactMatch()
    {
        // Arrange
        var filter = new SearchBookDto(Genre: "Architecture");

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _secondBook.Id.Value, _thirdBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByPagesCountFrom()
    {
        // Arrange
        var filter = new SearchBookDto(PagesCountFrom: 500);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _thirdBook.Id.Value, _fourthBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByPagesCountTo()
    {
        // Arrange
        var filter = new SearchBookDto(PagesCountTo: 450);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _firstBook.Id.Value, _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByCreatedAtFrom()
    {
        // Arrange
        var filter = new SearchBookDto(CreatedAtFrom: _thirdCreatedAt);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _thirdBook.Id.Value, _fourthBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByCreatedAtTo()
    {
        // Arrange
        var filter = new SearchBookDto(CreatedAtTo: _secondCreatedAt);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _firstBook.Id.Value, _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByString_CaseInsensitive()
    {
        // Arrange
        var filter = new SearchBookDto(Title: "cLeAn");

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _firstBook.Id.Value, _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByPagesCount_WithInclusiveBoundaries()
    {
        // Arrange (432 та 560 точні значення другої та третьої книг)
        var filter = new SearchBookDto(PagesCountFrom: 432, PagesCountTo: 560);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _secondBook.Id.Value, _thirdBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByCreatedAt_WithInclusiveBoundaries()
    {
        // Arrange
        var filter = new SearchBookDto(CreatedAtFrom: _secondCreatedAt, CreatedAtTo: _thirdCreatedAt);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _secondBook.Id.Value, _thirdBook.Id.Value });
    }

    [Fact]
    public async Task ShouldFilterByMultipleParameters_LogicalAnd()
    {
        // Arrange
        var filter = new SearchBookDto(Author: "Martin", Genre: "Architecture", PagesCountFrom: 300);

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[] { _secondBook.Id.Value });
    }

    [Fact]
    public async Task ShouldReturnAllBooks_WhenFilterIsEmpty()
    {
        // Arrange
        var filter = new SearchBookDto();

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books!.Select(b => b.Id).Should().BeEquivalentTo(new[]
        {
            _firstBook.Id.Value,
            _secondBook.Id.Value,
            _thirdBook.Id.Value,
            _fourthBook.Id.Value
        });
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoBooksMatchFilter()
    {
        // Arrange
        var filter = new SearchBookDto(Title: "NonExistentBookTitle");

        // Act
        var response = await Client.PostAsJsonAsync(FilterRoute, filter);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        books.Should().NotBeNull();
        books.Should().BeEmpty();
    }
}