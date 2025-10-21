# GitHub Integration

This document describes how to use the GitHub integration feature in the Transaction Explorer API.

## Overview

The GitHub integration allows you to retrieve your personal GitHub repositories through the Transaction Explorer API.

## Configuration

To use the GitHub API integration, you need to configure a GitHub Personal Access Token in the application settings.

### Setting up a GitHub Token

1. Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
2. Click "Generate new token (classic)"
3. Give it a descriptive name (e.g., "Transaction Explorer")
4. Select the following scopes:
   - `repo` - Full control of private repositories (required to list personal repositories)
5. Click "Generate token" and copy the token

### Configure the Application

Add your GitHub token to the `appsettings.json` or `appsettings.Development.json` file:

```json
{
  "GitHubApi": {
    "BaseUrl": "https://api.github.com",
    "AccessToken": "your_github_token_here",
    "TimeoutSeconds": 30,
    "UserAgent": "TransactionExplorer"
  }
}
```

**Security Note**: Never commit your GitHub token to source control. Consider using environment variables or user secrets for production deployments.

### Using Environment Variables

You can also set the token using an environment variable:

```bash
export GitHubApi__AccessToken="your_github_token_here"
```

Or in Docker:

```bash
docker run -e GitHubApi__AccessToken="your_github_token_here" ...
```

## API Endpoint

### Get Personal Repositories

**Endpoint**: `GET /api/github/repositories`

**Description**: Retrieves all personal repositories for the authenticated GitHub user.

**Parameters**: None

**Response**: Array of GitHub repository objects

**Example Response**:
```json
[
  {
    "id": 123456789,
    "name": "my-repo",
    "full_name": "username/my-repo",
    "description": "My awesome repository",
    "private": false,
    "html_url": "https://github.com/username/my-repo",
    "created_at": "2024-01-15T10:30:00Z",
    "updated_at": "2024-10-20T15:45:00Z",
    "pushed_at": "2024-10-20T15:45:00Z",
    "language": "C#",
    "stargazers_count": 10,
    "watchers_count": 10,
    "forks_count": 3,
    "open_issues_count": 2,
    "default_branch": "main"
  }
]
```

## Testing

### Using Swagger UI

1. Navigate to `http://localhost:5070/swagger`
2. Expand the "GitHub" section
3. Click on the `GET /api/github/repositories` endpoint
4. Click "Try it out"
5. Click "Execute"

### Using cURL

```bash
curl -X GET "http://localhost:5070/api/github/repositories" -H "accept: application/json"
```

### Using the API Root Endpoint

The GitHub endpoint is listed in the API root response:

```bash
curl http://localhost:5070/
```

Look for the `github.repositories` section in the response.

## Error Handling

The API will return appropriate HTTP status codes:

- `200 OK` - Successfully retrieved repositories
- `500 Internal Server Error` - Error fetching repositories (e.g., invalid token, GitHub API unavailable)

## Rate Limiting

GitHub API has rate limits:
- **Authenticated requests**: 5,000 requests per hour
- **Unauthenticated requests**: 60 requests per hour

The GitHub integration uses authenticated requests when a token is configured, providing higher rate limits.

## Limitations

- The current implementation retrieves up to 100 repositories per request
- Only repositories owned by the authenticated user are returned (not organizations)
- The endpoint does not support pagination yet (for users with more than 100 repositories)

## Future Enhancements

Potential improvements for this feature:
- Pagination support for users with many repositories
- Filtering by repository type (public/private)
- Sorting options
- Organization repositories
- Repository statistics and insights
