using System.Linq.Expressions;
using LibraryApi.Domain.Entities;

namespace LibraryApi.Application.Specifications;

public class AvailableBooksSpecification : ISpecification<Book>
{
    public Expression<Func<Book, bool>> Criteria
        => book => book.IsAvailable == true;
}