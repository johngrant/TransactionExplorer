namespace Services.Configuration;

/// <summary>
/// Configuration options for GitHub API client
/// </summary>
public class GitHubOptions
{
    public const string SectionName = "GitHubApi";

    /// <summary>
    /// Base URL for the GitHub API (default: https://api.github.com)
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.github.com";

    /// <summary>
    /// GitHub personal access token for authentication
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Default timeout for HTTP requests in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// User agent string for API requests (required by GitHub)
    /// </summary>
    public string UserAgent { get; set; } = "TransactionExplorer";
}
