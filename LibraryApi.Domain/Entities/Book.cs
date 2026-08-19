namespace LibraryApi.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required bool IsAvailable { get; set; }
}
