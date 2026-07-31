using Domain.DTOs;

namespace Domain.IServices
{
    public interface ICollectionService
    {
        Task<int> CreateCollectionAsync(CollectionDto collection);
        Task<int> UpdateCollectionNameAsync(int collectionId, string name);
        Task<bool> DeleteCollectionAsync(int collectionId);
        Task<int> SafeDeleteCollectionAsync(int collectionId);
        Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId);
    }
}
