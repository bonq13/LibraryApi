using LibraryApi.Application.Repositories;
using LibraryApi.Application.Services;
using LibraryApi.Domain.Entities;
using Moq;

namespace LibraryApi.Tests.Services;

public class BookServiceTests
{
   private readonly Mock<IBookRepository> _mockRepository;
   private readonly BookService _bookService;
   
   public BookServiceTests()
    {
        _mockRepository = new Mock<IBookRepository>();
        _bookService = new BookService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllBooks_WhenCalled_ReturnsBookFromRepository()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", Author = "Author 1", IsAvailable = true},
            new Book { Id = 2, Title = "Book 2", Author = "Author 2", IsAvailable = true}
        };
        _mockRepository.Setup(r => r.GetAllBooks()).ReturnsAsync(books);
        
        // Act
        var result = await _bookService.GetAllBooks();
        
        // Assert
        Assert.Equal(2, result.Count);

    }
   
    [Fact]
    public async Task AddBook_WhenBookExists_ReturnsFalse()
    {
        // Arrange
        var book = new Book { Title = "Book 1", Author = "Author 1", IsAvailable = true };
        _mockRepository.Setup(r => r.GetAllBooks()).ReturnsAsync(new List<Book> { book });
        
        
        // Act
        var result = await _bookService.AddBook(book);
        
        // Assert
        Assert.False(result);
        _mockRepository.Verify(
            r => r.AddBook(It.IsAny<Book>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AddBook_WhenBookDoesNotExist_ReturnsTrue()
    {
        // Arrange
        var book = new Book { Title = "Book 1", Author = "Author 1", IsAvailable = true };
        _mockRepository.Setup(r => r.GetAllBooks()).ReturnsAsync(new List<Book>());
        _mockRepository.Setup(r => r.AddBook(It.IsAny<Book>()));
        
        
        // Act
        var result = await _bookService.AddBook(book);
        
        // Assert
        Assert.True(result);
        _mockRepository.Verify(
            r => r.AddBook(It.IsAny<Book>()),
            Times.Once
        );
    }
    
    [Fact]
    public async Task GetBookById_WhenBookExist_ReturnsBook()
    {
        //Arrange
        var book = new Book{Id = 1, Title = "clean code", Author = "Robert Martin", IsAvailable = true};
        _mockRepository.Setup(r => r.GetBookById(1)).ReturnsAsync(book);
	
        // Act
        var result = await _bookService.GetBookById(1);


        //Assert
        Assert.NotNull(result);
        Assert.Equal("clean code", result.Title);
    }

    [Fact]
    public async Task GetBookById_WhenBookDoesNotExist_ReturnsNull()
    {
        //Arrange
        _mockRepository.Setup(r => r.GetBookById(1)).ReturnsAsync((Book?)null);

        //Act
        var result = await _bookService.GetBookById(1);

        //Assert
        Assert.Null(result);
    }
}