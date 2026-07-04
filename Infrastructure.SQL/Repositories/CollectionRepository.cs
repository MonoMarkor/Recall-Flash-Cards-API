using Domain.IRepositories;
using Domain.DTOs;

namespace Infrastructure.SQL.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        public Task<int> CreateCollection(CollectionDto collection)
        {
            return Task.FromResult(0);
        }
        public Task<int> UpdateCollectionNameAsync(int collectionId, string name)
        {
            return Task.FromResult(0);
        }
        public Task<bool> DeleteCollectionAsync(string collectionId)
        {
            return Task.FromResult(false);
        }
        public Task<bool> DeleteAllFlashCardsOfCollectionAsync(string collectionId)
        {
            return Task.FromResult(false);
        }
    }
}
