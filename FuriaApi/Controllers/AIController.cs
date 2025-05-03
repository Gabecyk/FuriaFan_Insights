using FuriaAPI.Models;
using FuriaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly AIService _aiService;
        private readonly ILogger<AIController> _logger;

        public AIController(AIService aiService, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost("recomendar")]
        public async Task<IActionResult> Post([FromBody] FanInput input)
        {
            _logger.LogInformation($"Recebido: jogoFavorito={input.JogoFavorito}, mensagem={input.Mensagem}");

            if (string.IsNullOrWhiteSpace(input.Mensagem))
            {
                return BadRequest(new { error = "A mensagem do fã não pode estar vazia." });
            }

            var resposta = await _aiService.GetRecommendations(input.JogoFavorito, input.Mensagem);

            if (resposta == null || resposta.Recommendations == null || resposta.Recommendations.Count == 0)
            {
                return StatusCode(502, new { error = "Erro ao gerar recomendações com a API Cohere." });
            }

            _logger.LogInformation("Recomendações geradas com sucesso.");
            return Ok(resposta);
        }
    }
}
