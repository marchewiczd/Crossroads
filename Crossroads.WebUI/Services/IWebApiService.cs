namespace Crossroads.WebUI.Services;

public interface IWebApiService : IDisposable
{
    public Task<string> GetAsync(string resource);

    public Task<T> GetAsync<T>(string resource) where T : new();

    public Task<bool> PostAsync<T>(string resource, T data);

    public Task<bool> DeleteAsync(string resource, int id);
}