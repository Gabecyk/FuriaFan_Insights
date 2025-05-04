using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using FuriaAPI.Models;

namespace FuriaAPI.Services
{
    public class YouTubeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public YouTubeService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _apiKey = config["YOUTUBE_API_KEY"];
        }

        public async Task<List<Recommendation>> SearchFuriaVideos(string jogoFavorito)
        {
            var query = $"FURIA {jogoFavorito}";
            var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(query)}&type=video&maxResults=3&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Recommendation>();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"YouTube API error: {response.StatusCode}");
                return new List<Recommendation>();
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var videos = new List<Recommendation>();
            foreach (var item in doc.RootElement.GetProperty("items").EnumerateArray())
            {
                var id = item.GetProperty("id").GetProperty("videoId").GetString();
                var snippet = item.GetProperty("snippet");
                var title = snippet.GetProperty("title").GetString();

                videos.Add(new Recommendation
                {
                    Type = "video",
                    Title = title,
                    Link = $"https://www.youtube.com/watch?v={id}",
                    Tags = new List<string> { "youtube", "furia", jogoFavorito.ToLower() }
                });
            }

            return videos;
        }
    }
}
