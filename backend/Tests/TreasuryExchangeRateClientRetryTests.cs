using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using Services.Clients;
using Services.Configuration;
using Services.Interfaces;
using Services.Models;

namespace Tests;

[TestClass]
public class TreasuryExchangeRateClientRetryTests
{
    private Mock<IRestClientWrapper> _mockRestClientWrapper = null!;
    private Mock<ILogger<TreasuryExchangeRateClient>> _mockLogger = null!;
    private IOptions<TreasuryExchangeRateOptions> _options = null!;
    private TreasuryExchangeRateClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRestClientWrapper = new Mock<IRestClientWrapper>();
        _mockLogger = new Mock<ILogger<TreasuryExchangeRateClient>>();
        _options = Options.Create(new TreasuryExchangeRateOptions
        {
            BaseUrl = "https://api.fiscaldata.treasury.gov",
            TimeoutSeconds = 30
        });

        _mockRestClientWrapper.Setup(x => x.BaseUrl).Returns("https://api.fiscaldata.treasury.gov");

        _client = new TreasuryExchangeRateClient(_mockRestClientWrapper.Object, _options, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetExchangeRatesAsync_WithPollyRetry_RetriesOnServerError()
    {
        // Arrange
        var failedResponse = Mock.Of<IRestResponseWrapper>(r => 
            r.StatusCode == HttpStatusCode.InternalServerError &&
            r.IsSuccessful == false &&
            r.ErrorMessage == "Server error");

        var successResponse = Mock.Of<IRestResponseWrapper>(r => 
            r.StatusCode == HttpStatusCode.OK &&
            r.IsSuccessful == true &&
            r.Content == "{\"data\":[]}");

        _mockRestClientWrapper.SetupSequence(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResponse)
            .ReturnsAsync(successResponse);

        // Act
        var result = await _client.GetExchangeRatesAsync();

        // Assert
        Assert.IsNotNull(result);
        _mockRestClientWrapper.Verify(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task GetExchangeRatesAsync_Returns404_DoesNotRetry()
    {
        // Arrange
        var notFoundResponse = Mock.Of<IRestResponseWrapper>(r => 
            r.StatusCode == HttpStatusCode.NotFound &&
            r.IsSuccessful == false);

        _mockRestClientWrapper.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notFoundResponse);

        // Act
        var result = await _client.GetExchangeRatesAsync();

        // Assert
        Assert.IsNotNull(result);
        // Should not retry on 404
        _mockRestClientWrapper.Verify(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task GetExchangeRatesAsync_RetriesOnTimeout()
    {
        // Arrange
        var timeoutResponse = Mock.Of<IRestResponseWrapper>(r => 
            r.StatusCode == HttpStatusCode.RequestTimeout &&
            r.IsSuccessful == false &&
            r.ErrorMessage == "Request timeout");

        var successResponse = Mock.Of<IRestResponseWrapper>(r => 
            r.StatusCode == HttpStatusCode.OK &&
            r.IsSuccessful == true &&
            r.Content == "{\"data\":[{\"record_date\":\"2024-01-01\",\"exchange_rate\":\"1.25\"}]}");

        _mockRestClientWrapper.SetupSequence(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeoutResponse)
            .ReturnsAsync(timeoutResponse)
            .ReturnsAsync(successResponse);

        // Act
        var result = await _client.GetExchangeRatesAsync();

        // Assert
        Assert.IsNotNull(result);
        // Should retry 2 times and succeed on 3rd attempt
        _mockRestClientWrapper.Verify(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [TestMethod]
    public async Task GetExchangeRatesAsync_BadRequest_DoesNotRetry()
    {
        // Arrange
        var badRequestResponse = Mock.Of<IRestResponseWrapper>(r => 
            r.StatusCode == HttpStatusCode.BadRequest &&
            r.IsSuccessful == false &&
            r.ErrorMessage == "Bad request");

        _mockRestClientWrapper.Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(badRequestResponse);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
        {
            await _client.GetExchangeRatesAsync();
        });

        // Should not retry on 400 Bad Request
        _mockRestClientWrapper.Verify(x => x.ExecuteAsync(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
