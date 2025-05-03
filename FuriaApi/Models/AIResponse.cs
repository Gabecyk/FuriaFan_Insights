namespace FuriaAPI.Models
{
    public class AIResponse
    {
        public string Message { get; set; }
        public List<RecommendationJson> Recommendations { get; set; }
    }

    public class RecommendationJson
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}