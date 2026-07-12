
namespace Infrastructure.SQL.Database.Entities
{
    public class FlashCardEntity
    {
        public enum DifficultyLevel
        {
            Easy,
            Normal,
            Medium,
            Hard
        }
        public int Id { get; set; }
        public CardContentEntity Question { get; set; }
        public CardContentEntity Answer { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int CollectionId { get; set; }
    }
}

public class CardContentEntity
{
    public enum PrimaryContentType
    {
        Text,
        Image,
        Audio
    }
    public PrimaryContentType PrimaryContent { get; set; }
    public string? Text { get; set; }
    public string? ImagePath { get; set; }
    public string? AudioPath { get; set; }
}

