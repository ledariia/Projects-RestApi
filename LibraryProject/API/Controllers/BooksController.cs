﻿﻿using Api.Contracts;
using Api.Extensions;
using Application.Commands.CreateBook;
using Application.Commands.UpdateBook;
using Application.Interfaces;
using Domain;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IBookQueries _queries;

    public BooksController(ISender sender, IBookQueries queries)
    {
        _sender = sender;
        _queries = queries;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var books = await _queries.GetAllAsync(cancellationToken);
        return Ok(books.Select(BookResponse.FromDomain));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var bookOption = await _queries.GetByIdAsync(id, cancellationToken);
        return bookOption.Match<IActionResult>(
            Some: book => Ok(BookResponse.FromDomain(book)),
            None: () => NotFound(new { Error = $"Книгу з ID {id} не знайдено." })
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(request.Title, request.Author, request.Genre, request.PageCount);
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            Right: book => CreatedAtAction(nameof(GetById), new { id = book.Id.Value }, BookResponse.FromDomain(book)),
            Left: error => error.ToActionResult()
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(id, request.Title, request.Author, request.Genre, request.PageCount);
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            Right: book => Ok(BookResponse.FromDomain(book)),
            Left: error => error.ToActionResult()
        );
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new Application.Commands.DeleteBook.DeleteBookCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            Right: _ => NoContent(),
            Left: error => error.ToActionResult()
        );
    }
    [HttpPost("filter")]
    public async Task<IActionResult> Filter([FromBody] SearchBookDto request, CancellationToken cancellationToken)
    {
        var books = await _queries.FilterAsync(
            request.Title,
            request.Author,
            request.Genre,
            request.PagesCountFrom,
            request.PagesCountTo,
            request.CreatedAtFrom,
            request.CreatedAtTo,
            cancellationToken);

        return Ok(books.Select(BookResponse.FromDomain));
    }
}