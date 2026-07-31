using RecallFlashCardsAPI.Endpoints;

namespace RecallFlashCardsAPI.RouteGroups
{
    public static class CollectionGroup
    {
        public static void AddCollectionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/collection");

            group.MapPost("/", CollectionEndpoints.CreateCollectionAsync);
            group.MapPut("/name", CollectionEndpoints.UpdateCollectionNameAsync);
            group.MapPut("/description", CollectionEndpoints.UpdateCollectionDescriptionAsync);
            group.MapDelete("/", CollectionEndpoints.SafelyDeleteAsync);
            group.MapDelete("/delete", CollectionEndpoints.DeleteCollectionAsync);
            group.MapDelete("/flashcards", CollectionEndpoints.DeleteAllFlashCardsOfCollectionAsync);
        }
    }
}
