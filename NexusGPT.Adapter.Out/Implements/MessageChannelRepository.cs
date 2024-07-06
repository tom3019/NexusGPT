using Microsoft.EntityFrameworkCore;
using NexusGPT.Entities;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.Adapter.Out.Implements;

public class MessageChannelRepository : IMessageChannelOutPort
{
    private readonly MessageChannelDbContext _context;

    public MessageChannelRepository(MessageChannelDbContext context)
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
    /// <param name="channelId"></param>
    /// <returns></returns>
    private async Task<bool> IsExistAsync(Guid channelId)
    {
        var count = await _context.MessageChannels
            .Where(x => x.Id == new MessageChannelId(channelId))
            .Include(x => x.Messages)
            .CountAsync();

        return count > 0;
    }

    /// <summary>
    /// 儲存
    /// </summary>
    /// <param name="messageChannel"></param>
    /// <returns></returns>
    public async Task<bool> SaveAsync(MessageChannel messageChannel)
    {
        _context.MessageChannels.Add(messageChannel);
        var successCount = await _context.SaveChangesAsync();
        return successCount > 0;
    }

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<MessageChannel> GetAsync(Guid channelId, Guid memberId)
    {
        var messageChannel = await _context.MessageChannels
            .Where(x => x.Id == new MessageChannelId(channelId))
            .Where(x => x.MemberId == new MemberId(memberId))
            .Include(x => x.Messages)
            .FirstOrDefaultAsync();

        return messageChannel ?? MessageChannel.Null;
    }

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public async Task<MessageChannel> GetAsync(Guid channelId)
    {
        var messageChannel = await _context.MessageChannels
            .Where(x => x.Id == new MessageChannelId(channelId))
            .Include(x => x.Messages)
            .FirstOrDefaultAsync();

        return messageChannel ?? MessageChannel.Null;
    }

    /// <summary>
    /// 更新MessageChannel
    /// </summary>
    /// <param name="messageChannel"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(MessageChannel messageChannel)
    {
        _context.MessageChannels.Update(messageChannel);
        var successCount = await _context.SaveChangesAsync();
        return successCount > 0;
    }

    /// <summary>
    /// 刪除頻道
    /// </summary>
    /// <param name="messageChannel"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(MessageChannel messageChannel)
    {
        _context.MessageChannels.Remove(messageChannel);
        var successCount = await _context.SaveChangesAsync();
        return successCount > 0;
    }

    /// <summary>
    /// 取得清單
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<MessageChannelDataModel>> GetListAsync(Guid memberId)
    {
        var messageChannels = await _context.MessageChannels
            .Where(x => x.MemberId == new MemberId(memberId))
            .Include(x => x.Messages)
            .Select(x => new MessageChannelDataModel
            {
                Id = x.Id,
                Title = x.Title,
                CreateTime = x.CreateTime.DateTime,
                LastMessageCreateTime = x.Messages
                    .OrderByDescending(x => x.CreateTime)
                    .FirstOrDefault().CreateTime.DateTime
            })
            .ToListAsync();

        return messageChannels;
    }

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SearchMessageChannelDataModel>> SearchMessageChannelAsync
        (Guid memberId, string keyword)
    {

        var messageChannels = await _context.MessageChannels
            .Where(x => x.MemberId == new MemberId(memberId))
            .Include(x => x.Messages)
            .Where(x => x.Title.Contains(keyword) 
                        || x.Messages.Any(y=>y.Question.Contains(keyword))
                        || x.Messages.Any(y=>y.Answer.Contains(keyword)))
            .Select(x => new SearchMessageChannelDataModel
            {
                Id = x.Id,
                MessageIds = x.Messages
                    .Where(y=>y.Question.Contains(keyword) 
                              || y.Answer.Contains(keyword))
                    .Select(y=>(Guid) y.Id)
            })
            .ToListAsync();

        return messageChannels;
    }
}