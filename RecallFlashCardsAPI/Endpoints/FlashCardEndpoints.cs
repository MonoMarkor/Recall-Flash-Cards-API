using RecallFlashCardsAPI.Mapper.Interfaces;
using RecallFlashCardsAPI.Models;
using Domain.DTOs;
using Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace RecallFlashCardsAPI.Endpoints
{
    public static class FlashCardEndpoints
    {
        public static async Task<IResult> GetFlashCardAsync([FromBody] int id , IFlashCardMapper mapper, IFlashCardService flashCardService)
        {
            FlashCardDto flashCardDto = await flashCardService.RetrieveFlashCardByIdAsync(id);
            FlashCard flashCard = mapper.Map(flashCardDto);
            return Results.Ok(flashCard);
        }

        public static async Task<IResult> PostFlashCardAsync([FromBody] FlashCard flashCard, IFlashCardMapper mapper, IFlashCardService flashCardService)
        {
            FlashCardDto flashCardDto = mapper.Map(flashCard);
            int id = await flashCardService.CreateOrUpdateFlashCardAsync(flashCardDto);
            if (id <= 0)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Results.CreatedAtRoute("flashcardById", new { Id = id });
        }

        public static async Task<IResult> UpdateFlashCardAsync([FromBody] FlashCard flashCard, IFlashCardMapper mapper, IFlashCardService flashCardService)
        {
            FlashCardDto flashCardDto = mapper.Map(flashCard);
            int id = await flashCardService.CreateOrUpdateFlashCardAsync(flashCardDto);
            if (id <= 0)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Results.Ok();
        }

        public static async Task<IResult> UpdateCardContentAsync([FromBody] CardContent cardContent, [FromBody] int id, [FromBody] bool isAnswer, IFlashCardMapper mapper, IFlashCardService flashCardService)
        {
            CardContentDto cardContentDto = mapper.Map(cardContent);
            id = await flashCardService.UpdateCardContentAsync(id, cardContentDto, isAnswer);
            if (id <= 0)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Results.Ok();
        }

        public static async Task<IResult> GetAllFlashCardsAsync([FromQuery] int id, IFlashCardMapper mapper, IFlashCardService flashCardService)
        {
            var cardDtos = await flashCardService.RetrieveAllFlashCardsByCollectionIdAsync(id);
            var cards = mapper.Map(cardDtos);
            return Results.Ok(cards);
        }

        public static async Task<IResult> GetPagedFlashCardsAsync([FromQuery] int id, [FromQuery] int index, [FromQuery] int range, IFlashCardMapper mapper, IFlashCardService flashCardService)
        {
            var cardDtos = await flashCardService.RetrievePagedFlashCardsByCollectionIdAsync(id, index, range);
            var cards = mapper.Map(cardDtos);
            return Results.Ok(cards);
        }

        public static async Task<IResult> CopyFlashCardAsync([FromBody] int cardId, [FromBody] int collectionId, IFlashCardService flashCardService)
        {
            int newCardId = await flashCardService.CopyFlashCardAsync(cardId, collectionId);

            if (newCardId <= 0)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Results.CreatedAtRoute("flashcardById", new { Id = newCardId });
        }

        public static async Task<IResult> DeleteFlashCardAsync([FromBody] int id, IFlashCardService flashCardService)
        {
            if (await flashCardService.DeleteFlashCardAsync(id))
            {
                return Results.NoContent();
            }
            return Results.NotFound();
        }
    }
}
