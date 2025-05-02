using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FuriaAPI.Models;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.SharedModels; 


namespace FuriaAPI.Services
{
    public class AIService
    {
        private readonly OpenAIService _openAIService;

        public AIService(IConfiguration configuration)
        {
            string apiKey = configuration["OPENAI_API_KEY"];
            _openAIService = new OpenAIService(new OpenAiOptions
            {
                ApiKey = apiKey
            });
        }

        public async Task<List<Recommendation>> GetRecommendationsFromOpenAI(string jogoFavorito, string mensagem)
        {
            var recommendations = new List<Recommendation>();
            string prompt;

            if (!string.IsNullOrEmpty(jogoFavorito))
            {
                prompt = $"Recomende 3 conteúdos relacionados a {jogoFavorito} para um fã que disse: '{mensagem}'. Forneça o tipo (YouTube, Artigo, Notícia), título e link. Responda no formato de um array JSON com os campos: type, title e link.";
            }
            else
            {
                prompt = $"Recomende 3 conteúdos interessantes para um fã que disse: '{mensagem}'. Forneça o tipo (YouTube, Artigo, Notícia), título e link. Responda no formato de um array JSON com os campos: type, title e link.";
            }

            var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("Você é um assistente que recomenda conteúdos para fãs de e-sports."),
                    ChatMessage.FromUser(prompt)
                },
                Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo,
                MaxTokens = 300,
                Temperature = 0.7f
            });

            if (completionResult.Successful)
            {
                try
                {
                    var jsonContent = completionResult.Choices.First().Message.Content;
                    string jsonResponse = ExtractJsonResponse(jsonContent);
                    var openaiRecommendations = System.Text.Json.JsonSerializer.Deserialize<List<OpenAIRecommendationItem>>(jsonResponse);

                    if (openaiRecommendations != null)
                    {
                        recommendations = openaiRecommendations.Select(item => new Recommendation
                        {
                            Type = item.type,
                            Title = item.title,
                            Link = item.link,
                            Tags = new List<string>()
                        }).ToList();
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    Console.WriteLine($"Erro ao desserializar resposta da OpenAI: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Erro na chamada à OpenAI: {completionResult.Error?.Message}");
            }

            return recommendations;
        }

        public class OpenAIRecommendationItem
        {
            public string type { get; set; }
            public string title { get; set; }
            public string link { get; set; }
        }

        private string ExtractJsonResponse(string text)
        {
            int startIndex = text.IndexOf('[');
            int endIndex = text.LastIndexOf(']');
            if (startIndex >= 0 && endIndex > startIndex)
            {
                return text.Substring(startIndex, endIndex - startIndex + 1);
            }
            return "[]";
        }

        public async Task<List<Recommendation>> GetRecommendations(string jogoFavorito, string mensagem)
        {
            return await GetRecommendationsFromOpenAI(jogoFavorito, mensagem);
        }
    }
}
