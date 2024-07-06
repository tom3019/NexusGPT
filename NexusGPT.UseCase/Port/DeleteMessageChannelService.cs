using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.UseCase.Port;

public class DeleteMessageChannelService : IDeleteMessageChannelService
{
    private readonly IMessageChannelOutPort _messageChannelOutPort;

    public DeleteMessageChannelService(IMessageChannelOutPort messageChannelOutPort)
    {
        _messageChannelOutPort = messageChannelOutPort;
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
        var messageChannel = await _messageChannelOutPort.GetAsync(channelId, memberId);
        if (messageChannel.IsNull())
        {
            throw new MessageChannelNotFoundException("找不到訊息頻道");
        }

        var success = await _messageChannelOutPort.DeleteAsync(messageChannel);
        return success;
    }
}