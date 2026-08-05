using Domain.DTOs;
using RecallFlashCardsAPI.Models;
using Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace RecallFlashCardsAPI.Endpoints
{
    public class CollectionEndpoints
    {
        public static async Task<IResult> CreateCollectionAsync([FromBody] Collection collection, ICollectionService collectionService)
        {
            if (collection == null || collection.Id != 0)
            {
                return Results.StatusCode(StatusCodes.Status400BadRequest);
            }

            var collectionDto = new CollectionDto()
            {
                UserId = collection.UserId,
                Name = collection.Name,
                Description = collection.Description
            };
            
            int id = await collectionService.CreateCollectionAsync(collectionDto);
            if (id == 0)
            {
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Results.CreatedAtRoute("CollectionId", new { Id = id });
        }

        public static async Task<IResult> GetCollectionsOfUserAsync([FromQuery] int userId, ICollectionService collectionService)
        {
            List<CollectionDto> collections = await collectionService.GetAllCollectionsOfUserAsync(userId);
            if (collections != null)
            {
                return Results.Ok(collections);
            }
            return Results.NotFound();
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
        public static async Task<IResult> UpdateCollectionDescriptionAsync([FromBody] int collectionId, [FromBody] string desc, ICollectionService collectionService)
        {
            int rows = await collectionService.UpdateCollectionDescriptionAsync(collectionId, desc);
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