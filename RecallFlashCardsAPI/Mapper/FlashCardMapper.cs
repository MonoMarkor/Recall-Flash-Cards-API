using Domain.DTOs;
using RecallFlashCardsAPI.Models;
using RecallFlashCardsAPI.Mapper.Interfaces;

namespace RecallFlashCardsAPI.Mapper
{
    public class FlashCardMapper : IFlashCardMapper
    {
        public FlashCardDto Map(FlashCard flashCard)
        {
            FlashCardDto flashCardDto = new FlashCardDto
            {
                Id = flashCard.Id,
                Question = new CardContentDto
                {
                    Text = flashCard.Question.Text,
                    ImageBytes = flashCard.Question.ImageBase64 != null ? Convert.FromBase64String(flashCard.Question.ImageBase64) : null,
                    AudioBytes = flashCard.Question.AudioBase64 != null ? Convert.FromBase64String(flashCard.Question.AudioBase64) : null
                },
                Answer = new CardContentDto
                {
                    Text = flashCard.Answer.Text,
                    ImageBytes = flashCard.Answer.ImageBase64 != null ? Convert.FromBase64String(flashCard.Answer.ImageBase64) : null,
                    AudioBytes = flashCard.Answer.AudioBase64 != null ? Convert.FromBase64String(flashCard.Answer.AudioBase64) : null
                },
                ExpirationDate = flashCard.ExpirationDate,
                Difficulty = (FlashCardDto.DifficultyLevel)flashCard.Difficulty,
                CollectionId = flashCard.CollectionId
            };

            return flashCardDto;
        }
        public FlashCard Map(FlashCardDto flashCardDto)
        {
            FlashCard flashCard = new FlashCard
            {
                Id = flashCardDto.Id,
                Question = new CardContent
                {
                    Text = flashCardDto.Question.Text,
                    ImageBase64 = flashCardDto.Question.ImageBytes != null ? Convert.ToBase64String(flashCardDto.Question.ImageBytes) : null,
                    AudioBase64 = flashCardDto.Question.AudioBytes != null ? Convert.ToBase64String(flashCardDto.Question.AudioBytes) : null
                },
                Answer = new CardContent
                {
                    Text = flashCardDto.Answer.Text,
                    ImageBase64 = flashCardDto.Answer.ImageBytes != null ? Convert.ToBase64String(flashCardDto.Answer.ImageBytes) : null,
                    AudioBase64 = flashCardDto.Answer.AudioBytes != null ? Convert.ToBase64String(flashCardDto.Answer.AudioBytes) : null
                },
                ExpirationDate = flashCardDto.ExpirationDate,
                Difficulty = (FlashCard.DifficultyLevel)flashCardDto.Difficulty,
                CollectionId = flashCardDto.CollectionId
            };

            return flashCard;
        }
        public List<FlashCardDto> Map(List<FlashCard>? flashCards)
        {
            if (flashCards == null) return new List<FlashCardDto>();

            return flashCards
                .Where(fc => fc != null)
                .Select(fc => Map(fc))   
                .ToList();
        }

        public List<FlashCard> Map(List<FlashCardDto>? flashCardDtos)
        {
            if (flashCardDtos == null) return new List<FlashCard>();

            return flashCardDtos
                .Where(dto => dto != null)
                .Select(dto => Map(dto))  
                .ToList();
        }

        public CardContentDto Map(CardContent cardContent)
        {
            CardContentDto cardContentDto = new CardContentDto
            {
                Text = cardContent.Text,
                ImageBytes = cardContent.ImageBase64 != null ? Convert.FromBase64String(cardContent.ImageBase64) : null,
                AudioBytes = cardContent.AudioBase64 != null ? Convert.FromBase64String(cardContent.AudioBase64) : null
            };

            return cardContentDto;
        }
    }
}
