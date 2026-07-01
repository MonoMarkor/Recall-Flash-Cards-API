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

        async Task<string> IClassification.GetClassificationAsync(string flashCard, string[] categories)
        {
            var response = await _client.Models.GenerateContentAsync(
                      model: "gemini-2.5-flash", 
                      contents: $"Below you are given the contents of a flashcard, i.e it's title content and it's body content. You have to order the flashcard into one of the following categories: {categories}. If not categories match the flashcard then just output \"other\". Only output the category and nothing else. Below are the contents of the flash card:{flashCard}"
                    );
            return response.Candidates[0].Content.Parts[0].Text;
        }

        async Task<string> IClassification.GetTranslationOfHeaderAsync(string header, string languageName)
        {
            var response = await _client.Models.GenerateContentAsync(
                      model: "gemini-2.5-flash",
                      contents: $"Below you are given the header of a flashcard, i.e it's title content. You have to translate the header into the {languageName} language. Only output the translated header and nothing else. Below is the header:\n{header}"
                    );
            return response.Candidates[0].Content.Parts[0].Text;
        }
        async Task<string> IClassification.GetTranslationOfBodyAsync(string body, string languageName)
        {
            var response = await _client.Models.GenerateContentAsync(
                      model: "gemini-2.5-flash",
                      contents: $"Below you are given the body of a flashcard, i.e it's main content. You have to translate the body into the {languageName} language. Only output the translated body and nothing else. Below is the body:\n{body}"
                    );
            return response.Candidates[0].Content.Parts[0].Text;
        }
    }
}
