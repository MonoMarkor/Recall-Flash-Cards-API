using Domain.DTOs;
using Infrastructure.SQL.IRepositories;
using Domain.IServices;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.SQL.Repositories
{
    public class FlashCardRepository : IFlashCardRepository
    {

        private readonly PostgreSQLDbContext _dbContext;
        
        public FlashCardRepository(PostgreSQLDbContext dbContext, IMinioService minioService, IConfiguration configuration)
        {
            _dbContext = dbContext;
        }

        public async Task<FlashCardEntity> RetrieveFlashCardByIdAsync(int flashCardId)
        {
            var flashCardEntity = await _dbContext.FlashCards
                .AsNoTracking()
                .Include(f => f.Question)
                .Include(f => f.Answer)
                .SingleOrDefaultAsync(f => f.Id == flashCardId);

            return flashCardEntity;
        }

        public async Task<List<FlashCardEntity>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId)
        {
            var flashCardEntities = await _dbContext.FlashCards
                                        .Where(fc => fc.CollectionId == collectionId)
                                        .Include(fc => fc.Question)
                                        .Include(fc => fc.Answer)
                                        .OrderBy(fc => fc.Id)
                                        .ToListAsync();

            return flashCardEntities;
        }

        public async Task<List<FlashCardEntity>> RetrievePagedFlashCardsByCollectionIdAsync(int collectionId, int index, int amount)
        {

            var flashCardEntities = await _dbContext.FlashCards
                                        .Where(fc => fc.CollectionId == collectionId)
                                        .Include(fc => fc.Question)
                                        .Include(fc => fc.Answer)
                                        .OrderBy(fc => fc.Id)
                                        .Skip(index)
                                        .Take(amount)
                                        .ToListAsync();

            return flashCardEntities;

        }

        public async Task<int> CreateFlashCardAsync(FlashCardDto flashCard)
        {
            FlashCardEntity? flashCardEntity = null;

            if (flashCardEntity == null)
            {
                flashCardEntity = new FlashCardEntity();
                _dbContext.FlashCards.Add(flashCardEntity);
            }

            flashCardEntity.ExpirationDate = flashCard.ExpirationDate;
            flashCardEntity.Difficulty = (FlashCardEntity.DifficultyLevel)flashCard.Difficulty;
            flashCardEntity.CollectionId = flashCard.CollectionId;

            await _dbContext.SaveChangesAsync();
            return flashCardEntity.Id;
        }

        public async Task<int> CreateFlashCardAsync(FlashCardEntity flashCardEntity)
        {

            if (flashCardEntity == null)
            {
                return 0;
            }

            _dbContext.FlashCards.Add(flashCardEntity);
            await _dbContext.SaveChangesAsync();
            return flashCardEntity.Id;
        }

        public async Task<bool> UpdateFlashCardAsync(FlashCardDto flashCard)
        {
            FlashCardEntity? flashCardEntity = null;

            if (flashCard.Id > 0)
            {
                flashCardEntity = await _dbContext.FlashCards
                .Include(f => f.Question)
                .Include(f => f.Answer)
                .SingleOrDefaultAsync(f => f.Id == flashCard.Id);
            }

            if (flashCardEntity == null)
            {
                return false;
            }

            flashCardEntity.ExpirationDate = flashCard.ExpirationDate;
            flashCardEntity.Difficulty = (FlashCardEntity.DifficultyLevel)flashCard.Difficulty;
            flashCardEntity.CollectionId = flashCard.CollectionId;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCardContentAsync(int flashCardId, CardContentEntity sourceContent, bool isAnswer)
        {
            if (sourceContent == null)
            {
                return false;
            }

            var flashCardEntity = await _dbContext.FlashCards
                .Include(f => f.Answer)
                .Include(f => f.Question)
                .SingleOrDefaultAsync(f => f.Id == flashCardId);

            if (flashCardEntity == null) 
            {
                return false;
            }

            var targetContent = isAnswer ? flashCardEntity.Answer : flashCardEntity.Question;

            if (targetContent == null)
            {
                targetContent = new CardContentEntity();
                if (isAnswer)
                {
                    flashCardEntity.Answer = targetContent;
                }
                else 
                {
                    flashCardEntity.Question = targetContent;
                } 
            }

            targetContent.PrimaryContent = sourceContent.PrimaryContent;
            targetContent.Text = sourceContent.Text;
            targetContent.ImagePath = sourceContent.ImagePath;
            targetContent.AudioPath = sourceContent.AudioPath;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFlashCardAsync(int flashCardId)
        {
            var flashCardEntity = await _dbContext.FlashCards
                .Include(f => f.Question)
                .Include(f => f.Answer)
                .SingleOrDefaultAsync(f => f.Id == flashCardId);

            if(flashCardEntity == null)
            {
                return true;
            }

            _dbContext.FlashCards.Remove(flashCardEntity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}