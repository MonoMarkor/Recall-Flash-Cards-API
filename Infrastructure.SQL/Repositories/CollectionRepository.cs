using Domain.DTOs;
using Domain.IRepositories;
using Infrastructure.SQL.Database.Entities;
using Infrastructure.SQL.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {

        private readonly PostgreSQLDbContext _dbContext;

        public CollectionRepository(PostgreSQLDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateCollectionAsync(CollectionDto collection)
        {
            var newCollection = new CollectionEntity
            {
                Name = collection.Name,
                Description = collection.Description
            };
            _dbContext.Collections.Add(newCollection);
            await _dbContext.SaveChangesAsync();

            return newCollection.Id;
        }
        public async Task<int> UpdateCollectionNameAsync(int collectionId, string name)
        {
            int rowsAffected = await _dbContext.Collections
                .Where(c => c.Id == collectionId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.Name, name));

            return rowsAffected;
        }
        public async Task<int> SafeDeleteCollectionAsync(int collectionId)
        {
            int rows = _dbContext.FlashCards.Count(fc => fc.CollectionId == collectionId);
            if (rows > 0)
            {
              return rows;  
            }
            await DeleteCollectionAsync(collectionId);
            return rows;
        }
        public async Task<bool> DeleteCollectionAsync(int collectionId)
        {
            int rowsAffected = await _dbContext.Collections
                .Where(c => c.Id == collectionId).ExecuteDeleteAsync();

            return rowsAffected > 0;
        }
        public async Task<bool> DeleteAllFlashCardsOfCollectionAsync(int collectionId)
        {
            await _dbContext.FlashCards.Where(fc => fc.CollectionId == collectionId).ExecuteDeleteAsync();
            return true;
        }
    }
}
