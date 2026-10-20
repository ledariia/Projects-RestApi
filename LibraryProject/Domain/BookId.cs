using System;

namespace Domain;

public record BookId(Guid Value)
{
    public static BookId New() => new(Guid.NewGuid());
    public static BookId Empty() => new(Guid.Empty);
}