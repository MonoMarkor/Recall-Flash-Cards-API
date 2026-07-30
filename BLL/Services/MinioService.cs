using Domain.IServices;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace BLL.Services
{
    public class MinioService : IMinioService
    {
        private readonly IMinioClient _minioClient;

        public MinioService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task InitBucketAsync(string bucketName)
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);
                if (found)
                {
                    Console.WriteLine($"{bucketName} already exists");
                }
                else
                {
                    var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(makeBucketArgs);
                    Console.WriteLine($"{bucketName} is created successfully");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error initializing bucket({bucketName}): {e.Message}");
                throw;
            }
        }

        public async Task<Byte[]> GetFileAsync(string bucketName, string objectName)
        {
            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                                                    .WithBucket(bucketName)
                                                    .WithObject(objectName);
                await _minioClient.StatObjectAsync(statObjectArgs);

                byte[] fileBytes = Array.Empty<byte>();
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                                                  .WithBucket(bucketName)
                                                  .WithObject(objectName)
                                                  .WithCallbackStream((stream) =>
                                                  {
                                                      using (var memoryStream = new MemoryStream())
                                                      {
                                                          stream.CopyTo(memoryStream);
                                                          fileBytes = memoryStream.ToArray();
                                                      }
                                                  });
                await _minioClient.GetObjectAsync(getObjectArgs);
                return fileBytes;
            }
            catch (MinioException e)
            {
                Console.WriteLine("Minio Error occurred while retrieving: " + e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Minio Error occurred while retrieving: " + e.Message);
                throw;
            }
        }

        public async Task UploadOrUpdateFileAsync(string bucketName, string objectName, byte[] file)
        {
            try
            {
                var progress = new Progress<ProgressReport>(progressReport =>
                {
                    Console.WriteLine(
                            $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes");
                    if (progressReport.Percentage != 100)
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    else Console.WriteLine();
                });

                using (var stream = new MemoryStream(file))
                {
                    PutObjectArgs putObjectArgs = new PutObjectArgs()
                                                  .WithBucket(bucketName)
                                                  .WithObject(objectName)
                                                  .WithContentType("application/octet-stream")
                                                  .WithStreamData(stream)
                                                  .WithObjectSize(stream.Length)
                                                  .WithProgress(progress);
                    await _minioClient.PutObjectAsync(putObjectArgs);
                    Console.WriteLine($"File: {objectName} is uploaded successfully");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Minio Error occurred while uploading/updating: " + e.Message);
                throw;
            }
        }

        public async Task CopyFileInSameBucketAsync(string bucketName, string originalObjectName, string newObjectName)
        {
            try
            {
                CopyObjectArgs copyObjectArgs = new CopyObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(newObjectName)
                    .WithCopyObjectSource(new CopySourceObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(originalObjectName));

                await _minioClient.CopyObjectAsync(copyObjectArgs);
                Console.WriteLine($"Successfully copied '{originalObjectName}' to '{newObjectName}' in bucket '{bucketName}'.");
            }
            catch (MinioException e)
            {
                Console.WriteLine("Minio Error occurred while copying: " + e.Message);
                throw;
            }
        }

        public async Task DeleteFileAsync(string bucketName, string objectName) 
        {
            try
            {
                RemoveObjectArgs rmArgs = new RemoveObjectArgs()
                                              .WithBucket(bucketName)
                                              .WithObject(objectName);
                await _minioClient.RemoveObjectAsync(rmArgs);
                Console.WriteLine($"successfully removed {bucketName}/{objectName}");
            }
            catch (MinioException e)
            {
                Console.WriteLine("Minio Error occured while deleting: " + e.Message);
                throw;
            }
        }
    }
}
