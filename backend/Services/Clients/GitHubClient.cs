using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Configuration;
using Services.Interfaces;
using Services.Models;

namespace Services.Clients;

/// <summary>
/// Client for GitHub API
/// </summary>
public class GitHubClient : IGitHubClient
{
    private readonly HttpClient _httpClient;
    private readonly GitHubOptions _options;
    private readonly ILogger<GitHubClient> _logger;

    public GitHubClient(
        HttpClient httpClient,
        IOptions<GitHubOptions> options,
        ILogger<GitHubClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.UserAgent);

        // Add authentication if token is provided
        if (!string.IsNullOrEmpty(_options.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        }
    }

    /// <summary>
    /// Retrieves personal repositories for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of GitHub repository information</returns>
    public async Task<IEnumerable<GitHubRepository>> GetPersonalRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching personal repositories from GitHub");

        try
        {
            var response = await _httpClient.GetAsync("/user/repos?type=owner&sort=updated&per_page=100", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GitHub API request failed with status code: {StatusCode}", response.StatusCode);
                throw new HttpRequestException($"GitHub API request failed with status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var repositories = JsonSerializer.Deserialize<List<GitHubRepository>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Successfully fetched {Count} repositories from GitHub", repositories?.Count ?? 0);

            return repositories ?? new List<GitHubRepository>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching personal repositories from GitHub");
            throw;
        }
    }
}
