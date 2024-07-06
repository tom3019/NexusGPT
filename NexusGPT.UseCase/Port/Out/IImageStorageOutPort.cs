namespace NexusGPT.UseCase.Port.Out;

public interface IImageStorageOutPort
{
    /// <summary>
    /// 儲存物件
    /// </summary>
    /// <param name="base64"></param>
    /// <returns>物件路徑</returns>
    Task<string> SaveObjectAsync(string base64);
}