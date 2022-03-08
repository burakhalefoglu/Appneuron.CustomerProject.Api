using System.Linq.Expressions;
using Core.Entities;

namespace Core.DataAccess.Cassandra;

public interface ICassandraRepository<T> where T : class, IEntity
{
    Task UpdateFilterAsync(T record, Expression<Func<T, bool>> filter);
    void UpdateFilter(T record, Expression<Func<T, bool>> filter);
}