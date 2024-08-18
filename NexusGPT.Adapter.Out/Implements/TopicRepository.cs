using Microsoft.EntityFrameworkCore;
using NexusGPT.Entities;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.Adapter.Out.Implements;

public class TopicRepository : ITopicOutPort
{
    private readonly NexusGptDbContext _context;

    public TopicRepository(NexusGptDbContext context)
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
            isExist = await IsExistAsync(id);
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
        var count = await _context.Topics
            .Where(x => x.Id == new TopicId(channelId))
            .Include(x => x.Messages)
            .CountAsync();

        return count > 0;
    }

    /// <summary>
    /// 儲存
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public async Task<bool> SaveAsync(Topic topic)
    {
        _context.Topics.Add(topic);
        var successCount = await _context.SaveChangesAsync();
        return successCount > 0;
    }

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="topicId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<Topic> GetAsync(Guid topicId, Guid memberId)
    {
        var messageChannel = await _context.Topics
            .Where(x => x.Id == new TopicId(topicId))
            .Where(x => x.MemberId == new MemberId(memberId))
            .Include(x => x.Messages)
            .FirstOrDefaultAsync();

        return messageChannel ?? Topic.Null;
    }

    /// <summary>
    /// 取得MessageChannel
    /// </summary>
    /// <param name="topicId"></param>
    /// <returns></returns>
    public async Task<Topic> GetAsync(Guid topicId)
    {
        var messageChannel = await _context.Topics
            .Where(x => x.Id == new TopicId(topicId))
            .Include(x => x.Messages)
            .FirstOrDefaultAsync();

        return messageChannel ?? Topic.Null;
    }

    /// <summary>
    /// 更新MessageChannel
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(Topic topic)
    {
        _context.Topics.Update(topic);
        var successCount = await _context.SaveChangesAsync();
        return successCount > 0;
    }

    /// <summary>
    /// 刪除頻道
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(Topic topic)
    {
        _context.Topics.Remove(topic);
        var successCount = await _context.SaveChangesAsync();
        return successCount > 0;
    }

    /// <summary>
    /// 取得清單
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TopicDataModel>> GetListAsync(Guid memberId)
    {
        var messageChannels = await _context.Topics
            .Where(x => x.MemberId == new MemberId(memberId))
            .Include(x => x.Messages)
            .Select(x => new TopicDataModel
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
    public async Task<IEnumerable<SearchTopicDataModel>> SearchMessageChannelAsync
        (Guid memberId, string keyword)
    {

        var messageChannels = await _context.Topics
            .Where(x => x.MemberId == new MemberId(memberId))
            .Include(x => x.Messages)
            .Where(x => x.Title.Contains(keyword) 
                        || x.Messages.Any(y=>y.Question.Contains(keyword))
                        || x.Messages.Any(y=>y.Answer.Contains(keyword)))
            .Select(x => new SearchTopicDataModel
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