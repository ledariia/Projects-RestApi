using System;
using Domain;

namespace Application.Errors;

public abstract record BookError(BookId Id, string Message);

public record BookAlreadyExistsError(BookId Id, string Message) : BookError(Id, Message);
public record BookNotFoundError(BookId Id, string Message) : BookError(Id, Message);
public record BookUnhandledError(BookId Id, string Message, Exception Exception) : BookError(Id, Message);