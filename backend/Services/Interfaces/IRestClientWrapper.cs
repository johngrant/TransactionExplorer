using RestSharp;

namespace Services.Interfaces;

/// <summary>
/// Wrapper interface for RestClient to enable mocking and testing
/// </summary>
public interface IRestClientWrapper
{
    /// <summary>
    /// Execute an async REST request
    /// </summary>
    /// <param name="request">The request to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response wrapper</returns>
    Task<IRestResponseWrapper> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the base URL of the REST client
    /// </summary>
    string? BaseUrl { get; }
}
