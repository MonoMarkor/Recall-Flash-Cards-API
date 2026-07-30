using Domain.DTOs;
using Infrastructure.SQL.Database.Entities;

namespace Infrastructure.SQL.IRepositories
{
    public interface IFlashCardRepository
    {
        Task<FlashCardEntity> RetrieveFlashCardByIdAsync(int flashCardId);
        Task<List<FlashCardEntity>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId);
        Task<List<FlashCardEntity>> RetrievePagedFlashCardsByCollectionIdAsync(int collectionId, int index ,int amount);
        Task<int> CreateFlashCardAsync(FlashCardDto flashCard);
        Task<int> CreateFlashCardAsync(FlashCardEntity flashCardEntity);
        Task<bool> UpdateFlashCardAsync(FlashCardDto flashCard);
        Task<bool> UpdateCardContentAsync(int flashCardId, CardContentEntity cardContentEntity, bool isAnswer);
        Task<bool> DeleteFlashCardAsync(int flashCardId);
    }
}
