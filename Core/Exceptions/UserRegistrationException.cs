
using Microsoft.AspNetCore.Identity;

namespace Core.Exceptions;

public class UserRegistrationException : Exception
{

    public IEnumerable<IdentityError>? IdentityErrors { get; set; }

    public UserRegistrationException(string? message, IEnumerable<IdentityError> identityErrors) : base(message)
    {
        IdentityErrors = identityErrors;
    }

    public UserRegistrationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}