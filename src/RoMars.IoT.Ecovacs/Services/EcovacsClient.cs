using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoMars.IoT.Ecovacs.Configuration;
using RoMars.IoT.Ecovacs.Exceptions;
using RoMars.IoT.Ecovacs.Infrastructure.JsonConverters;
using RoMars.IoT.Ecovacs.Interfaces;
using RoMars.IoT.Ecovacs.Models.Responses;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web; // Required for UrlEncode

namespace RoMars.IoT.Ecovacs.Services;

/// <summary>
/// Main implementation of the Ecovacs client.
/// </summary>
public class EcovacsClient : IEcovacsClient
{
    private readonly HttpClient _httpClient;
    private readonly EcovacsClientOptions _options;
    private readonly ILogger<EcovacsClient> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsClient"/> class.
    /// </summary>
    // ... (Constructor remains the same)
    public EcovacsClient(
        IHttpClientFactory httpClientFactory,
        IOptions<EcovacsClientOptions> options,
        ILogger<EcovacsClient> logger,
        ILoggerFactory loggerFactory)
    {
        _httpClient = httpClientFactory.CreateClient("EcovacsClient");
        _options = options.Value;
        _logger = logger;
        _loggerFactory = loggerFactory;

        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new StringValueEnumConverter() }
        };

        Devices = new EcovacsDeviceService(this, _loggerFactory.CreateLogger<EcovacsDeviceService>());
        Control = new EcovacsControlService(this, _loggerFactory.CreateLogger<EcovacsControlService>());
    }

    /// <inheritdoc/>
    public IEcovacsDeviceService Devices { get; }

    /// <inheritdoc/>
    public IEcovacsControlService Control { get; }

    /// <inheritdoc/>
    public async Task<ApiResponse<T>> SendPostRequestAsync<T>(string endpoint, object parameters)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation("Sending POST request to {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);

        try
        {
            // Convert parameters to dictionary and add API key
            var stringParameters = ConvertParametersToDictionary(parameters);
            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                stringParameters["ak"] = _options.ApiKey;
            }

            var response = await _httpClient.PostAsJsonAsync(endpoint, stringParameters);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Received response for {Endpoint} with correlation ID {CorrelationId}: {Response}",
                endpoint, correlationId, responseContent);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, _jsonOptions);
            return apiResponse ?? new ApiResponse<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);
            throw new EcovacsApiException($"Request failed for endpoint {endpoint}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed for {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);
            throw new EcovacsApiException($"Failed to deserialize response from endpoint {endpoint}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred for {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);
            throw new EcovacsApiException($"Unexpected error for endpoint {endpoint}", ex);
        }
    }

    /// <summary>
    /// Sends a GET request to the Ecovacs API and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The expected response data type.</typeparam>
    /// <param name="endpoint">The API endpoint (path).</param>
    /// <param name="parameters">The parameters to include as query string arguments.</param>
    /// <returns>The API response object.</returns>
    public async Task<ApiResponse<T>> SendGetRequestAsync<T>(string endpoint, object parameters)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogInformation("Sending GET request to {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);

        try
        {
            // Convert parameters to a dictionary
            var stringParameters = ConvertParametersToDictionary(parameters);

            // Add API key
            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                stringParameters["ak"] = _options.ApiKey;
            }

            // Build the query string
            var queryString = BuildQueryString(stringParameters);
            var requestUri = $"{endpoint}{queryString}";

            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Received response for {Endpoint} with correlation ID {CorrelationId}: {Response}",
                requestUri, correlationId, responseContent);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, _jsonOptions);
            return apiResponse ?? new ApiResponse<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP GET request failed for {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);
            throw new EcovacsApiException($"GET Request failed for endpoint {endpoint}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization failed for {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);
            throw new EcovacsApiException($"Failed to deserialize response from endpoint {endpoint}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred for {Endpoint} with correlation ID {CorrelationId}", endpoint, correlationId);
            throw new EcovacsApiException($"Unexpected error for endpoint {endpoint}", ex);
        }
    }

    private static Dictionary<string, string> ConvertParametersToDictionary(object parameters)
    {
        var result = new Dictionary<string, string>();

        if (parameters is not IEnumerable<KeyValuePair<string, object>> parameterPairs)
        {
            // If it's not already a dictionary-like object, try to convert it from properties
            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters)?.ToString() ?? string.Empty;
                result[property.Name] = value;
            }
        }
        else
        {
            // It's already an IEnumerable<KeyValuePair<string, object>>
            foreach (var pair in parameterPairs)
            {
                result[pair.Key] = pair.Value?.ToString() ?? string.Empty;
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a dictionary of parameters into a URL-encoded query string (e.g., "?key1=value1&key2=value2").
    /// </summary>
    /// <param name="parameters">The dictionary of parameters.</param>
    /// <returns>A URL-encoded query string.</returns>
    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return string.Empty;
        }

        var query = string.Join("&",
            parameters.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));

        return "?" + query;
    }
}