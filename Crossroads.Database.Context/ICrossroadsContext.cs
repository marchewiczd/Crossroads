using System.Linq.Expressions;
using Crossroads.Database.Entities.Abstractions;

namespace Crossroads.Database.Context;

public interface ICrossroadsContext
{
    public Task<T?> GetAsync<T>(object key) where T : TableBase;
    public Task<T?> GetAsync<T>(Expression<Func<T, bool>> expression) where T : TableBase;
    public T? Get<T>(object key) where T : TableBase;
    public T? Get<T>(Expression<Func<T, bool>> expression) where T : TableBase;
    public List<T>? GetAll<T>() where T : TableBase;
    public bool Delete<T>(object key) where T : TableBase;
    public Task<bool> DeleteAsync<T>(object key) where T : TableBase;
    public bool Update<T>(T record) where T : TableBase;
    public bool Insert<T>(T entity) where T : TableBase;
    public Task<bool> InsertAsync<T>(T entity) where T : TableBase;
}