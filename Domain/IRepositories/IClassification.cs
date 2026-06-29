
namespace Domain.IRepositories
{
    public interface IClassification
    {
        Task<string> GetClassificationAsync(string flashCard);
    }
}
