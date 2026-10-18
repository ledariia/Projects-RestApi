using Application.Common.Interfaces.Repositories;
using Application.Movies.Exceptions;
using Domain.Movies;
using LanguageExt;
using MediatR;

namespace Application.Movies.Commands;

public record CreateMovieCommand : IRequest<Either<MovieException, Movie>>
{
    public required string Title { get; init; }
    public required int DurationMinutes { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required AgeRating AgeRating { get; init; }
}

public class CreateMovieCommandHandler(IMovieRepository movieRepository)
    : IRequestHandler<CreateMovieCommand, Either<MovieException, Movie>>
{
    public async Task<Either<MovieException, Movie>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var existingMovie = await movieRepository.GetByTitle(request.Title, cancellationToken);
        
        return await existingMovie.MatchAsync(
            m => new MovieAlreadyExistsException(m.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<MovieException, Movie>> CreateEntity(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var movie = Movie.New(MovieId.New(), request.Title, request.DurationMinutes, request.ReleaseDate, request.AgeRating);
            return await movieRepository.Add(movie, cancellationToken);
        }
        catch (Exception exception)
        {
            return new MovieUnhandledException(MovieId.Empty(), exception);
        }
    }
}