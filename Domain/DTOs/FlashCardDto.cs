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
        public string? Id { get; set; }
        public CardContent Question { get; set; }
        public CardContent Answer { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string CollectionId { get; set; }
    }
}

public class CardContent
{
    public enum PrimaryContentType
    {
        Text,
        Image,
        Audio
    }
    PrimaryContentType Primary { get; set; }
    public string? Text { get; set; }
    public byte[]? ImageBytes { get; set; }
    public byte[]? AudioBytes { get; set; }
}
