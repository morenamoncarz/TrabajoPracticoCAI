namespace Users.API.Exceptions;


/// Se usa cuando se rompe una regla de negocio.
/// Ej: email duplicado, usuario bloqueado, etc.

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }

    public BusinessRuleException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}