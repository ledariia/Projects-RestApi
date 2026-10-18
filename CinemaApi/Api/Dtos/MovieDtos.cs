using Domain.Movies;

namespace Api.Dtos;

public record MovieDto(Guid Id, string Title, int DurationMinutes, DateTime ReleaseDate, string AgeRating, DateTime CreatedAt)
{
    public static MovieDto FromDomainModel(Movie movie)
        => new(movie.Id.Value, movie.Title, movie.DurationMinutes, movie.ReleaseDate, movie.AgeRating.ToString(), movie.CreatedAt);
}
public record CreateMovieDto(string Title, int DurationMinutes, DateTime ReleaseDate, AgeRating AgeRating);

public record UpdateMovieDto(string Title, int DurationMinutes, DateTime ReleaseDate, AgeRating AgeRating);