using Domain.DTOs;
using Domain.IServices;
using Domain.IRepositories;

namespace BLL.Services
{
    public class FlashCardService : IFlashCardService
    {
        private readonly IFlashCardRepository _flashCardRepository;

        public FlashCardService(IFlashCardRepository flashCardRepository)
        {
            _flashCardRepository = flashCardRepository;
        }

        public async Task<FlashCardDto> RetrieveFlashCardByIdAsync(int flashCardId)
        {
            return await _flashCardRepository.RetrieveFlashCardByIdAsync(flashCardId);
        }
        public async Task<List<FlashCardDto>> RetrieveAllFlashCardsByCollectionIdAsync(int collectionId)
        {
            return await _flashCardRepository.RetrieveAllFlashCardsByCollectionIdAsync(collectionId);
        }
        public async Task<List<FlashCardDto>> RetrievePagedFlashCardsByCollectionIdAsync(int collectionId,int index, int amount)
        {
            return await _flashCardRepository.RetrievePagedFlashCardsByCollectionIdAsync(collectionId, index, amount);
        }
        public async Task<int> CreateOrUpdateFlashCardAsync(FlashCardDto flashCard)
        {
            return await _flashCardRepository.CreateOrUpdateFlashCardAsync(flashCard);
        }
        public async Task<int> UpdateAnswerAsync(int flashCardId, CardContentDto cardContent)
        {
            return await _flashCardRepository.UpdateAnswerAsync(flashCardId, cardContent);
        }
        public async Task<int> CopyFlashCard(int flashCardId, int collectionId)
        {
            return await _flashCardRepository.CopyFlashCard(flashCardId, collectionId);
        }
        public async Task<bool> DeleteFlashCardAsync(int flashCardId)
        {
            return await _flashCardRepository.DeleteFlashCardAsync(flashCardId);
        }
    }
}
