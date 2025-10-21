using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Net;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("GitHub")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubClient _gitHubClient;
    private readonly ILogger<GitHubController> _logger;

    public GitHubController(IGitHubClient gitHubClient, ILogger<GitHubController> logger)
    {
        _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all personal repositories from GitHub
    /// </summary>
    /// <returns>List of personal GitHub repositories</returns>
    /// <response code="200">Returns the list of personal repositories</response>
    /// <response code="500">If there was an error fetching repositories</response>
    [HttpGet("repositories")]
    [ProducesResponseType(typeof(IEnumerable<GitHubRepository>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<GitHubRepository>>> GetPersonalRepositoriesAsync()
    {
        _logger.LogInformation($"Executing {nameof(GetPersonalRepositoriesAsync)}()");

        try
        {
            var repositories = await _gitHubClient.GetPersonalRepositoriesAsync();
            return Ok(repositories);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching GitHub repositories");
            return StatusCode((int)HttpStatusCode.InternalServerError, 
                new { message = "Failed to fetch repositories from GitHub", error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching GitHub repositories");
            return StatusCode((int)HttpStatusCode.InternalServerError, 
                new { message = "An unexpected error occurred", error = ex.Message });
        }
        finally
        {
            _logger.LogInformation($"Executed {nameof(GetPersonalRepositoriesAsync)}()");
        }
    }
}
