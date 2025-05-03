namespace FuriaAPI.Models
{
    public class RecommendationResponse
    {
        public string Message { get; set; }
        public List<Recommendation> Recommendations { get; set; }
    }
}