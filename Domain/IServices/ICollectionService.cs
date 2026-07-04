using Domain.DTOs;

namespace Domain.IServices
{
    public interface ICollectionService
    {
        Task<int> CreateCollection(CollectionDto collection);
        Task<int> UpdateCollectionNameAsync(int collectionId, string name);
        Task<bool> DeleteCollectionAsync(string collectionId);
        Task<bool> DeleteAllFlashCardsOfCollectionAsync(string collectionId);
    }
}
