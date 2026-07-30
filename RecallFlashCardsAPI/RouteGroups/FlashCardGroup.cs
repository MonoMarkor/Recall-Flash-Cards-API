using RecallFlashCardsAPI.Endpoints;

namespace RecallFlashCardsAPI.RouteGroups
{
    public static class FlashCardGroup
    {
        public static void AddFlashCardEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/flashcards");

            group.MapGet("/", FlashCardEndpoints.GetFlashCard);
            group.MapGet("/all", FlashCardEndpoints.GetAllFlashCards);
            group.MapGet("/paged", FlashCardEndpoints.GetPagedFlashCards);
            group.MapPost("/", FlashCardEndpoints.PostFlashCard);
            group.MapPut("/", FlashCardEndpoints.UpdateFlashCard);
            group.MapPut("/cardcontent", FlashCardEndpoints.UpdateCardContent);
            group.MapPost("/copy", FlashCardEndpoints.CopyFlashCard);
            group.MapDelete("/", FlashCardEndpoints.DeleteFlashCard);
        }
    }
}
