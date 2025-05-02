using System.Collections.Generic;

namespace FuriaAPI.Models
{
    public class Recommendation
    {
        public string Type { get; set; } // Ex: "YouTube", "Twitter", "Instagram"
        public string Title { get; set; }
        public string Link { get; set; }
        public List<string> Tags { get; set; } // Tags para facilitar a busca/filtragem
    }
}