namespace PacoMediaTranscriber.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    // Example call
    public async Task<string> PingAsync()
    {
        return await _http.GetStringAsync("ping");
    }
}
