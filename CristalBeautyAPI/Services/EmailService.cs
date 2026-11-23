using MailKit.Net.Smtp;
using MimeKit;
using CristalBeautyAPI.Services.IServices;

namespace CristalBeautyAPI.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly string _senderEmail;
    private readonly string _appPassword;

    public EmailService(IConfiguration config)
    {
        _config = config;
        _senderEmail = _config["Email:SenderEmail"] ?? "";
        _appPassword = _config["Email:AppPassword"] ?? "";
    }

    public async Task SendPlainEmailAsync(string to, string subject, string body)
    {
        MimeMessage emailMessage = new();
        emailMessage.From.Add(MailboxAddress.Parse(_senderEmail));
        emailMessage.To.Add(MailboxAddress.Parse(to));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("plain") { Text = body };

        await SendAsync(emailMessage);
    }

    public async Task SendHtmlEmailAsync(string to, string subject, string htmlBody)
    {
        MimeMessage emailMessage = new();
        emailMessage.From.Add(MailboxAddress.Parse(_senderEmail));
        emailMessage.To.Add(MailboxAddress.Parse(to));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = htmlBody };

        await SendAsync(emailMessage);
    }

    private async Task SendAsync(MimeMessage message)
    {
        using SmtpClient smtp = new();
        await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_senderEmail, _appPassword);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
