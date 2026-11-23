using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services.IServices;
using CristalBeautyAPI.Utility;

namespace CristalBeautyAPI.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepo;
    private readonly IEmailService _emailService;

    public MessageService(IMessageRepository messageRepo, IEmailService emailService)
    {
        _messageRepo = messageRepo;
        _emailService = emailService;
    }

    public async Task<List<Message>> GetAllMessagesAsync()
    {
        return await _messageRepo.GetAllSortedByDateAsync();
    }

    public async Task<string> CreateMessageAsync(CreateMessageDTO dto)
    {
        if (string.IsNullOrEmpty(dto.FirstName) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Message))
        {
            throw new ArgumentException("First name, email, and message are required");
        }

        Message? lastMessage = await _messageRepo.GetLastMessageAsync();
        
        string messageId = "MSG0001";
        if (lastMessage != null && !string.IsNullOrEmpty(lastMessage.MessageId))
        {
            int lastNum = int.Parse(lastMessage.MessageId.Replace("MSG", ""));
            messageId = "MSG" + (lastNum + 1).ToString().PadLeft(4, '0');
        }

        Message newMessage = new()
        {
            MessageId = messageId,
            FirstName = dto.FirstName,
            LastName = dto.LastName ?? "",
            Email = dto.Email,
            Phone = dto.Phone ?? "",
            MessageText = dto.Message,
            Status = SD.MessageStatus_Unread
        };

        await _messageRepo.InsertAsync(newMessage);
        return messageId;
    }

    public async Task<Message?> UpdateMessageStatusAsync(string messageId, string status)
    {
        string[] validStatuses = [SD.MessageStatus_Unread, SD.MessageStatus_Read, SD.MessageStatus_Replied];
        if (!validStatuses.Contains(status))
            throw new ArgumentException("Invalid status");

        return await _messageRepo.UpdateStatusAsync(messageId, status);
    }

    public async Task<Message?> DeleteMessageAsync(string messageId)
    {
        return await _messageRepo.DeleteByMessageIdAsync(messageId);
    }

    public async Task ReplyToMessageAsync(string messageId, string subject, string replyMessage)
    {
        if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(replyMessage))
        {
            throw new ArgumentException("Subject and reply message are required");
        }

        Message? originalMessage = await _messageRepo.GetByMessageIdAsync(messageId);
        if (originalMessage == null)
        {
            throw new KeyNotFoundException("Message not found");
        }

        string htmlBody = $@"
            <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;"">
                <div style=""background: linear-gradient(135deg, #1B9C85 0%, #10b981 100%); padding: 30px; text-align: center;"">
                    <h1 style=""color: white; margin: 0; font-size: 28px;"">CRISTAL</h1>
                    <p style=""color: rgba(255,255,255,0.8); margin: 10px 0 0 0;"">Beauty &amp; Skincare</p>
                </div>
                <div style=""padding: 30px; background: #f9fafb;"">
                    <p style=""color: #374151; font-size: 16px; line-height: 1.6;"">
                        Dear {originalMessage.FirstName} {originalMessage.LastName ?? ""},
                    </p>
                    <p style=""color: #374151; font-size: 16px; line-height: 1.6;"">
                        Thank you for contacting us. Here is our response to your inquiry:
                    </p>
                    <div style=""background: white; padding: 20px; border-radius: 10px; margin: 20px 0; border-left: 4px solid #1B9C85;"">
                        <p style=""color: #374151; font-size: 16px; line-height: 1.8; white-space: pre-wrap; margin: 0;"">
                            {replyMessage}
                        </p>
                    </div>
                    <hr style=""border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;"">
                    <p style=""color: #6b7280; font-size: 14px;"">
                        <strong>Your original message:</strong>
                    </p>
                    <div style=""background: #f3f4f6; padding: 15px; border-radius: 8px; margin-top: 10px;"">
                        <p style=""color: #6b7280; font-size: 14px; line-height: 1.6; margin: 0; font-style: italic;"">
                            ""{originalMessage.MessageText}""
                        </p>
                    </div>
                </div>
            </div>";

        await _emailService.SendHtmlEmailAsync(originalMessage.Email, subject, htmlBody);
        await _messageRepo.UpdateStatusAsync(messageId, SD.MessageStatus_Replied);
    }
}
