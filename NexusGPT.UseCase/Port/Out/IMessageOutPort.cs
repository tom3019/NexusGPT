using NexusGPT.Entities;

namespace NexusGPT.UseCase.Port.Out;

public interface IMessageOutPort
{
    /// <summary>
    /// 產生Id
    /// </summary>
    /// <returns></returns>
    Task<Guid> GenerateIdAsync();
}