using Application.Common.Interfaces.Repositories;
using Application.Movies.Exceptions;
using Domain.Movies;
using LanguageExt;
using MediatR;

namespace Application.Movies.Commands;

public record DeleteMovieCommand : IRequest<Either<MovieException, Movie>>
{
    public required Guid MovieId { get; init; }
}

public class DeleteMovieCommandHandler(IMovieRepository movieRepository)
    : IRequestHandler<DeleteMovieCommand, Either<MovieException, Movie>>
{
    public async Task<Either<MovieException, Movie>> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movieId = new MovieId(request.MovieId);
        var movie = await movieRepository.GetById(movieId, cancellationToken);

        return await movie.MatchAsync(
            m => DeleteEntity(m, cancellationToken),
            () => new MovieNotFoundException(movieId));
    }

    private async Task<Either<MovieException, Movie>> DeleteEntity(Movie movie, CancellationToken cancellationToken)
    {
        try
        {
            await movieRepository.Delete(movie, cancellationToken);
            return movie;
        }
        catch (Exception exception)
        {
            return new MovieUnhandledException(movie.Id, exception);
        }
    }
}