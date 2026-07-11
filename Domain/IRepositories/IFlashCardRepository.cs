using Domain.DTOs;

namespace Domain.IRepositories
{
    public interface IFlashCardRepository
    {
        Task<FlashCardDto> RetrieveFlashCardByIdAsync(int flashCardId);
        Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId);
        Task<List<FlashCardDto>> RetrieveSomeFlashCardsByCollectionIdAsync(int collectionId, int amount);
        Task<int> CreateOrUpdateFlashCardAsync(FlashCardDto flashCard);
        Task<int> UpdateAnswerAsync(int flashCardId, CardContentDto cardContent);
        Task<int> CopyFlashCard(int flashCardId, int collectionId);
        Task<bool> DeleteFlashCardAsync(int flashCardId);
    }
}
