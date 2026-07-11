namespace RecallFlashCardsAPI.Models
{
    public class FlashCard
    {
        public enum DifficultyLevel
        {
            Easy,
            Normal,
            Medium,
            Hard
        }
        public int Id { get; set; }
        public CardContent Question { get; set; }
        public CardContent Answer { get; set; }
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
    public string? ImageBase64 { get; set; }
    public string? AudioImage64 { get; set; }
}



