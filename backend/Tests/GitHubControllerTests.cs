using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.Interfaces;
using Services.Models;
using WebApi.Controllers;

namespace Tests;

[TestClass]
public class GitHubControllerTests
{
    private Mock<IGitHubClient> _mockGitHubClient = null!;
    private Mock<ILogger<GitHubController>> _mockLogger = null!;
    private GitHubController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockGitHubClient = new Mock<IGitHubClient>();
        _mockLogger = new Mock<ILogger<GitHubController>>();
        _controller = new GitHubController(_mockGitHubClient.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetPersonalRepositoriesAsync_ReturnsOkResult_WithRepositories()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository
            {
                Id = 1,
                Name = "test-repo-1",
                FullName = "user/test-repo-1",
                Description = "Test repository 1",
                IsPrivate = false,
                HtmlUrl = "https://github.com/user/test-repo-1",
                Language = "C#",
                StarsCount = 10,
                ForksCount = 5
            },
            new GitHubRepository
            {
                Id = 2,
                Name = "test-repo-2",
                FullName = "user/test-repo-2",
                Description = "Test repository 2",
                IsPrivate = true,
                HtmlUrl = "https://github.com/user/test-repo-2",
                Language = "TypeScript",
                StarsCount = 20,
                ForksCount = 3
            }
        };

        _mockGitHubClient
            .Setup(client => client.GetPersonalRepositoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositories);

        // Act
        var result = await _controller.GetPersonalRepositoriesAsync();

        // Assert
        Assert.IsNotNull(result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var returnedRepos = okResult.Value as IEnumerable<GitHubRepository>;
        Assert.IsNotNull(returnedRepos);
        Assert.AreEqual(2, returnedRepos.Count());
    }

    [TestMethod]
    public async Task GetPersonalRepositoriesAsync_ReturnsOkResult_WithEmptyList()
    {
        // Arrange
        var emptyRepositories = new List<GitHubRepository>();

        _mockGitHubClient
            .Setup(client => client.GetPersonalRepositoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyRepositories);

        // Act
        var result = await _controller.GetPersonalRepositoriesAsync();

        // Assert
        Assert.IsNotNull(result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var returnedRepos = okResult.Value as IEnumerable<GitHubRepository>;
        Assert.IsNotNull(returnedRepos);
        Assert.AreEqual(0, returnedRepos.Count());
    }

    [TestMethod]
    public async Task GetPersonalRepositoriesAsync_ReturnsInternalServerError_OnHttpRequestException()
    {
        // Arrange
        _mockGitHubClient
            .Setup(client => client.GetPersonalRepositoriesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("GitHub API unavailable"));

        // Act
        var result = await _controller.GetPersonalRepositoriesAsync();

        // Assert
        Assert.IsNotNull(result);
        var statusCodeResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }

    [TestMethod]
    public async Task GetPersonalRepositoriesAsync_ReturnsInternalServerError_OnGeneralException()
    {
        // Arrange
        _mockGitHubClient
            .Setup(client => client.GetPersonalRepositoriesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetPersonalRepositoriesAsync();

        // Assert
        Assert.IsNotNull(result);
        var statusCodeResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
}
