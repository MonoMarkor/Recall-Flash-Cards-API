using Domain.DTOs;

namespace Domain.IServices
{
    public interface IFlashCardService
    {
        Task<FlashCardDto> RetrieveFlashCardByIdAsync(int flashCardId);
        Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId);
        Task<List<FlashCardDto>> RetrieveSomeFlashCardsByCollectionIdAsync(int collectionId, int amount);
        Task<int> CreateOrUpdateFlashCardAsync(FlashCardDto flashCard);
        Task<int> UpdateAnswerAsync(int flashCardId, CardContentDto cardContent);
        Task<bool> DeleteFlashCardAsync(int flashCardId);
    }
}
