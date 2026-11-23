using CristalBeautyAPI.Models;

namespace CristalBeautyAPI.Repositories.IRepository;

public interface IMessageRepository
{
    Task<List<Message>> GetAllSortedByDateAsync();
    Task<Message?> GetByMessageIdAsync(string messageId);
    Task<Message?> GetLastMessageAsync();
    Task InsertAsync(Message message);
    Task<Message?> UpdateStatusAsync(string messageId, string status);
    Task<Message?> DeleteByMessageIdAsync(string messageId);
}
