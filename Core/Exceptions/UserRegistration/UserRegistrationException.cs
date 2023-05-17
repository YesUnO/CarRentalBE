using Microsoft.AspNetCore.Identity;

namespace Core.Exceptions.UserRegistration;

public class UserRegistrationException : Exception
{

    public IEnumerable<UserRegistrationError>? Errors { get; set; }

    public UserRegistrationException(string? message, IEnumerable<UserRegistrationError> identityErrors) : base(message)
    {
        Errors = identityErrors;
    }

    public UserRegistrationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}