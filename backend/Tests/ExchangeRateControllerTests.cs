using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Interfaces;
using Services.Models;
using WebApi.Controllers;
using WebApi.Models;

namespace Tests;

/// <summary>
/// Unit tests for the ExchangeRateController.
///
/// These tests verify that the controller properly uses the injected Treasury client dependency
/// and handles various scenarios correctly.
/// </summary>
[TestClass]
public class ExchangeRateControllerTests
{
    private ExchangeRateController _controller = null!;
    private Mock<ITreasuryExchangeRateClient> _mockTreasuryClient = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockTreasuryClient = new Mock<ITreasuryExchangeRateClient>();
        _controller = new ExchangeRateController(_mockTreasuryClient.Object);
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_ReturnsNotFound_WhenNoRatesFound()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var countryCurrencyDesc = "Canada-Dollar";
        var expectedStartDate = transactionDate.AddMonths(-6); // 6 months before transaction date

        var emptyResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>()
        };

        _mockTreasuryClient
            .Setup(client => client.GetExchangeRatesForCurrencyAsync(
                countryCurrencyDesc,
                expectedStartDate,
                transactionDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResponse);

        // Act
        var result = await _controller.GetExchangeRatesForPeriod(transactionDate, countryCurrencyDesc);

        // Assert
        _mockTreasuryClient.Verify(client => client.GetExchangeRatesForCurrencyAsync(
            countryCurrencyDesc,
            expectedStartDate,
            transactionDate,
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.AreEqual("No exchange rates found for the specified currency and date range.", notFoundResult!.Value);
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_ReturnsOkWithSortedRates_WhenRatesFound()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var countryCurrencyDesc = "Canada-Dollar";
        var expectedStartDate = transactionDate.AddMonths(-6);

        var treasuryResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new() { RecordDate = "2024-06-15", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "1.35" },
                new() { RecordDate = "2024-05-15", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "1.33" },
                new() { RecordDate = "2024-04-15", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "1.32" }
            }
        };

        _mockTreasuryClient
            .Setup(client => client.GetExchangeRatesForCurrencyAsync(
                countryCurrencyDesc,
                expectedStartDate,
                transactionDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(treasuryResponse);

        // Act
        var result = await _controller.GetExchangeRatesForPeriod(transactionDate, countryCurrencyDesc);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedResponse = okResult.Value as TreasuryApiResponse;
        Assert.IsNotNull(returnedResponse);
        Assert.AreEqual(3, returnedResponse.Data.Count);

        // Verify the rates are sorted by date (most recent first)
        Assert.AreEqual("2024-06-15", returnedResponse.Data[0].RecordDate);
        Assert.AreEqual("2024-05-15", returnedResponse.Data[1].RecordDate);
        Assert.AreEqual("2024-04-15", returnedResponse.Data[2].RecordDate);
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_ThrowsArgumentException_WhenCurrencyDescIsEmpty()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var emptyCurrencyDesc = "";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _controller.GetExchangeRatesForPeriod(transactionDate, emptyCurrencyDesc));
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_ThrowsArgumentException_WhenCurrencyDescIsNull()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        string nullCurrencyDesc = null!;

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _controller.GetExchangeRatesForPeriod(transactionDate, nullCurrencyDesc));
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_CalculatesCorrectDateRange_ForSixMonthsPeriod()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 12, 31);
        var countryCurrencyDesc = "Euro-Zone-Euro";
        var expectedStartDate = new DateOnly(2024, 6, 30); // 6 months before

        var treasuryResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>()
        };

        _mockTreasuryClient
            .Setup(client => client.GetExchangeRatesForCurrencyAsync(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(treasuryResponse);

        // Act
        await _controller.GetExchangeRatesForPeriod(transactionDate, countryCurrencyDesc);

        // Assert
        _mockTreasuryClient.Verify(client => client.GetExchangeRatesForCurrencyAsync(
            countryCurrencyDesc,
            expectedStartDate,
            transactionDate,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_ReturnsInternalServerError_WhenClientThrowsException()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var countryCurrencyDesc = "Canada-Dollar";
        var exceptionMessage = "External API is unavailable";

        _mockTreasuryClient
            .Setup(client => client.GetExchangeRatesForCurrencyAsync(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException(exceptionMessage));

        // Act
        var result = await _controller.GetExchangeRatesForPeriod(transactionDate, countryCurrencyDesc);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        var objectResult = result.Result as ObjectResult;
        Assert.AreEqual(500, objectResult!.StatusCode);
        Assert.IsTrue(objectResult.Value!.ToString()!.Contains(exceptionMessage));
    }

    [TestMethod]
    public async Task GetExchangeRatesForPeriod_ReturnsNotFound_WhenResponseDataIsNull()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var countryCurrencyDesc = "Canada-Dollar";

        var responseWithNullData = new TreasuryApiResponse
        {
            Data = null!
        };

        _mockTreasuryClient
            .Setup(client => client.GetExchangeRatesForCurrencyAsync(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseWithNullData);

        // Act
        var result = await _controller.GetExchangeRatesForPeriod(transactionDate, countryCurrencyDesc);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetLatestExchangeRate_ReturnsOkWithLatestRate_WhenRateFound()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var countryCurrencyDesc = "Canada-Dollar";

        var treasuryResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new() { RecordDate = "2024-06-10", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "1.35" }
            }
        };

        _mockTreasuryClient
            .Setup(client => client.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(treasuryResponse);

        // Act
        var result = await _controller.GetLatestExchangeRate(transactionDate, countryCurrencyDesc);

        // Assert
        _mockTreasuryClient.Verify(client => client.GetLatestExchangeRateAsync(
            countryCurrencyDesc,
            transactionDate,
            It.IsAny<CancellationToken>()), Times.Once);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedResponse = okResult.Value as TreasuryApiResponse;
        Assert.IsNotNull(returnedResponse);
        Assert.AreEqual(1, returnedResponse.Data.Count);
        Assert.AreEqual("1.35", returnedResponse.Data[0].ExchangeRate);
    }

    [TestMethod]
    public async Task GetLatestExchangeRate_ReturnsNotFound_WhenNoRateFound()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var countryCurrencyDesc = "Canada-Dollar";

        var emptyResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>()
        };

        _mockTreasuryClient
            .Setup(client => client.GetLatestExchangeRateAsync(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResponse);

        // Act
        var result = await _controller.GetLatestExchangeRate(transactionDate, countryCurrencyDesc);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.AreEqual("No exchange rate found for the specified currency and date.", notFoundResult!.Value);
    }

    [TestMethod]
    public async Task GetLatestExchangeRate_ThrowsArgumentException_WhenCurrencyDescIsEmpty()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var emptyCurrencyDesc = "";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _controller.GetLatestExchangeRate(transactionDate, emptyCurrencyDesc));
    }

    [TestMethod]
    public async Task Convert_ReturnsOkWithConvertedAmount_WhenValidRateFound()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var amountUsd = 100.50m;
        var countryCurrencyDesc = "Canada-Dollar";

        var treasuryResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new() { RecordDate = "2024-06-10", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "1.35" }
            }
        };

        _mockTreasuryClient
            .Setup(client => client.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(treasuryResponse);

        // Act
        var result = await _controller.Convert(transactionDate, amountUsd, countryCurrencyDesc);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var conversionResult = okResult.Value as CurrencyConversionResponse;
        Assert.IsNotNull(conversionResult);

        Assert.AreEqual(100.50m, conversionResult.OriginalAmountUsd);
        Assert.AreEqual(135.68m, conversionResult.ConvertedAmount); // 100.50 * 1.35 = 135.675, rounded to 135.68
        Assert.AreEqual(1.35m, conversionResult.ExchangeRate);
        Assert.AreEqual("Canada-Dollar", conversionResult.TargetCurrency);
        Assert.AreEqual(transactionDate, conversionResult.TransactionDate);
        Assert.AreEqual(new DateOnly(2024, 6, 10), conversionResult.ExchangeRateDate);
        Assert.IsFalse(conversionResult.IsExactDateMatch); // Rate date is different from transaction date
    }

    [TestMethod]
    public async Task Convert_ReturnsNotFound_WhenNoExchangeRateFound()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var amountUsd = 100.00m;
        var countryCurrencyDesc = "Canada-Dollar";

        var emptyResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>()
        };

        _mockTreasuryClient
            .Setup(client => client.GetLatestExchangeRateAsync(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResponse);

        // Act
        var result = await _controller.Convert(transactionDate, amountUsd, countryCurrencyDesc);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        var notFoundResult = result.Result as NotFoundObjectResult;
        Assert.AreEqual("The purchase cannot be converted to the target currency - no exchange rate found within 6 months.", notFoundResult!.Value);
    }

    [TestMethod]
    public async Task Convert_ThrowsArgumentException_WhenAmountIsNegative()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var negativeAmount = -10.00m;
        var countryCurrencyDesc = "Canada-Dollar";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _controller.Convert(transactionDate, negativeAmount, countryCurrencyDesc));
    }

    [TestMethod]
    public async Task Convert_ThrowsArgumentException_WhenAmountIsZero()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var zeroAmount = 0.00m;
        var countryCurrencyDesc = "Canada-Dollar";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _controller.Convert(transactionDate, zeroAmount, countryCurrencyDesc));
    }

    [TestMethod]
    public async Task Convert_ThrowsArgumentException_WhenCurrencyDescIsEmpty()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var amountUsd = 100.00m;
        var emptyCurrencyDesc = "";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _controller.Convert(transactionDate, amountUsd, emptyCurrencyDesc));
    }

    [TestMethod]
    public async Task Convert_RoundsToTwoDecimalPlaces_ForComplexCalculation()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var amountUsd = 123.456m; // This will test rounding
        var countryCurrencyDesc = "Canada-Dollar";

        var treasuryResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new() { RecordDate = "2024-06-10", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "1.234567" }
            }
        };

        _mockTreasuryClient
            .Setup(client => client.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(treasuryResponse);

        // Act
        var result = await _controller.Convert(transactionDate, amountUsd, countryCurrencyDesc);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var conversionResult = okResult.Value as CurrencyConversionResponse;
        Assert.IsNotNull(conversionResult);

        // 123.456 * 1.234567 with decimal precision yields 152.41 when rounded to 2 places
        Assert.AreEqual(152.41m, conversionResult.ConvertedAmount);
        Assert.AreEqual(123.456m, conversionResult.OriginalAmountUsd);
        Assert.AreEqual(1.234567m, conversionResult.ExchangeRate);
    }

    [TestMethod]
    public async Task Convert_ReturnsInternalServerError_WhenExchangeRateFormatIsInvalid()
    {
        // Arrange
        var transactionDate = new DateOnly(2024, 6, 15);
        var amountUsd = 100.00m;
        var countryCurrencyDesc = "Canada-Dollar";

        var treasuryResponse = new TreasuryApiResponse
        {
            Data = new List<TreasuryExchangeRateData>
            {
                new() { RecordDate = "2024-06-10", CountryCurrencyDesc = "Canada-Dollar", ExchangeRate = "invalid-rate" }
            }
        };

        _mockTreasuryClient
            .Setup(client => client.GetLatestExchangeRateAsync(
                countryCurrencyDesc,
                transactionDate,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(treasuryResponse);

        // Act
        var result = await _controller.Convert(transactionDate, amountUsd, countryCurrencyDesc);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        var objectResult = result.Result as ObjectResult;
        Assert.AreEqual(500, objectResult!.StatusCode);
        Assert.IsTrue(objectResult.Value!.ToString()!.Contains("Invalid exchange rate format"));
    }
}
