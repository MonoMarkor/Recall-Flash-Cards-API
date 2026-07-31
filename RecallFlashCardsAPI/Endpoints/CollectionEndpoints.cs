using Domain.DTOs;
using RecallFlashCardsAPI.Models;
using Domain.IServices;
using Microsoft.AspNetCore.Mvc;
using Google.GenAI.Types;

namespace RecallFlashCardsAPI.Endpoints
{
    public class CollectionEndpoints
    {
        public static async Task<IResult> CreateCollectionAsync([FromBody] Collection collection, ICollectionService collectionService)
        {
            var collectionDto = new CollectionDto();
            collectionDto.Name = collection.Name;
            collectionDto.Description = collection.Description;
            int id = await collectionService.CreateCollectionAsync(collectionDto);
            return Results.CreatedAtRoute("CollectionId", new { Id = id });
        }

        public static async Task<IResult> UpdateCollectionNameAsync([FromBody] int collectionId, [FromBody] string name, ICollectionService collectionService)
        {
            int rows = await collectionService.UpdateCollectionNameAsync(collectionId, name);
            if (rows == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        }

        public static async Task<IResult> SafelyDeleteAsync([FromQuery] int collectionId, ICollectionService collectionService)
        {
            int numFlashCards = await collectionService.SafeDeleteCollectionAsync(collectionId);
            return Results.Ok(numFlashCards);
        }
        public static async Task<IResult> DeleteAllFlashCardsOfCollectionAsync([FromQuery] int collectionId, ICollectionService collectionService)
        {
            if (await collectionService.DeleteCollectionAsync(collectionId))
            {
                return Results.Ok();
            }
            return Results.NotFound();
        }
        public static async Task<IResult> DeleteCollectionAsync([FromQuery] int collectionId, ICollectionService collectionService)
        {
            if (await collectionService.DeleteAllFlashCardsOfCollectionAsync(collectionId))
            {
                return Results.Ok();
            }
            return Results.NotFound();
        }
    }
}