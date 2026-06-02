namespace Orders.API.Exceptions;

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }

    public BusinessRuleException(string errorCode, string message) : base(message)
    {
        // Guardamos el código propio del catálogo ORD
        ErrorCode = errorCode;
    }
}