using System.Net.Http.Json;
using Crossroads.Utils.Helpers.Enums;
using Environment = Crossroads.Utils.Helpers.Environment;

namespace Crossroads.Utils.Services;

public class WebApiService : IDisposable
{
    private readonly HttpClient _httpClient;
    private Uri? _baseUri;
    
    public WebApiService()
    {
        IsDisposed = false;
        
        _httpClient = new HttpClient();
        IsActive = SetBaseUri();
    }
    
    public bool IsActive { get; private set; }
    public bool IsDisposed { get; private set; }

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

    private bool SetBaseUri()
    {
        var useHttps = 
            Environment.GetVariable(Variable.ApiHttps).Equals("true", StringComparison.InvariantCultureIgnoreCase);
        var protocol = useHttps ? "https" : "http";
        var serviceName = Environment.GetVariable(Variable.ApiServiceName);
        var servicePort = Environment.GetVariable(Variable.ApiPort);
        
        return Uri.TryCreate($"{protocol}://{serviceName}:{servicePort}/", UriKind.Absolute, out _baseUri);
    }

    public void Dispose()
    {
        IsActive = false;
        _httpClient.Dispose();
        IsDisposed = true;
    }
}