using Domain.DTOs;
using Domain.IRepositories;
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
        private readonly IMinioService _minioService;
        private readonly string _imageBucket;
        private readonly string _audioBucket;

        public FlashCardRepository(PostgreSQLDbContext dbContext, IMinioService minioService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _minioService = minioService;
            _imageBucket = configuration["Minio:ImageBucket"];
            _audioBucket = configuration["Minio:AudioBucket"];
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
                CollectionId = flashCardEntity.CollectionId,
            };
            if (flashCardEntity.Answer != null)
            {
                flashCardDto.Answer.PrimaryContent = (CardContentDto.PrimaryContentType)flashCardEntity.Answer.PrimaryContent;
                switch (flashCardEntity.Answer.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Text:
                        flashCardDto.Answer.Text = flashCardEntity.Answer.Text;
                        break;
                    case CardContentEntity.PrimaryContentType.Image:
                        flashCardDto.Answer.ImageBytes = await _minioService.GetFileAsync(_imageBucket, flashCardEntity.Answer.ImagePath);
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        flashCardDto.Answer.AudioBytes = await _minioService.GetFileAsync(_audioBucket, flashCardEntity.Answer.AudioPath);
                        break;
                    default:
                        break;
                }
            }
            if (flashCardEntity.Question != null)
            {
                flashCardDto.Question.PrimaryContent = (CardContentDto.PrimaryContentType)flashCardEntity.Question.PrimaryContent;
                switch (flashCardEntity.Question.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Text:
                        flashCardDto.Question.Text = flashCardEntity.Question.Text;
                        break;
                    case CardContentEntity.PrimaryContentType.Image:
                        flashCardDto.Question.ImageBytes = await _minioService.GetFileAsync(_imageBucket, flashCardEntity.Question.ImagePath);
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        flashCardDto.Question.AudioBytes = await _minioService.GetFileAsync(_audioBucket, flashCardEntity.Question.AudioPath);
                        break;
                    default:
                        break;
                }
            }
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
