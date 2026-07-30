using Domain.DTOs;
using Domain.IServices;
using Infrastructure.SQL.Database.Entities;
using Infrastructure.SQL.IRepositories;
using Microsoft.Extensions.Configuration;

namespace BLL.Services
{
    public class FlashCardService : IFlashCardService
    {
        private readonly IFlashCardRepository _flashCardRepository;
        private readonly IMinioService _minioService;
        private readonly string _imageBucket;
        private readonly string _audioBucket;

        public FlashCardService(IFlashCardRepository flashCardRepository, IMinioService minioService, IConfiguration configuration)
        {
            _flashCardRepository = flashCardRepository;
            _minioService = minioService;
            _imageBucket = configuration["Minio:ImageBucket"]
                ?? throw new InvalidOperationException("Minio:ImageBucket configuration is missing."); ;
            _audioBucket = configuration["Minio:AudioBucket"]
                ?? throw new InvalidOperationException("Minio:AudioBucket configuration is missing."); ;
        }

        private static string GetAnswerImagePath(int id) => $"{id}-Answer-Image";
        private static string GetAnswerAudioPath(int id) => $"{id}-Answer-Audio";
        private static string GetQuestionImagePath(int id) => $"{id}-Question-Image";
        private static string GetQuestionAudioPath(int id) => $"{id}-Question-Audio";

        public async Task<FlashCardDto> RetrieveFlashCardByIdAsync(int flashCardId)
        {
            var flashCardEntity = await _flashCardRepository.RetrieveFlashCardByIdAsync(flashCardId);

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
                        if (flashCardEntity.Answer.ImagePath != null)
                        {
                            flashCardDto.Answer.ImageBytes = await _minioService.GetFileAsync(_imageBucket, flashCardEntity.Answer.ImagePath);
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCardEntity.Answer?.AudioPath != null)
                        {
                            flashCardDto.Answer.AudioBytes = await _minioService.GetFileAsync(_audioBucket, flashCardEntity.Answer.AudioPath);
                        }
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
                        if (flashCardEntity.Question.ImagePath != null)
                        {
                            flashCardDto.Question.ImageBytes = await _minioService.GetFileAsync(_imageBucket, flashCardEntity.Question.ImagePath);
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCardEntity.Question.AudioPath != null)
                        {
                            flashCardDto.Question.AudioBytes = await _minioService.GetFileAsync(_audioBucket, flashCardEntity.Question.AudioPath);
                        }
                        break;
                    default:
                        break;
                }
            }

