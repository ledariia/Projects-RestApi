using Domain.Movies;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IMovieQueries
{
    Task<IReadOnlyList<Movie>> GetAll(CancellationToken cancellationToken);
    Task<Option<Movie>> GetById(MovieId id, CancellationToken cancellationToken);
}