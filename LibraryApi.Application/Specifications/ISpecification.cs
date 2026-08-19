using System.Linq.Expressions;

namespace LibraryApi.Application.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}