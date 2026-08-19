using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using FluentValidation;
using LibraryApi.Application.Commands;
using LibraryApi.Application.Queries;
using LibraryApi.Application.Repositories;
using LibraryApi.Application.Services;
using LibraryApi.Application.Specifications;
using LibraryApi.Domain.Entities;
using LibraryApi.Infrastructure.Data;
using LibraryApi.Infrastructure.Repositories;
using LibraryApi.Models;
using LibraryApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AddBookCommand).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<AddBookCommandValidator>();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; 
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = "role" 
        };
    });


builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();




var v1 = app.MapGroup("/v1/books")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(1, 0))
    .WithTags("Books")
    .RequireRateLimiting("fixed");


var v2 = app.MapGroup("/v2/books")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(new ApiVersion(2, 0))
    .WithTags("Books");

app.MapPost("/login", async (LoginRequest request, IJwtService jwtService) =>
{
    const string testEmail = "admin@library.com";
    const string testPassword = "Admin123!";
    const string testRole = "Admin";

    if (request.Email == testEmail && request.Password == testPassword)
    {
        return Results.Ok(new { token = jwtService.GenerateToken("1", testEmail, testRole) });
    }

    return Results.Unauthorized();
});

v1.MapGet("/",  async (IMediator mediator) =>
    {
        var books = await mediator.Send(new GetAllBooksQuery());
        return Results.Ok(books);
    })
    .WithSummary("Pobierz wszystkie ksiązki")
    .Produces<List<Book>>(StatusCodes.Status200OK);

v2.MapGet("/", async (IMediator mediator) => {
    var books = await mediator.Send(new GetAllBooksQuery());
    return Results.Ok(new { books, apiVersion = "v2" });
});

v1.MapGet("/available", async (IBookRepository repository) =>
    {
        var books = await repository.GetBySpecification(new AvailableBooksSpecification());
        return Results.Ok(books);
    })
    .WithSummary("Pobierz dostępne ksiązki")
    .Produces<List<Book>>(StatusCodes.Status200OK);

v1.MapGet("/{id}", async (int id, IMediator mediator) =>
{
    var book = await mediator.Send(new GetBookByIdQuery(id));
    return book != null ? Results.Ok(book) : Results.Problem(
        detail: $"Ksiązka o id {id} nie istnieje",
        statusCode: StatusCodes.Status404NotFound,
        title: "Nie znaleziono ksiązki");
})
.WithSummary("Pobierz ksiązke o podanym id")
.Produces<Book>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);;

v1.MapPost("/", async  (AddBookCommand request,
        IValidator<AddBookCommand> validator,
        IMediator mediator) =>
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var result = await mediator.Send(request);
        return result is not null
            ? Results.Created($"/v1/books/{result.Id}", result)
            : Results.Conflict();
    })
    .RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithSummary("Dodaj nową ksiązkę")
.Produces<AddBookCommand>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status409Conflict)
.ProducesValidationProblem();

app.MapGet("/me", async (HttpContext context) =>
{
    var userId = context.User.FindFirst("sub")?.Value;
    var email = context.User.FindFirst("email")?.Value;
    var role = context.User.FindFirst("role")?.Value;

    return Results.Ok(new { userId, email, role });
}).RequireAuthorization();




app.Run();