using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LibraryApi.Domain.Entities;
using LibraryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryApi.Tests.Books;

public class BookEndpointsTests : IClassFixture<LibraryApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly AppDbContext _dbContext;
    private readonly IServiceScope _scope;
    
    public BookEndpointsTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
    
    public async Task InitializeAsync()
    {
        await _dbContext.Books.ExecuteDeleteAsync();

        var loginRequest = new { Email = "admin@library.com", Password = "Admin123!" };
        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _client.PostAsync("/login", loginContent);
        var loginBody = await loginResponse.Content.ReadAsStringAsync();

        var token = JsonDocument.Parse(loginBody).RootElement.GetProperty("token").GetString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
  
    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllBooks_ReturnsOkStatusCode()
    {
        //Act
        var response = await _client.GetAsync("/v1/books");
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddBook_WithValidData_ReturnsCreated()
    {
        //Arrange
        var newBook = new { Title = "Clean Code", Author = "Robert Martin", IsAvailable = true };
        var content = new StringContent(
            JsonSerializer.Serialize(newBook),
            Encoding.UTF8,
            "application/json");
        //Act
        var response = await _client.PostAsync("/v1/books", content);
        
        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }


    [Fact]
    public async Task AddBook_WithDuplicateTitle_ReturnsConflict()
    {
        //Arrange
        var book = new { Title = "Duplicate Book", Author = "Author", IsAvailable = true };
        var content = new StringContent(
            JsonSerializer.Serialize(book),
            Encoding.UTF8,
            "application/json");
        
        //Act
        await _client.PostAsync("/v1/books", content);
        var response = await _client.PostAsync("/v1/books", content);
        
        //Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetBookById_WhenBookExists_ReturnsOkAndBook()
    {
        //Arrange
        var book = new { Title = "Test Book", Author = "Author1", IsAvailable = true };
        var content = new StringContent(
            JsonSerializer.Serialize(book),
            Encoding.UTF8,
            "application/json");
        
        //Act
        var postResponse = await _client.PostAsync("/v1/books", content);
        var postBody = await postResponse.Content.ReadAsStringAsync();
        var createdBook = JsonSerializer.Deserialize<Book>(postBody, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        var response = await _client.GetAsync($"/v1/books/{createdBook!.Id}");
        
        var body = await response.Content.ReadAsStringAsync();
        var bookFromResponse = JsonSerializer.Deserialize<Book>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Test Book", bookFromResponse!.Title);
    }

    [Fact]
    public async Task GetBooks_WhenBooksExist_ReturnBooks()
    {
        //Arrange
        var book1 = new Book { Title = "Book 1", Author = "Author 1", IsAvailable = true };
        var book2 = new Book { Title = "Book 2", Author = "Author 2", IsAvailable = true };
        
        var content1 = new StringContent(JsonSerializer.Serialize(book1), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(book2), Encoding.UTF8, "application/json");
        
        //Act
        await _client.PostAsync("/v1/books", content1);
        await _client.PostAsync("/v1/books", content2);
        
        var response = await _client.GetAsync("/v1/books");
        
        var body = await response.Content.ReadAsStringAsync();
        var books = JsonSerializer.Deserialize<List<Book>>(body, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, books!.Count);
        
    }
    
    [Fact]
    public async Task GetBookById_WhenBookDoesNotExist_ReturnsNotFound()
    {
        //Act
        var response = await _client.GetAsync("/v1/books/1");
        
        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task AddBook_WhenDataIsInvalid_ReturnsBadRequest()
    {
        //Arrange
        var invalidBook = new { Title = "", Author = "Author", IsAvailable = true };
        var content = new StringContent(
            JsonSerializer.Serialize(invalidBook),
            Encoding.UTF8,
            "application/json");
        
        //Act
        var response = await _client.PostAsync("/v1/books", content);
        
        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
}