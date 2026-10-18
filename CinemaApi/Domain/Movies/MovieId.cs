namespace Domain.Movies;

public record MovieId(Guid Value)
{
    public static MovieId Empty() => new(Guid.Empty);
    public static MovieId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}