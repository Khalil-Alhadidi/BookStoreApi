namespace BookStoreApi.Infrastructure;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
} 