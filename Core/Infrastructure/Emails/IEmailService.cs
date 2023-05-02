
namespace Core.Infrastructure.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
