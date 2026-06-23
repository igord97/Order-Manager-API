namespace OrderManager.Core.Exceptions;

internal class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException()
    {
    }

    public UnauthorizedAccessException(string message)
        : base(message)
    {
    }
}
