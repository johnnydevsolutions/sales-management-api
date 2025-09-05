namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Exception thrown when a sale item validation fails.
/// </summary>
public class SaleItemValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the SaleItemValidationException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public SaleItemValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the SaleItemValidationException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public SaleItemValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
