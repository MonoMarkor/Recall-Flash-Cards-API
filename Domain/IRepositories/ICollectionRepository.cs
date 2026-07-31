using Domain.DTOs;

namespace Domain.IRepositories
{
    public interface ICollectionRepository
    {
        Task<int> CreateCollectionAsync(CollectionDto collection);
        Task<int> UpdateCollectionNameAsync(int collectionId, string name);
        Task<int> UpdateCollectionDescriptionAsync(int collectionId, string desc);
        Task<bool> DeleteCollectionAsync(int collectionId);
        Task<int> SafeDeleteCollectionAsync(int collectionId);
        Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId);
    }
}
