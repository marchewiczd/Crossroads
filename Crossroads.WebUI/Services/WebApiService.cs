using Crossroads.Utils.Helpers.Enums;
using Environment = Crossroads.Utils.Helpers.Environment;

namespace Crossroads.WebUI.Services;

public class WebApiService(ILogger<WebApiService> logger) : IWebApiService
{
    private readonly HttpClient _httpClient = new();
    private readonly Uri? _baseUri = Environment.GetUri();

    public async Task<string> GetAsync(string resource)
    {
        var response = await _httpClient.GetAsync($"{_baseUri}{resource}");
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<T> GetAsync<T>(string resource) where T : new()
    {
        var response = await _httpClient.GetAsync($"{_baseUri}{resource}");
        return await response.Content.ReadFromJsonAsync<T>() ?? new T();
    }

    public async Task PostAsync<T>(string resource, T data)
    {
        await _httpClient.PostAsJsonAsync($"{_baseUri}{resource}", data);
    }

    public async Task<bool> DeleteAsync(string resource, int id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUri}{resource}?id={id}");
        return await response.Content.ReadFromJsonAsync<bool>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }
}