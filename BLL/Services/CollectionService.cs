using Domain.DTOs;
using Domain.IRepositories;
using Domain.IServices;
using Infrastructure.SQL.IRepositories;
using Microsoft.Extensions.Configuration;

namespace BLL.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IMinioService _minioService;
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly string _imageBucket;
        private readonly string _audioBucket;

        public CollectionService(ICollectionRepository collectionRepository, IMinioService minioService, IFlashCardRepository flashCardRepository, IConfiguration configuration)
        {
            _collectionRepository = collectionRepository;
            _minioService = minioService;
            _flashCardRepository = flashCardRepository;
            _imageBucket = configuration["Minio:ImageBucket"]
                ?? throw new InvalidOperationException("Minio:ImageBucket configuration is missing."); ;
            _audioBucket = configuration["Minio:AudioBucket"]
                ?? throw new InvalidOperationException("Minio:AudioBucket configuration is missing."); ;
        }

        private async Task<bool> DeleteAllFlashCards(int collectionId)
        {
            var flashCardEntities = await _flashCardRepository.RetrieveAllFlashCardsByCollectionIdAsync(collectionId);

            var mapTasks = flashCardEntities.Select(async entity =>
            {
                if (entity.Answer != null)
                {
                    switch (entity.Answer.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Image:
                            if (entity.Answer.ImagePath != null)
                            {
                                await _minioService.DeleteFileAsync(_imageBucket, entity.Answer.ImagePath);
                            }
                            break;
                        case CardContentEntity.PrimaryContentType.Audio:
                            if (entity.Answer.AudioPath != null)
                            {
                                await _minioService.DeleteFileAsync(_audioBucket, entity.Answer.AudioPath);
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (entity.Question != null)
                {
                    switch (entity.Question.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Image:
                            if (entity.Question.ImagePath != null)
                            {
                                await _minioService.DeleteFileAsync(_imageBucket, entity.Question.ImagePath);
                            }
                            break;
                        case CardContentEntity.PrimaryContentType.Audio:
                            if (entity.Question.AudioPath != null)
                            {
                                await _minioService.DeleteFileAsync(_audioBucket, entity.Question.AudioPath);
                            }
                            break;
                        default:
                            break;
                    }
                }
            });

            await Task.WhenAll(mapTasks);
            return true;
        }

        // returns a "0" if collection Id is present
        public async Task<int> CreateCollectionAsync(CollectionDto collection)
        {
            if (collection == null)
            {
                return 0;
            }
            if (collection.Id != 0)
            {
                return 0;
            }
            var collectionId = await _collectionRepository.CreateCollectionAsync(collection);
            return collectionId;
        }

        // returns a "0" if not updated
        public async Task<int> UpdateCollectionNameAsync(int collectionId, string name)
        {
            int rowsAffected = await _collectionRepository.UpdateCollectionNameAsync(collectionId, name);
            return rowsAffected;
        }
        // returns a "0" if not updated
        public async Task<int> UpdateCollectionDescriptionAsync(int collectionId, string desc)
        {
            int rowsAffected = await _collectionRepository.UpdateCollectionDescriptionAsync(collectionId, desc);
            return rowsAffected;
        }
        public async Task<bool> DeleteCollectionAsync(int collectionId)
        {
            if (await DeleteAllFlashCards(collectionId)){
                bool result = await _collectionRepository.DeleteCollectionAsync(collectionId);
                return result;
            }
            
            return false;
        }
        public async Task<int> SafeDeleteCollectionAsync(int collectionId)
        {
            int rows = await _collectionRepository.SafeDeleteCollectionAsync(collectionId);
            return rows;
        }
        public async Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId)
        {
            if (await DeleteAllFlashCards(collectionId))
            {
                return await _collectionRepository.DeleteAllFlashCardsOfCollectionAsync(collectionId);
            }
            return false;
        }
    }
}
