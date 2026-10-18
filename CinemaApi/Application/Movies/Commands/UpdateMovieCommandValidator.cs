using FluentValidation;

namespace Application.Movies.Commands;

public class UpdateMovieCommandValidator : AbstractValidator<UpdateMovieCommand>
{
    public UpdateMovieCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(1).MaximumLength(255);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(1, 1000);
        RuleFor(x => x.ReleaseDate).NotEmpty();
        RuleFor(x => x.AgeRating).IsInEnum();
    }
}