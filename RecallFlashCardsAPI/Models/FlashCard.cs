namespace RecallFlashCardsAPI.Models
{
    public class FlashCard
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string LanguageCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
