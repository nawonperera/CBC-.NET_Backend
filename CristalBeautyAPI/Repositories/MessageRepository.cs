using MongoDB.Driver;
using CristalBeautyAPI.Models;
using CristalBeautyAPI.Services;
using CristalBeautyAPI.Repositories.IRepository;

namespace CristalBeautyAPI.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _messages;

    public MessageRepository(MongoDbService db)
    {
        _messages = db.Messages;
    }

    public async Task<List<Message>> GetAllSortedByDateAsync()
    {
        return await _messages
            .Find(_ => true)
            .SortByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Message?> GetByMessageIdAsync(string messageId)
    {
        return await _messages.Find(m => m.MessageId == messageId).FirstOrDefaultAsync();
    }

    public async Task<Message?> GetLastMessageAsync()
    {
        return await _messages
            .Find(_ => true)
            .SortByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task InsertAsync(Message message)
    {
        await _messages.InsertOneAsync(message);
    }

    public async Task<Message?> UpdateStatusAsync(string messageId, string status)
    {
        UpdateDefinition<Message> update = Builders<Message>.Update.Set(m => m.Status, status);
        return await _messages.FindOneAndUpdateAsync(
            m => m.MessageId == messageId,
            update,
            new FindOneAndUpdateOptions<Message> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<Message?> DeleteByMessageIdAsync(string messageId)
    {
        return await _messages.FindOneAndDeleteAsync(m => m.MessageId == messageId);
    }
}
