using System.Collections.Generic;

namespace FuriaAPI.Models
{
    public class Recommendation
    {
        public required string Type { get; set; }
        public required string Title { get; set; }
        public required string Link { get; set; }
        public required List<string> Tags { get; set; }
    }

}