using Domain.DTOs;

namespace Domain.IServices
{
    public interface IFlashCardService
    {
        Task<FlashCardDto> RetrieveFlashCardByIdAsync(string flashCardId);
        Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(string collectionId);
        Task<List<FlashCardDto>> RetrieveSomeFlashCardsByCollectionIdAsync(string collectionId, int amount);
        Task<int> CreateOrUpdateAsync(FlashCardDto flashCard);
        Task<int> UpdateAnswerAsync(string flashCardId, CardContent cardContent);
        Task<int> UpdateCollectionNameAsync(int collectionId, string name);
        Task<bool> DeleteFlashCardAsync(string flashCardId);
        Task<bool> DeleteCollectionAsync(string collectionId);
        Task<bool> DeleteAllFlashCardsOfCollectionAsync(string collectionId);
    }
}
