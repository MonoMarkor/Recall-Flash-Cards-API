using RecallFlashCardsAPI.Endpoints;

namespace RecallFlashCardsAPI.RouteGroups
{
    public static class FlashCardGroup
    {
        public static void AddFlashCardEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/flashcard");

            group.MapGet("/", FlashCardEndpoints.GetFlashCardAsync);
            group.MapGet("/all", FlashCardEndpoints.GetAllFlashCardsAsync);
            group.MapGet("/paged", FlashCardEndpoints.GetPagedFlashCardsAsync);
            group.MapPost("/", FlashCardEndpoints.PostFlashCardAsync);
            group.MapPut("/", FlashCardEndpoints.UpdateFlashCardAsync);
            group.MapPut("/cardcontent", FlashCardEndpoints.UpdateCardContentAsync);
            group.MapPost("/copy", FlashCardEndpoints.CopyFlashCardAsync);
            group.MapDelete("/", FlashCardEndpoints.DeleteFlashCardAsync);
        }
    }
}
