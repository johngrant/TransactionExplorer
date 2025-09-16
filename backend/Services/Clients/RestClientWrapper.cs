using RestSharp;
using Services.Interfaces;

namespace Services.Clients;

/// <summary>
/// Wrapper implementation for RestClient to enable mocking and testing
/// </summary>
public class RestClientWrapper : IRestClientWrapper
{
    private readonly RestClient _restClient;

    public RestClientWrapper(RestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    /// <inheritdoc />
    public async Task<IRestResponseWrapper> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
    {
        var restResponse = await _restClient.ExecuteAsync(request, cancellationToken);
        return new RestResponseWrapper(restResponse);
    }

    /// <inheritdoc />
    public string? BaseUrl => _restClient.Options.BaseUrl?.ToString();
}
