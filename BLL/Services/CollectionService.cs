using Domain.DTOs;
using Domain.IServices;
using Domain.IRepositories;

namespace BLL.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;

        public CollectionService(ICollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }
        public Task<int> CreateCollection(CollectionDto collection)
        {
            var collectionId = _collectionRepository.CreateCollection(collection);
            return collectionId;
        }
        public Task<int> UpdateCollectionNameAsync(int collectionId, string name)
        {
            return Task.FromResult(0);
        }
        public Task<bool> DeleteCollectionAsync(int collectionId)
        {
            return Task.FromResult(false);
        }
        public Task<int> SafeDeleteCollectionAsync(int collectionId)
        {
            return Task.FromResult(0);
        }
        public Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId)
        {
            return Task.FromResult(false);
        }
    }
}
