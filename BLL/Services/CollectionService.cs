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
        public async Task<int> UpdateCollectionNameAsync(int collectionId, string name)
        {
            int rowsAffected = await _collectionRepository.UpdateCollectionNameAsync(collectionId, name);
            return rowsAffected;
        }
        public async Task<bool> DeleteCollectionAsync(int collectionId)
        {
            bool result = await _collectionRepository.DeleteCollectionAsync(collectionId);
            return result;
        }
        public async Task<int> SafeDeleteCollectionAsync(int collectionId)
        {
            int rows = await _collectionRepository.SafeDeleteCollectionAsync(collectionId);
            return rows;
        }
        public async Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId)
        {
            return await _collectionRepository.DeleteAllFlashCardsOfCollectionAsync(collectionId);
        }
    }
}
