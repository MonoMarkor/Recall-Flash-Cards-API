using Domain.DTOs;

namespace Domain.IRepositories
{
    public interface IFlashCardRepository
    {
        Task<FlashCardDto> RetrieveFlashCardByIdAsync(string flashCardId);
        Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(string collectionId);
        Task<List<FlashCardDto>> RetrieveSomeFlashCardsByCollectionIdAsync(string collectionId, int amount);
        Task<int> CreateOrUpdateFlashCardAsync(FlashCardDto flashCard);
        Task<int> UpdateAnswerAsync(string flashCardId, CardContent cardContent);
        Task<bool> DeleteFlashCardAsync(string flashCardId);
    }
}
