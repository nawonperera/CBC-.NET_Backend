using CristalBeautyAPI.Models;
using CristalBeautyAPI.Models.Dto;

namespace CristalBeautyAPI.Services.IServices;

public interface IMessageService
{
    Task<List<Message>> GetAllMessagesAsync();
    Task<string> CreateMessageAsync(CreateMessageDTO dto);
    Task<Message?> UpdateMessageStatusAsync(string messageId, string status);
    Task<Message?> DeleteMessageAsync(string messageId);
    Task ReplyToMessageAsync(string messageId, string subject, string replyMessage);
}
