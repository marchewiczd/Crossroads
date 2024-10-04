using Crossroads.Database.Entities.Abstractions;

namespace Crossroads.Database.Context;

public interface ICrossroadsContext
{
    public Task<T?> GetAsync<T>(object key) where T : TableBase;
    public T? Get<T>(object key) where T : TableBase;
    public List<T>? GetAll<T>() where T : TableBase;
    public bool Delete<T>(object key) where T : TableBase;
    public bool Update<T>(T record) where T : TableBase;
}