using Domain.DTOs;
using Domain.IRepositories;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.Repositories
{
    public class FlashCardRepository : IFlashCardRepository
    {

        private readonly PostgreSQLDbContext _dbContext;

        public FlashCardRepository(PostgreSQLDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<FlashCardDto> RetrieveFlashCardByIdAsync(int flashCardId)
        {
            var flashCardEntity = await _dbContext.FlashCards.SingleOrDefaultAsync(f => f.Id == flashCardId);
            if (flashCardEntity == null)
            {
                throw new KeyNotFoundException($"FlashCard with ID {flashCardId} was not found.");
            }
            var flashCardDto = new FlashCardDto
            {
                Id = flashCardEntity.Id,
                ExpirationDate = flashCardEntity.ExpirationDate,
                Difficulty = (FlashCardDto.DifficultyLevel)flashCardEntity.Difficulty,
                CollectionId = flashCardEntity.CollectionId
            };
            return flashCardDto;
        }
        public async Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId)
        {
            var FlashCardEntites = await _dbContext.FlashCards
                                        .Where(fc => fc.CollectionId == collectionId)
                                        .ToListAsync();

            var FlashCardDtos = FlashCardEntites.Select(fc => new FlashCardDto
            {
                Id = fc.Id,
                ExpirationDate = fc.ExpirationDate,
                Difficulty = (FlashCardDto.DifficultyLevel)fc.Difficulty,
                CollectionId = collectionId
            }).ToList();

            return FlashCardDtos;
        }
        public async Task<List<FlashCardDto>> RetrieveSomeFlashCardsByCollectionIdAsync(int collectionId, int amount)
        {
            var FlashCardEntites = await _dbContext.FlashCards
                                        .Where(fc => fc.CollectionId == collectionId)
                                        .Take(amount)
                                        .ToListAsync();

            var FlashCardDtos = FlashCardEntites.Select(fc => new FlashCardDto
            {
                Id = fc.Id,
                ExpirationDate = fc.ExpirationDate,
                Difficulty = (FlashCardDto.DifficultyLevel)fc.Difficulty,
                CollectionId = collectionId
            }).ToList();

            return FlashCardDtos;
        }
        public async Task<int> CreateOrUpdateFlashCardAsync(FlashCardDto flashCard)
        {
            FlashCardEntity? flashCardEntity = null;

            if (flashCard.Id > 0)
            {
                flashCardEntity = await _dbContext.FlashCards.FindAsync(flashCard.Id);
            }

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
        public async Task<int> UpdateAnswerAsync(int flashCardId, CardContentDto cardContent)
        {
            return 0;
        }

        public async Task<int> CopyFlashCard(int flashCardId, int collectionId)
        {
            var flashCardEntity = await _dbContext.FlashCards.AsNoTracking().SingleOrDefaultAsync(f => f.Id == flashCardId);
            
            if (flashCardEntity == null)
            {
                throw new KeyNotFoundException($"FlashCard with ID {flashCardId} was not found.");
            }

            var newFlashCard = new FlashCardEntity
            {
                CollectionId = collectionId
            };

            _dbContext.FlashCards.Add(newFlashCard);
            await _dbContext.SaveChangesAsync();
            return newFlashCard.Id;
        }
        public async Task<bool> DeleteFlashCardAsync(int flashCardId)
        {
            await _dbContext.FlashCards.Where(fc => fc.Id == flashCardId).ExecuteDeleteAsync();
            return true;
        }
    }
}
