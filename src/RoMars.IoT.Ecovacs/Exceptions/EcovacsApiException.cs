namespace RoMars.IoT.Ecovacs.Exceptions;

/// <summary>
/// Exception thrown when the Ecovacs API returns an error.
/// </summary>
public class EcovacsApiException : Exception
{
    /// <summary>
    /// Gets the error code returned by the API.
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// Gets the endpoint that was called when the error occurred.
    /// </summary>
    public string? Endpoint { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public EcovacsApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code returned by the API.</param>
    public EcovacsApiException(string message, int errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code returned by the API.</param>
    /// <param name="endpoint">The endpoint that was called.</param>
    public EcovacsApiException(string message, int errorCode, string endpoint)
        : base(message)
    {
        ErrorCode = errorCode;
        Endpoint = endpoint;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public EcovacsApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EcovacsApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code returned by the API.</param>
    /// <param name="endpoint">The endpoint that was called.</param>
    /// <param name="innerException">The inner exception.</param>
    public EcovacsApiException(string message, int errorCode, string endpoint, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Endpoint = endpoint;
    }
}
