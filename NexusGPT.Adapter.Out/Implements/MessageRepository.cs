using Microsoft.EntityFrameworkCore;
using NexusGPT.Entities;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.Adapter.Out.Implements;

public class MessageRepository : IMessageOutPort
{
    private readonly NexusGptDbContext _context;

    public MessageRepository(NexusGptDbContext context)
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
        var count = await _context.Topics
            .SelectMany(x=>x.Messages)
            .Where(x => x.Id == new MessageId(messageId) )
            .CountAsync();

        return count > 0;
    }
    
    
}