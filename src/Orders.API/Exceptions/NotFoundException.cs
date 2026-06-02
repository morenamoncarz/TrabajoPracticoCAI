namespace Orders.API.Exceptions;

public class NotFoundException : Exception
{
    public string ErrorCode { get; }

    public NotFoundException(string errorCode, string message) : base(message)
    {
        // Guardamos el código propio del catálogo ORD
        ErrorCode = errorCode;
    }
}