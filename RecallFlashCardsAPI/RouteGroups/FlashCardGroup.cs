using RecallFlashCardsAPI.Endpoints;

namespace RecallFlashCardsAPI.RouteGroups
{
    public static class FlashCardGroup
    {
        public static void AddFlashCardEndpoints(this WebApplication app, string fixedFileUploadPolicy)
        {
            var group = app.MapGroup("/flashcard");

            group.MapGet("/", FlashCardEndpoints.GetFlashCardAsync);
            group.MapGet("/all", FlashCardEndpoints.GetAllFlashCardsAsync);
            group.MapGet("/paged", FlashCardEndpoints.GetPagedFlashCardsAsync);
            group.MapPost("/", FlashCardEndpoints.PostFlashCardAsync).RequireRateLimiting(fixedFileUploadPolicy);
            group.MapPut("/", FlashCardEndpoints.UpdateFlashCardAsync).RequireRateLimiting(fixedFileUploadPolicy);
            group.MapPut("/cardcontent", FlashCardEndpoints.UpdateCardContentAsync).RequireRateLimiting(fixedFileUploadPolicy);
            group.MapPost("/copy", FlashCardEndpoints.CopyFlashCardAsync).RequireRateLimiting(fixedFileUploadPolicy);
            group.MapDelete("/", FlashCardEndpoints.DeleteFlashCardAsync);
        }
    }
}
