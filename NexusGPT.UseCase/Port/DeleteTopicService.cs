using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase.Port;

public class DeleteTopicService : IDeleteTopicService
{
    private readonly ITopicOutPort _topicOutPort;

    public DeleteTopicService(ITopicOutPort topicOutPort)
    {
        _topicOutPort = topicOutPort;
    }

    /// <summary>
    /// 刪除訊息頻道
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="MessageChannelNotFoundException"></exception>
    public async Task<bool> HandleAsync(Guid channelId, Guid memberId)
    {
        var messageChannel = await _topicOutPort.GetAsync(channelId, memberId);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }

        var success = await _topicOutPort.DeleteAsync(messageChannel);
        return success;
    }
}