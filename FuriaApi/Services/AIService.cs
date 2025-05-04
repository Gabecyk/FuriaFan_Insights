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
        private readonly YouTubeService _youTubeService;

        public AIService(IConfiguration configuration, YouTubeService youTubeService)
        {
            _apiKey = configuration["COHERE_API_KEY"];
            _youTubeService = youTubeService;
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
                mensagem = "Fale mais sobre o time/organização da FURIA Esport";
            }

            var jogo = jogoFavorito.ToLower();
            string prompt = jogo switch
            {
                "valorant" => $@"
                Você é um assistente. Informe sobre o time de Valorant da FURIA.
                Fale sobre a organização FURIA Esports.
                Responda: '{mensagem}'
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o vídeo 'https://www.youtube.com/watch?v=tjMs5UuK_S8'
                Forneça um JSON com: message, recommendations (lista de objetos com: type, title, link)
                ",
                                "counter strike 2" => $@"
                Você é um assistente. Informe sobre o time de Counter-Strike 2 da FURIA.
                Fale sobre a organização FURIA Esports.
                Responda: '{mensagem}'
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o vídeo 'https://www.youtube.com/watch?v=MvNP9FuN4qU'
                Forneça um JSON com: message, recommendations (lista de objetos com: type, title, link)
                ",
                                "rocket league" => $@"
                Você é um assistente. Informe sobre o time de Rocket League da FURIA.
                Fale sobre a organização FURIA Esports.
                Responda: '{mensagem}'
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o vídeo 'https://www.youtube.com/watch?v=BDXfF9-4BKo'
                Forneça um JSON com: message, recommendations (lista de objetos com: type, title, link)
                ",
                                "league of legends" => $@"
                Você é um assistente. Informe sobre o time de League of Legends da FURIA.
                Fale sobre a organização FURIA Esports.
                Responda: '{mensagem}'
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o vídeo 'https://www.youtube.com/watch?v=zKe3MLpsddM&t=10s'
                Forneça um JSON com: message, recommendations (lista de objetos com: type, title, link)
                ",
                                "rainbow six" => $@"
                Você é um assistente. Informe sobre o time de Rainbow Six da FURIA.
                Fale sobre a organização FURIA Esports.
                Responda: '{mensagem}'
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o vídeo 'https://www.youtube.com/watch?v=CIXy3M2kQxA'
                Forneça um JSON com: message, recommendations (lista de objetos com: type, title, link)
                ",
                                _ => $@"
                Você é um assistente. Informe sobre o time de Apex Legends da FURIA.
                Fale sobre a organização FURIA Esports.
                Responda: '{mensagem}'
                Recomende o Instagram 'https://www.instagram.com/furiagg/' e o vídeo 'https://www.youtube.com/watch?v=00Dqtnwtico'
                Forneça um JSON com: message, recommendations (lista de objetos com: type, title, link)
"
            };

            var requestBody = new
            {
                model = "command-r",
                message = prompt,
                chat_history = new[] { new { role = "USER", message = mensagem } },
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
                var root = doc.RootElement;

                // Busca o JSON gerado pela IA (geralmente em generations[0].text)
                var aiJson = root.GetProperty("generations")[0].GetProperty("text").GetString();
                if (string.IsNullOrWhiteSpace(aiJson))
                {
                    Console.WriteLine("Texto gerado vazio.");
                    return null;
                }

                var cleanedJson = aiJson.Replace("```json", "").Replace("```", "").Trim();

                var aiResponse = JsonSerializer.Deserialize<AIResponse>(
                    cleanedJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (aiResponse?.Recommendations == null)
                {
                    Console.WriteLine("Recomendações ausentes na resposta.");
                    return null;
                }

                var cohereRecs = aiResponse.Recommendations.Select(r => new Recommendation
                {
                    Type = r.Type,
                    Title = r.Title,
                    Link = r.Link,
                    Tags = new List<string>()
                }).ToList();

                // Adiciona vídeos do YouTube relacionados
                var youtubeRecs = await _youTubeService.SearchFuriaVideos(jogoFavorito);
                cohereRecs.AddRange(youtubeRecs);

                return new RecommendationResponse
                {
                    Message = aiResponse.Message,
                    Recommendations = cohereRecs
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar resposta da IA: {ex.Message}");
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
