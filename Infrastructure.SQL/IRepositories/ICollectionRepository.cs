using Domain.DTOs;
using Infrastructure.SQL.Database.Entities;

namespace Infrastructure.SQL.IRepositories
{
    public interface ICollectionRepository
    {
        Task<int> CreateCollectionAsync(CollectionDto collection);
        Task<List<CollectionEntity>> GetAllCollectionsOfUserAsync(int userId);
        Task<int> UpdateCollectionNameAsync(int collectionId, string name);
        Task<int> UpdateCollectionDescriptionAsync(int collectionId, string desc);
        Task<bool> DeleteCollectionAsync(int collectionId);
        Task<int> SafeDeleteCollectionAsync(int collectionId);
        Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId);
    }
}
