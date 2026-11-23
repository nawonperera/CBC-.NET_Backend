namespace CristalBeautyAPI.Services.IServices;

public interface IEmailService
{
    Task SendPlainEmailAsync(string to, string subject, string body);
    Task SendHtmlEmailAsync(string to, string subject, string htmlBody);
}
