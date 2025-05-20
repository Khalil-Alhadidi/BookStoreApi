using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookStoreApi.Features.Books;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;

namespace BookStoreApi.Features.Books;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/books")
            .WithTags("Books")
            .RequireRateLimiting("Books");

        group.MapGet("/", async (IBookService bookService) =>
        {
            var books = await bookService.GetAllBooksAsync();
            return Results.Ok(books);
        });

        group.MapGet("/{id}", async (int id, IBookService bookService) =>
        {
            var book = await bookService.GetBookByIdAsync(id);
            if (book == null)
                return Results.NotFound(new { message = $"Book with ID {id} not found" });
            return Results.Ok(book);
        });

        group.MapGet("/search", async (string title, IBookService bookService) =>
        {
            var books = await bookService.FindBooksByTitleAsync(title);
            return Results.Ok(books);
        });

        group.MapPost("/", [Authorize] async (
            [FromBody] Book book,
            IBookService bookService,
            IValidator<Book> validator) =>
        {
            var validationResult = await validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var createdBook = await bookService.CreateBookAsync(book);
            return Results.Created($"/api/books/{createdBook.Id}", createdBook);
        });

        group.MapPut("/{id}", [Authorize] async (
            int id,
            [FromBody] Book book,
            IBookService bookService,
            IValidator<Book> validator) =>
        {
            if (id != book.Id)
                return Results.BadRequest(new { message = "ID mismatch" });

            var validationResult = await validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var updatedBook = await bookService.UpdateBookAsync(book);
            if (updatedBook == null)
                return Results.NotFound(new { message = $"Book with ID {id} not found" });

            return Results.Ok(updatedBook);
        });

        group.MapDelete("/{id}", [Authorize] async (
            int id,
            IBookService bookService) =>
        {
            var result = await bookService.DeleteBookAsync(id);
            if (!result)
                return Results.NotFound(new { message = $"Book with ID {id} not found" });

            return Results.NoContent();
        });
    }
} 