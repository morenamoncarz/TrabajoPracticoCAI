namespace Users.API.Exceptions;


/// Se usa cuando no se encuentra un usuario.
/// Ej: usuario no existe.

public class NotFoundException : Exception
{
    public string ErrorCode { get; }

    public NotFoundException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}