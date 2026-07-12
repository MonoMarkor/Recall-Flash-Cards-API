namespace Domain.IServices
{
    public interface IMinioService
    {
        Task InitBucketAsync(string bucketName);
        Task<Byte[]> GetFileAsync(string bucketName, string objectName);
        Task UploadOrUpdateFileAsync(string bucketName, string objectName, byte[] file);
        Task DeleteFileAsync(string bucketName, string objectName);
    }
}
