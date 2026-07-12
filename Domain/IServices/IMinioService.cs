namespace Domain.IServices
{
    public interface IMinioService
    {
        Task InitBucketAsync(string bucketName);
    }
}
