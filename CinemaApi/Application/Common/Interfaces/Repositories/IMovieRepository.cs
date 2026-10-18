using Domain.Movies;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IMovieRepository
{
    Task<Option<Movie>> GetById(MovieId id, CancellationToken cancellationToken);
    Task<Option<Movie>> GetByTitle(string title, CancellationToken cancellationToken);
    Task<Movie> Add(Movie movie, CancellationToken cancellationToken);
    Task<Movie> Update(Movie movie, CancellationToken cancellationToken);
    Task Delete(Movie movie, CancellationToken cancellationToken);
}