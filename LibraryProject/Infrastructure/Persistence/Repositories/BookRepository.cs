using Application.Interfaces;
using Domain;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository, IBookQueries
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Option<Book>> GetByIdAsync(BookId id, CancellationToken cancellationToken)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        return Optional(book);
    }

    public async Task<Option<Book>> GetByTitleAsync(string title, CancellationToken cancellationToken)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == title, cancellationToken);
        return Optional(book);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken)
    {
        await _context.Books.AddAsync(book, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Books.AsNoTracking().ToListAsync(cancellationToken);
    }

    async Task<Option<Book>> IBookQueries.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var bookId = new BookId(id);
        var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
        return Optional(book);
    }
}