using System.Net;
using RestSharp;
using Services.Interfaces;

namespace Services.Clients;

/// <summary>
/// Wrapper implementation for RestResponse to enable mocking and testing
/// </summary>
public class RestResponseWrapper : IRestResponseWrapper
{
    private readonly RestResponse _restResponse;

    public RestResponseWrapper(RestResponse restResponse)
    {
        _restResponse = restResponse ?? throw new ArgumentNullException(nameof(restResponse));
    }

    /// <inheritdoc />
    public HttpStatusCode StatusCode => _restResponse.StatusCode;

    /// <inheritdoc />
    public bool IsSuccessful => _restResponse.IsSuccessful;

    /// <inheritdoc />
    public string? Content => _restResponse.Content;

    /// <inheritdoc />
    public string? ErrorMessage => _restResponse.ErrorMessage;
}
