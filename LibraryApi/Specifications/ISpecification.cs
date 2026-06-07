using System.Linq.Expressions;

namespace LibraryApi.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}