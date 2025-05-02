using Microsoft.AspNetCore.Mvc;
using FuriaAPI.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Adicione esta diretiva using

namespace FuriaAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly AIService _aiService;
        private readonly ILogger<AIController> _logger; // Adicione o logger

        public AIController(AIService aiService, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _logger = logger; // Inicialize o logger
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AIRequest request)
        {
            // Validação dos dados de entrada
            if (string.IsNullOrEmpty(request.JogoFavorito) || string.IsNullOrEmpty(request.Mensagem))
            {
                _logger.LogWarning("JogoFavorito e Mensagem são obrigatórios."); // Registre o aviso
                return BadRequest("JogoFavorito e Mensagem são obrigatórios.");
            }

            try
            {
                // Obtenha as recomendações do serviço de IA
                var recommendations = await _aiService.GetRecommendations(request.JogoFavorito, request.Mensagem);
                _logger.LogInformation("Recomendações geradas com sucesso."); // Registre o sucesso

                // Retorne as recomendações
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar requisição de recomendações."); // Registre o erro com detalhes
                return StatusCode(500, "Ocorreu um erro ao processar a requisição."); // Retorne uma mensagem de erro genérica
            }
        }
    }
     // Classe para representar os dados de entrada da requisição
    public class AIRequest
    {
        public string JogoFavorito { get; set; }
        public string Mensagem { get; set; }
    }
}