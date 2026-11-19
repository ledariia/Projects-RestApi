﻿﻿using Application.Interfaces;
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
    public Task DeleteAsync(Domain.Book book, CancellationToken cancellationToken)
    {
        _context.Books.Remove(book);
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMassiveAsync(CancellationToken cancellationToken)
    {
        var books= await _context.Books.Where(b=>b.PageCount>=1000000).ToListAsync(cancellationToken);
        _context.Books.RemoveRange(books);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Book>> FilterAsync(
        string? title,
        string? author,
        string? genre,
        int? pagesCountFrom,
        int? pagesCountTo,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken)
    {
        IQueryable<Book> query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => EF.Functions.ILike(b.Title, $"%{title.Trim()}%"));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => EF.Functions.ILike(b.Author, $"%{author.Trim()}%"));

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(b => b.Genre.ToLower() == genre.Trim().ToLower());

        if (pagesCountFrom.HasValue)
            query = query.Where(b => b.PageCount >= pagesCountFrom.Value);

        if (pagesCountTo.HasValue)
            query = query.Where(b => b.PageCount <= pagesCountTo.Value);

        if (createdAtFrom.HasValue)
            query = query.Where(b => b.CreatedAt >= DateTime.SpecifyKind(createdAtFrom.Value, DateTimeKind.Utc));

        if (createdAtTo.HasValue)
            query = query.Where(b => b.CreatedAt <= DateTime.SpecifyKind(createdAtTo.Value, DateTimeKind.Utc));

        return await query.ToListAsync(cancellationToken);
    }
}