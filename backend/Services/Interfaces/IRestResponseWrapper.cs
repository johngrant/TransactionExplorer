using System.Net;

namespace Services.Interfaces;

/// <summary>
/// Wrapper interface for RestResponse to enable mocking and testing
/// </summary>
public interface IRestResponseWrapper
{
    /// <summary>
    /// HTTP status code of the response
    /// </summary>
    HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Whether the response was successful (status code 200-299)
    /// </summary>
    bool IsSuccessful { get; }

    /// <summary>
    /// Response content as string
    /// </summary>
    string? Content { get; }

    /// <summary>
    /// Error message if the request failed
    /// </summary>
    string? ErrorMessage { get; }
}
