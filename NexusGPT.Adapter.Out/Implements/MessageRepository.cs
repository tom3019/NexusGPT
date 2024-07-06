using Microsoft.EntityFrameworkCore;
using NexusGPT.Entities;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.Adapter.Out.Implements;

public class MessageRepository : IMessageOutPort
{
    private readonly MessageChannelDbContext _context;

    public MessageRepository(MessageChannelDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 產生Id
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> GenerateIdAsync()
    {
        var id = Guid.NewGuid();
        var isExist = await IsExistAsync(id);
        while (isExist)
        {
            id = Guid.NewGuid();
        }
        
        return id;
    }

    /// <summary>
    /// 判斷Id是否存在
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    private async Task<bool> IsExistAsync(Guid messageId)
    {
        var count = await _context.MessageChannels
            .SelectMany(x=>x.Messages)
            .Where(x => x.Id == new MessageId(messageId) )
            .CountAsync();

        return count > 0;
    }
    
    
}