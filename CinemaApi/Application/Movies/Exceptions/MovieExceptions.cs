using Domain.Movies;

namespace Application.Movies.Exceptions;

public abstract class MovieException(MovieId movieId, string message, Exception? innerException = null) 
    : Exception(message, innerException)
{
    public MovieId MovieId { get; } = movieId;
}

public class MovieAlreadyExistsException(MovieId movieId) 
    : MovieException(movieId, $"Movie already exists under id {movieId}");

public class MovieNotFoundException(MovieId movieId) 
    : MovieException(movieId, $"Movie not found under id {movieId}");

public class MovieUnhandledException(MovieId movieId, Exception? innerException = null) 
    : MovieException(movieId, $"Unhandled exception under id {movieId}", innerException);