            return flashCardDto;
        }
        public async Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId)
        {
            var flashCardEntities = await _flashCardRepository.RetrieveAllFlashCardsByCollectionIdAsync(collectionId);

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
        public async Task<List<FlashCardDto>> RetrievePagedFlashCardsByCollectionIdAsync(int collectionId,int index, int amount)
        {
            var flashCardEntities = await _flashCardRepository.RetrievePagedFlashCardsByCollectionIdAsync(collectionId, index, amount);

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

            int flashCardId;
            if (flashCard.Id == 0)
            {
                flashCardId = await _flashCardRepository.CreateFlashCardAsync(flashCard);
            } else
            {
                if(!await _flashCardRepository.UpdateFlashCardAsync(flashCard))
                {
                    return 0;
                }
                flashCardId = flashCard.Id;
            }

            FlashCardEntity flashCardEntity = await _flashCardRepository.RetrieveFlashCardByIdAsync(flashCardId);
            if (flashCardEntity == null) return 0;

            await ProcessCardContentAsync(
                flashCardId,
                flashCard.Question,
                flashCardEntity.Question,
                GetQuestionImagePath,
                GetQuestionAudioPath,
                isAnswer: false
            );

            await ProcessCardContentAsync(
                flashCardId,
                flashCard.Answer,
                flashCardEntity.Answer,
                GetAnswerImagePath,
                GetAnswerAudioPath,
                isAnswer: true
            );

            return flashCardId;
        }
        private async Task ProcessCardContentAsync(
            int flashCardId,
            CardContentDto? incomingDto,
            CardContentEntity? existingEntity,
            Func<int, string> getImagePath,
            Func<int, string> getAudioPath,
            bool isAnswer)
        {
            if (incomingDto == null) return;

            if (existingEntity == null)
            {
                existingEntity = new CardContentEntity();
            }
            else
            {
                if (incomingDto.PrimaryContent != (CardContentDto.PrimaryContentType)existingEntity.PrimaryContent)
                {
                    switch (existingEntity.PrimaryContent)
                    {
                        case CardContentEntity.PrimaryContentType.Text:
                            existingEntity.Text = null;
                            break;
                        case CardContentEntity.PrimaryContentType.Image when existingEntity.ImagePath != null:
                            await _minioService.DeleteFileAsync(_imageBucket, existingEntity.ImagePath);
                            existingEntity.ImagePath = null;
                            break;
                        case CardContentEntity.PrimaryContentType.Audio when existingEntity.AudioPath != null:
                            await _minioService.DeleteFileAsync(_audioBucket, existingEntity.AudioPath);
                            existingEntity.AudioPath = null;
                            break;
                    }
                }
            }

            switch (incomingDto.PrimaryContent)
            {
                case CardContentDto.PrimaryContentType.Text:
                    existingEntity.PrimaryContent = CardContentEntity.PrimaryContentType.Text;
                    existingEntity.Text = incomingDto.Text;
                    break;

                case CardContentDto.PrimaryContentType.Image:
                    existingEntity.PrimaryContent = CardContentEntity.PrimaryContentType.Image;
                    if (incomingDto.ImageBytes != null)
                    {
                        string path = getImagePath(flashCardId);
                        await _minioService.UploadOrUpdateFileAsync(_imageBucket, path, incomingDto.ImageBytes);
                        existingEntity.ImagePath = path;
                    }
                    break;

                case CardContentDto.PrimaryContentType.Audio:
                    existingEntity.PrimaryContent = CardContentEntity.PrimaryContentType.Audio;
                    if (incomingDto.AudioBytes != null)
                    {
                        string path = getAudioPath(flashCardId);
                        await _minioService.UploadOrUpdateFileAsync(_audioBucket, path, incomingDto.AudioBytes);
                        existingEntity.AudioPath = path;
                    }
                    break;
            }

            await _flashCardRepository.UpdateCardContentAsync(flashCardId, existingEntity, isAnswer);
        }
        public async Task<int> UpdateCardContentAsync(int flashCardId, CardContentDto cardContent, bool isAnswer)
        {

            FlashCardEntity flashCardEntity = await _flashCardRepository.RetrieveFlashCardByIdAsync(flashCardId);
            if (flashCardEntity == null) return 0;

            if (isAnswer)
            {
                await ProcessCardContentAsync(
                    flashCardId,
                    cardContent,
                    flashCardEntity.Answer,
                    GetAnswerImagePath,
                    GetAnswerAudioPath,
                    isAnswer: true
                );
            } else
            {
                await ProcessCardContentAsync(
                    flashCardId,
                    cardContent,
                    flashCardEntity.Question,
                    GetQuestionImagePath,
                    GetQuestionAudioPath,
                    isAnswer: false
                );
            }

            return flashCardId;
        }
        public async Task<int> CopyFlashCardAsync(int flashCardId, int collectionId)
        {
            FlashCardEntity flashCardEntity = await _flashCardRepository.RetrieveFlashCardByIdAsync(flashCardId);
            if (flashCardEntity == null)
            {
                throw new KeyNotFoundException($"FlashCard with ID {flashCardId} was not found.");
            }

            var newFlashCard = new FlashCardEntity
            {
                CollectionId = collectionId,
                ExpirationDate = flashCardEntity.ExpirationDate,
                Difficulty = flashCardEntity.Difficulty
            };
            if (flashCardEntity.Answer != null) newFlashCard.Answer = new CardContentEntity();
            if (flashCardEntity.Question != null) newFlashCard.Question = new CardContentEntity();

            if (flashCardEntity.Answer != null)
            {
                newFlashCard.Answer.PrimaryContent = flashCardEntity.Answer.PrimaryContent;
                switch (flashCardEntity.Answer.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Text:
                        if (flashCardEntity.Answer.Text != null)
                        {
                            newFlashCard.Answer.Text = flashCardEntity.Answer.Text;
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Image:
                        if (flashCardEntity.Answer.ImagePath != null)
                        {
                            await _minioService.CopyFileInSameBucketAsync(_imageBucket, flashCardEntity.Answer.ImagePath, GetAnswerImagePath(newFlashCard.Id));
                            newFlashCard.Answer.ImagePath = GetAnswerImagePath(newFlashCard.Id);
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCardEntity.Answer.AudioPath != null)
                        {
                            await _minioService.CopyFileInSameBucketAsync(_audioBucket, flashCardEntity.Answer.AudioPath, GetAnswerAudioPath(newFlashCard.Id));
                            newFlashCard.Answer.AudioPath = GetAnswerAudioPath(newFlashCard.Id);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (flashCardEntity.Question != null)
            {
                newFlashCard.Question.PrimaryContent = flashCardEntity.Question.PrimaryContent;
                switch (flashCardEntity.Question.PrimaryContent)
                {
                    case CardContentEntity.PrimaryContentType.Text:
                        if (flashCardEntity.Question.Text != null)
                        {
                            newFlashCard.Question.Text = flashCardEntity.Question.Text;
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Image:
                        if (flashCardEntity.Question.ImagePath != null)
                        {
                            await _minioService.CopyFileInSameBucketAsync(_imageBucket, flashCardEntity.Question.ImagePath, GetQuestionImagePath(newFlashCard.Id));
                            newFlashCard.Question.ImagePath = GetQuestionImagePath(newFlashCard.Id);
                        }
                        break;
                    case CardContentEntity.PrimaryContentType.Audio:
                        if (flashCardEntity.Question.AudioPath != null)
                        {
                            await _minioService.CopyFileInSameBucketAsync(_audioBucket, flashCardEntity.Question.AudioPath, GetQuestionAudioPath(newFlashCard.Id));
                            newFlashCard.Question.AudioPath = GetQuestionAudioPath(newFlashCard.Id);
                        }
                        break;
                    default:
                        break;
                }
            }

            int newFlashCardId = await _flashCardRepository.CreateFlashCardAsync(newFlashCard);
            return newFlashCardId;
        }
        public async Task<bool> DeleteFlashCardAsync(int flashCardId)
        {
            
            FlashCardEntity flashCardEntity = await _flashCardRepository.RetrieveFlashCardByIdAsync(flashCardId);
            if (flashCardEntity == null) return true;

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

            return await _flashCardRepository.DeleteFlashCardAsync(flashCardId);
        }
    }
}
