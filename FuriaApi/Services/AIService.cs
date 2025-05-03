using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FuriaAPI.Models;

namespace FuriaAPI.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIService(IConfiguration configuration)
        {
            _apiKey = configuration["COHERE_API_KEY"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.cohere.ai/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<Recommendation>> GetRecommendations(string jogoFavorito, string mensagem)
        {
            var prompt = string.IsNullOrEmpty(jogoFavorito)
                ? $"Recomende 3 conteúdos interessantes para um fã que disse: '{mensagem}'. Forneça um JSON com: type, title, link."
                : $"Recomende 3 conteúdos sobre {jogoFavorito} para um fã que disse: '{mensagem}'. Responda em JSON: type, title, link.";

            var requestBody = new
            {
                model = "command-r",
                prompt = prompt,
                max_tokens = 300,
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("v1/generate", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Erro da API Cohere: " + responseString);
                return new List<Recommendation>();
            }

            try
            {
                using var doc = JsonDocument.Parse(responseString);
                var text = doc.RootElement.GetProperty("generations")[0].GetProperty("text").GetString();
                var json = ExtractJsonResponse(text);

                var items = JsonSerializer.Deserialize<List<RecommendationJson>>(json);
                return items?.Select(item => new Recommendation
                {
                    Type = item.type,
                    Title = item.title,
                    Link = item.link,
                    Tags = new List<string>()
                }).ToList() ?? new List<Recommendation>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao interpretar resposta: " + ex.Message);
                return new List<Recommendation>();
            }
        }

        private string ExtractJsonResponse(string text)
        {
            int start = text.IndexOf('[');
            int end = text.LastIndexOf(']');
            if (start >= 0 && end > start)
                return text.Substring(start, end - start + 1);
            return "[]";
        }

        private class RecommendationJson
        {
            public string type { get; set; }
            public string title { get; set; }
            public string link { get; set; }
        }
    }
}
