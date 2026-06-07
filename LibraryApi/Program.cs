using LibraryApi.Commands;
using LibraryApi.Data;
using LibraryApi.Queries;
using LibraryApi.Repositories;
using LibraryApi.Services;
using LibraryApi.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=library.db"));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/books", async (IMediator mediator) =>
{
    var books = await mediator.Send(new GetAllBooksQuery());
    return Results.Ok(books);
});

app.MapGet("/books/{id}", async (int id, IMediator mediator) =>
{
    var book = await mediator.Send(new GetBookByIdQuery(id));
    return book != null ? Results.Ok(book) : Results.NotFound();
});

app.MapGet("/books/available", async (IBookRepository repository) =>
{
    var books = await repository.GetBySpecification(new AvailableBooksSpecification());
    return Results.Ok(books);
});

app.MapPost("/books", async (AddBookCommand request, IMediator mediator) =>
{
    var result = await mediator.Send(request);
    return result ? Results.Created("/books", request) : Results.Conflict();
});



app.Run();