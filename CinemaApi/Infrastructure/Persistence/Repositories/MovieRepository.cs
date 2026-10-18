using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Movies;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MovieRepository(ApplicationDbContext context) : IMovieRepository, IMovieQueries
{
    public async Task<IReadOnlyList<Movie>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Movies.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Option<Movie>> GetById(MovieId id, CancellationToken cancellationToken)
    {
        var entity = await context.Movies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity ?? Option<Movie>.None;
    }

    public async Task<Option<Movie>> GetByTitle(string title, CancellationToken cancellationToken)
    {
        var entity = await context.Movies.FirstOrDefaultAsync(x => x.Title == title, cancellationToken);
        return entity ?? Option<Movie>.None;
    }

    public async Task<Movie> Add(Movie movie, CancellationToken cancellationToken)
    {
        await context.Movies.AddAsync(movie, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return movie;
    }

    public async Task<Movie> Update(Movie movie, CancellationToken cancellationToken)
    {
        context.Movies.Update(movie);
        await context.SaveChangesAsync(cancellationToken);
        return movie;
    }

    public async Task Delete(Movie movie, CancellationToken cancellationToken)
    {
        context.Movies.Remove(movie);
        await context.SaveChangesAsync(cancellationToken);
    }
}