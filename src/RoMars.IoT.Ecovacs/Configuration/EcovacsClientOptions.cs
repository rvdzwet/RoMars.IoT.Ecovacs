namespace RoMars.IoT.Ecovacs.Configuration;

/// <summary>
/// Configuration options for the Ecovacs client.
/// </summary>
public class EcovacsClientOptions
{
    /// <summary>
    /// Gets or sets the base URL for the Ecovacs API.
    /// Default is "https://open.ecovacs.com".
    /// </summary>
    public string BaseUrl { get; set; } = "https://open.ecovacs.com";

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// This should be set via environment variable or configuration.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the timeout for HTTP requests.
    /// Default is 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed requests.
    /// Default is 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delay between retry attempts.
    /// Default is 1 second.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}
