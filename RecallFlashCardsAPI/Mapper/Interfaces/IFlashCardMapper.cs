using Domain.DTOs;
using RecallFlashCardsAPI.Models;

namespace RecallFlashCardsAPI.Mapper.Interfaces
{
    public interface IFlashCardMapper
    {
        public FlashCardDto Map(FlashCard flashCard);
        public FlashCard Map(FlashCardDto flashCardDto);
        public List<FlashCardDto> Map(List<FlashCard> flashCards);
        public List<FlashCard> Map(List<FlashCardDto> flashCardDtos);
        public CardContentDto Map(CardContent cardContent);
    }
}
