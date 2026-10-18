namespace Domain.Movies;

public class Movie
{
    public MovieId Id { get; }
    public string Title { get; private set; }
    public DateTime CreatedAt { get; }
    public int DurationMinutes { get; private set; }
    public DateTime ReleaseDate { get; private set; }
    public AgeRating AgeRating { get; private set; }
    
    private Movie(MovieId id, string title, DateTime createdAt, int durationMinutes, DateTime releaseDate, AgeRating ageRating)
        => (Id, Title, CreatedAt, DurationMinutes, ReleaseDate, AgeRating)
            = (id, title, createdAt, durationMinutes, releaseDate, ageRating);
    public static Movie New(MovieId id, string title, int durationMinutes, DateTime releaseDate, AgeRating ageRating)
        => new(id, title, DateTime.UtcNow, durationMinutes, releaseDate, ageRating);

    public void UpdateDetails(string title, int durationMinutes, DateTime releaseDate, AgeRating ageRating)
        => (Title, DurationMinutes, ReleaseDate, AgeRating)
            = (title, durationMinutes, releaseDate, ageRating);
}