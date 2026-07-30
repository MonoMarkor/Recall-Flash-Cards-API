using Domain.DTOs;

namespace Domain.IServices
{
    public interface IFlashCardService
    {
        Task<FlashCardDto> RetrieveFlashCardByIdAsync(int flashCardId);
        Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId);
        Task<List<FlashCardDto>> RetrievePagedFlashCardsByCollectionIdAsync(int collectionId, int index, int amount);
        Task<int> CreateOrUpdateFlashCardAsync(FlashCardDto flashCard);
        Task<int> UpdateCardContentAsync(int flashCardId, CardContentDto cardContent, bool isAnswer);
        Task<int> CopyFlashCardAsync(int flashCardId, int collectionId);
        Task<bool> DeleteFlashCardAsync(int flashCardId);
    }
}
