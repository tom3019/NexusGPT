using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.Adapter.Out.ImageStorage.Local;

public class LocalStorageRepository : IImageStorageOutPort
{
    /// <summary>
    ///  建立資料夾
    /// </summary>
    /// <param name="path"></param>
    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 儲存物件
    /// </summary>
    /// <param name="base64"></param>
    /// <returns>物件路徑</returns>
    public async Task<string> SaveObjectAsync(string base64)
    {
        var path = Path.Combine("./Images", DateTime.Now.Date.ToString("yyyyMMdd"));
        CreateDirectory(path);

        var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        
        var filePath = Path.Combine(path, fileName);
        await using var outputFileStream = new FileStream(filePath, FileMode.Create);

        var bytes = Convert.FromBase64String(base64);
        var contents = new MemoryStream(bytes);
        await contents.CopyToAsync(outputFileStream);

        return filePath.Replace("\\","/");
    }
}