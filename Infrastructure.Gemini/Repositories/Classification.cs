using Domain.IRepositories;
using Google.GenAI;

namespace Infrastructure.Gemini.Repositories
{
    public class Classification : IClassification
    {
        private readonly Client _client;

        public Classification(Client client)
        {
            _client = client;
        }

        async Task<string> IClassification.GetClassificationAsync(string flashCard)
        {
            var response = await _client.Models.GenerateContentAsync(
                      model: "gemini-2.5-flash", 
                      contents: "Would you classify a cat as cute or dangerous? Please output only one word!"
                    );
            return response.Candidates[0].Content.Parts[0].Text;
        }
    }
}
