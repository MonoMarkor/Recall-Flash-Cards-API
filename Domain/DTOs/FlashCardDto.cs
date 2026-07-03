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
        public string Id { get; set; }
        public CardContent Question { get; set; }
        public CardContent Answer { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DifficultyLevel DiffLevel { get; set; }
        public string CollectionId { get; set; }
    }
}

public class CardContent
{
    enum PrimaryContentType
    {
        IxText,
        IxImagePath,
        IxAudioPath
    }
    PrimaryContentType Primary { get; set; }
    public string Text { get; set; }
    public string ImagePath { get; set; }
    public string AudioPath { get; set; }
}
