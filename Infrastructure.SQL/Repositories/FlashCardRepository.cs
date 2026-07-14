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
            var flashCardEntity = await _dbContext.FlashCards
                .Include(f => f.Question)
                .Include(f => f.Answer)
                .SingleOrDefaultAsync(f => f.Id == flashCardId);
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
                flashCardDto.Answer = new CardContentDto();
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
                flashCardDto.Question = new CardContentDto();
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
            var flashCardEntities = await _dbContext.FlashCards
                                        .Where(fc => fc.CollectionId == collectionId)
                                        .Include(fc => fc.Question)
                                        .Include(fc => fc.Answer)
                                        .OrderBy(fc => fc.Id)
                                        .ToListAsync();

            var mapTasks = flashCardEntities.Select(async entity =>
            {
                var dto = new FlashCardDto
                {
                    Id = entity.Id,
                    ExpirationDate = entity.ExpirationDate,
                    Difficulty = (FlashCardDto.DifficultyLevel)entity.Difficulty,
                    CollectionId = collectionId
                };

                if (entity.Answer != null)
                {
                    dto.Answer = new CardContentDto
                    {
                        PrimaryContent = (CardContentDto.PrimaryContentType)entity.Answer.PrimaryContent
                    };

                    switch (entity.Answer.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Text:
                            dto.Answer.Text = entity.Answer.Text;
                            break;
                        case CardContentEntity.PrimaryContentType.Image:
                            if (entity.Answer.ImagePath != null)
                            {
                                dto.Answer.ImageBytes = await _minioService.GetFileAsync(_imageBucket, entity.Answer.ImagePath);
                            }
                            break;
                        case CardContentEntity.PrimaryContentType.Audio:
                            if (entity.Answer.AudioPath != null)
                            {
                                dto.Answer.AudioBytes = await _minioService.GetFileAsync(_audioBucket, entity.Answer.AudioPath);
                            }
                            break;
                    }
                }

                if (entity.Question != null)
                {
                    dto.Question = new CardContentDto
                    {
                        PrimaryContent = (CardContentDto.PrimaryContentType)entity.Question.PrimaryContent
                    };

                    switch (entity.Question.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Text:
                            dto.Question.Text = entity.Question.Text;
                            break;
                        case CardContentEntity.PrimaryContentType.Image:
                            if (entity.Question.ImagePath != null)
                            {
                                dto.Question.ImageBytes = await _minioService.GetFileAsync(_imageBucket, entity.Question.ImagePath);
                            }
                            break;
                        case CardContentEntity.PrimaryContentType.Audio:
                            if (entity.Question.AudioPath != null)
                            {
                                dto.Question.AudioBytes = await _minioService.GetFileAsync(_audioBucket, entity.Question.AudioPath);
                            }
                            break;
                    }
                }

                return dto;
            });

            var flashCardDtos = await Task.WhenAll(mapTasks);

            return flashCardDtos.ToList();
        }

        public async Task<List<FlashCardDto>> RetrievePagedFlashCardsByCollectionIdAsync(int collectionId, int index, int amount)
        {

            var flashCardEntities = await _dbContext.FlashCards
                                        .Where(fc => fc.CollectionId == collectionId)
                                        .Include(fc => fc.Question)
                                        .Include(fc => fc.Answer)
                                        .OrderBy(fc => fc.Id)
                                        .Skip(index)
                                        .Take(amount)
                                        .ToListAsync();

            var mapTasks = flashCardEntities.Select(async entity =>
            {
                var dto = new FlashCardDto
                {
                    Id = entity.Id,
                    ExpirationDate = entity.ExpirationDate,
                    Difficulty = (FlashCardDto.DifficultyLevel)entity.Difficulty,
                    CollectionId = collectionId
                };

                if (entity.Answer != null)
                {
                    dto.Answer = new CardContentDto
                    {
                        PrimaryContent = (CardContentDto.PrimaryContentType)entity.Answer.PrimaryContent
                    };

                    switch (entity.Answer.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Text:
                            dto.Answer.Text = entity.Answer.Text;
                            break;
                        case CardContentEntity.PrimaryContentType.Image:
                            if (entity.Answer.ImagePath != null)
                            {
                                dto.Answer.ImageBytes = await _minioService.GetFileAsync(_imageBucket, entity.Answer.ImagePath);
                            }
                            break;
                        case CardContentEntity.PrimaryContentType.Audio:
                            if (entity.Answer.AudioPath != null)
                            {
                                dto.Answer.AudioBytes = await _minioService.GetFileAsync(_audioBucket, entity.Answer.AudioPath);
                            }
                            break;
                    }
                }

                if (entity.Question != null)
                {
                    dto.Question = new CardContentDto
                    {
                        PrimaryContent = (CardContentDto.PrimaryContentType)entity.Question.PrimaryContent
                    };

                    switch (entity.Question.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Text:
                            dto.Question.Text = entity.Question.Text;
                            break;
                        case CardContentEntity.PrimaryContentType.Image:
                            if (entity.Question.ImagePath != null)
                            {
                                dto.Question.ImageBytes = await _minioService.GetFileAsync(_imageBucket, entity.Question.ImagePath);
                            }
                            break;
                        case CardContentEntity.PrimaryContentType.Audio:
                            if (entity.Question.AudioPath != null)
                            {
                                dto.Question.AudioBytes = await _minioService.GetFileAsync(_audioBucket, entity.Question.AudioPath);
                            }
                            break;
                    }
                }

                return dto;
            });

            var flashCardDtos = await Task.WhenAll(mapTasks);

            return flashCardDtos.ToList();
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

            if (flashCard.Answer != null)
            {
                if (flashCardEntity.Answer == null)
                {
                    flashCardEntity.Answer = new CardContentEntity();
                }
                flashCardEntity.Answer.PrimaryContent = (CardContentEntity.PrimaryContentType)flashCard.Answer.PrimaryContent;
                flashCard.Answer.PrimaryContent = (CardContentDto.PrimaryContentType)flashCardEntity.Answer.PrimaryContent;
                switch (flashCardEntity.Answer.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Text:
                        if (flashCard.Answer.Text != null)
                        {
                            flashCardEntity.Answer.Text = flashCard.Answer.Text;
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Image:
                        if (flashCard.Answer.ImageBytes != null)
                        {
                            await _minioService.UploadOrUpdateFileAsync(_imageBucket, $"{flashCardEntity.Id}-Answer-Image", flashCard.Answer.ImageBytes);
                            flashCardEntity.Answer.ImagePath = $"{flashCardEntity.Id}-Answer-Image";
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if(flashCard.Answer.AudioBytes != null)
                        {
                            await _minioService.UploadOrUpdateFileAsync(_audioBucket, $"{flashCardEntity.Id}-Answer-Audio", flashCard.Answer.AudioBytes);
                            flashCardEntity.Answer.AudioPath = $"{flashCardEntity.Id}-Answer-Audio";
                        }
                        break;
                    default:
                        break;
                }
            }

            if (flashCard.Question != null)
            {
                if (flashCardEntity.Question == null)
                {
                    flashCardEntity.Question = new CardContentEntity();
                }
                flashCardEntity.Question.PrimaryContent = (CardContentEntity.PrimaryContentType)flashCard.Question.PrimaryContent;
                flashCard.Question.PrimaryContent = (CardContentDto.PrimaryContentType)flashCardEntity.Question.PrimaryContent;
                switch (flashCardEntity.Question.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Text:
                        if (flashCard.Question.Text != null)
                        {
                            flashCardEntity.Question.Text = flashCard.Question.Text;
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Image:
                        if (flashCard.Question.ImageBytes != null)
                        {
                            await _minioService.UploadOrUpdateFileAsync(_imageBucket, $"{flashCardEntity.Id}-Question-Image", flashCard.Question.ImageBytes);
                            flashCardEntity.Question.ImagePath = $"{flashCardEntity.Id}-Question-Image";
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCard.Question.AudioBytes != null)
                        {
                            await _minioService.UploadOrUpdateFileAsync(_audioBucket, $"{flashCardEntity.Id}-Question-Audio", flashCard.Question.AudioBytes);
                            flashCardEntity.Question.AudioPath = $"{flashCardEntity.Id}-Question-Audio";
                        }
                        break;
                    default:
                        break;
                }
            }

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
                // Copy the Card Content Files as well
            };

            _dbContext.FlashCards.Add(newFlashCard);
            await _dbContext.SaveChangesAsync();
            return newFlashCard.Id;
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

            if (flashCardEntity.Answer != null)
            {
                switch (flashCardEntity.Answer.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Image:
                        if (flashCardEntity.Answer.ImagePath != null)
                        {
                            await _minioService.DeleteFileAsync(_imageBucket, flashCardEntity.Answer.ImagePath);
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCardEntity.Answer.AudioPath != null)
                        {
                            await _minioService.DeleteFileAsync(_audioBucket, flashCardEntity.Answer.AudioPath);
                        }
                        break;
                }
            }
            if (flashCardEntity.Question != null)
            {
                switch (flashCardEntity.Question.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Image:
                        if (flashCardEntity.Question.ImagePath != null)
                        {
                            await _minioService.DeleteFileAsync(_imageBucket, flashCardEntity.Question.ImagePath);
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCardEntity.Question.AudioPath != null)
                        {
                            await _minioService.DeleteFileAsync(_audioBucket, flashCardEntity.Question.AudioPath);
                        }
                        break;
                }
            }

            _dbContext.FlashCards.Remove(flashCardEntity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
