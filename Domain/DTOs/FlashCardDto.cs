namespace Domain.DTOs
{
    public class FlashCardDto
    {
        public enum DifficultyLevel
        {
            Easy,
            Normal,
            Medium,
            Hard
        }
        public int Id { get; set; }
        public CardContentDto Question { get; set; }
        public CardContentDto Answer { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int CollectionId { get; set; }
    }
}

public class CardContentDto
{
    public enum PrimaryContentType
    {
        Text,
        Image,
        Audio
    }
    public PrimaryContentType PrimaryContent { get; set; }
    public string? Text { get; set; }
    public byte[]? ImageBytes { get; set; }
    public byte[]? AudioBytes { get; set; }
}
