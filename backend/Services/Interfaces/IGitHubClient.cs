using Services.Models;

namespace Services.Interfaces;

/// <summary>
/// Interface for GitHub API client
/// </summary>
public interface IGitHubClient
{
    /// <summary>
    /// Retrieves personal repositories for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of GitHub repository information</returns>
    Task<IEnumerable<GitHubRepository>> GetPersonalRepositoriesAsync(CancellationToken cancellationToken = default);
}
