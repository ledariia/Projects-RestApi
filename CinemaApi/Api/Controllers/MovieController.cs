using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Movies.Commands;
using Domain.Movies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("movies")]
[ApiController]
public class MoviesController(
    ISender sender,
    IMovieQueries movieQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MovieDto>>> GetMovies(CancellationToken cancellationToken)
    {
        var movies = await movieQueries.GetAll(cancellationToken);
        return Ok(movies.Select(MovieDto.FromDomainModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MovieDto>> GetMovie(Guid id, CancellationToken cancellationToken)
    {
        var movie = await movieQueries.GetById(new MovieId(id), cancellationToken);
        
        return movie.Match<ActionResult<MovieDto>>(
            m => Ok(MovieDto.FromDomainModel(m)),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie(
        [FromBody] CreateMovieDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateMovieCommand
        {
            Title = request.Title,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            AgeRating = request.AgeRating
        };

        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<MovieDto>>(
            m => CreatedAtAction(nameof(GetMovie), new { id = m.Id.Value }, MovieDto.FromDomainModel(m)),
            e => e.ToObjectResult());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MovieDto>> UpdateMovie(
        Guid id, 
        [FromBody] UpdateMovieDto request, 
        CancellationToken cancellationToken)
    {
        var input = new UpdateMovieCommand
        {
            MovieId = id,
            Title = request.Title,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            AgeRating = request.AgeRating
        };

        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<MovieDto>>(
            m => Ok(MovieDto.FromDomainModel(m)),
            e => e.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMovie(Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteMovieCommand { MovieId = id };
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<IActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}