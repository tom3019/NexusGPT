using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using NexusGPT.UseCase.Port.Out;

namespace NexusGPT.Adapter.Out.ImageStorage.S3;

public class S3StorageRepository : IImageStorageOutPort
{
    private readonly IAmazonS3 _amazonS3;
    private readonly AwsS3StorageOptions _options;

    public S3StorageRepository(IAmazonS3 amazonS3,
        IOptions<AwsS3StorageOptions> options)
    {
        _amazonS3 = amazonS3;
        _options = options.Value;
    }

    /// <summary>
    /// 儲存物件
    /// </summary>
    /// <param name="base64"></param>
    /// <returns>物件路徑</returns>
    public async Task<string> SaveObjectAsync(string base64)
    {
        var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        var filePath = Path.Combine(DateTime.Now.Date.ToString("yyyyMMdd"), fileName);

        var putObjectResponse = await _amazonS3.PutObjectAsync(
            new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = filePath,
                InputStream = new MemoryStream(Convert.FromBase64String(base64))
            });
        
        if (putObjectResponse.HttpStatusCode == HttpStatusCode.Accepted)
        {
            return filePath;
        }

        return string.Empty;
    }
}