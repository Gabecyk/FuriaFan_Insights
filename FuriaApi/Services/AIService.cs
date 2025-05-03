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

        public async Task<RecommendationResponse> GetRecommendations(string jogoFavorito, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                Console.WriteLine("Erro: Mensagem do fã está vazia.");
                return null;
            }

            string prompt;

            if (jogoFavorito == "Valorant")
            {
                prompt = $@"
                Fale sobre o time de Valorant da FURIA.
                Responda o que o fã disse: '{mensagem}'.
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o video do YouTube 'conheça o time da furia' 'https://www.youtube.com/watch?v=tjMs5UuK_S8'.

                Forneça um JSON com os seguintes campos:
                - message: uma mensagem de resposta ao fã
                - recommendations: uma lista de objetos com: type, title, link
                ";
            }
            else
            {
                prompt = $"Recomende 3 conteúdos interessantes para um fã que disse: '{mensagem}'. Forneça um JSON com: message, recommendations (com type, title, link).";
            }

            var requestBody = new
            {
                model = "command-r",
                message = prompt,
                chat_history = new[]
                {
                    new { role = "USER", message = mensagem }
                },
                temperature = 0.7
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("v1/chat", content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine("🟡 Resposta bruta da API Cohere:");
            Console.WriteLine(responseString);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Erro da API Cohere: " + responseString);
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(responseString);
                var text = doc.RootElement.GetProperty("text").GetString();

                if (string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("Resposta da API Cohere não contém texto.");
                    return null;
                }

                var cleanedJson = text.Replace("```json", "").Replace("```", "").Trim();

                var aiResponse = JsonSerializer.Deserialize<AIResponse>(
                    cleanedJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (aiResponse == null || aiResponse.Recommendations == null)
                {
                    Console.WriteLine("Resposta não contém recomendações válidas.");
                    return null;
                }

                return new RecommendationResponse
                {
                    Message = aiResponse.Message,
                    Recommendations = aiResponse.Recommendations.Select(item => new Recommendation
                    {
                        Type = item.Type,
                        Title = item.Title,
                        Link = item.Link,
                        Tags = new List<string>()
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao interpretar resposta: " + ex.Message);
                return null;
            }
        }

        private class AIResponse
        {
            public string Message { get; set; }
            public List<RecommendationItem> Recommendations { get; set; }
        }

        private class RecommendationItem
        {
            public string Type { get; set; }
            public string Title { get; set; }
            public string Link { get; set; }
        }
    }
}