using Application.Common.Interfaces.Repositories;
using Application.Movies.Exceptions;
using Domain.Movies;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Movies.Commands;

public record UpdateMovieCommand : IRequest<Either<MovieException, Movie>>
{
    public required Guid MovieId { get; init; }
    public required string Title { get; init; }
    public required int DurationMinutes { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required AgeRating AgeRating { get; init; }
}

public class UpdateMovieCommandHandler(IMovieRepository movieRepository)
    : IRequestHandler<UpdateMovieCommand, Either<MovieException, Movie>>
{
    public async Task<Either<MovieException, Movie>> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var movieId = new MovieId(request.MovieId);
        var movie = await movieRepository.GetById(movieId, cancellationToken);

        return await movie.MatchAsync(
            m => CheckDuplicates(m.Id, request.Title, cancellationToken)
                .BindAsync(_ => UpdateEntity(m, request, cancellationToken)),
            () => new MovieNotFoundException(movieId));
    }

    private async Task<Either<MovieException, Unit>> CheckDuplicates(MovieId currentMovieId, string title, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByTitle(title, cancellationToken);
        return movie.Match<Either<MovieException, Unit>>(
            m => m.Id == currentMovieId ? Unit.Default : new MovieAlreadyExistsException(m.Id),
            () => Unit.Default);
    }

    private async Task<Either<MovieException, Movie>> UpdateEntity(Movie movie, UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        try
        {
            movie.UpdateDetails(request.Title, request.DurationMinutes, request.ReleaseDate, request.AgeRating);
            return await movieRepository.Update(movie, cancellationToken);
        }
        catch (Exception exception)
        {
            return new MovieUnhandledException(movie.Id, exception);
        }
    }
}