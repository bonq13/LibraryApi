using System.Linq.Expressions;
using LibraryApi.Models;

namespace LibraryApi.Specifications;

public class AvailableBooksSpecification : ISpecification<Book>
{
    public Expression<Func<Book, bool>> Criteria
        => book => book.IsAvailable == true;
}