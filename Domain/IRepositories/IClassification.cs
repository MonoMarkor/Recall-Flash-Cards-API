
namespace Domain.IRepositories
{
    public interface IClassification
    {
        Task<string> GetClassificationAsync(string flashCard, string[] categories);
        Task<string> GetTranslationOfHeaderAsync(string header, string lang);
        Task<string> GetTranslationOfBodyAsync(string body, string lang);
    }
}
