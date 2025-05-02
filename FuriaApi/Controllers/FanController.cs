using Microsoft.AspNetCore.Mvc;
using FuriaAPI.Models;
using FuriaAPI.Services; 

namespace FuriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FanController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public FanController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Fan fan)
        {
            Console.WriteLine($"Dados Recebidos: Nome={fan.Nome}, TempoFuria={fan.TempoFuria}, JogoFavorito={fan.JogoFavorito}, Plataforma={fan.Plataforma}, Mensagem={fan.Mensagem}");
            var nivel = fan.TempoFuria;
            if(nivel.ToLower() == "menos de 1 ano")
                nivel = "Fã Iniciante";
            else if(nivel.ToLower() == "1 a 3 anos")
                nivel = "Fã Raiz";
            else if(nivel.ToLower() == "mais de 3 anos")
                nivel = "FURIOSO MASTER";
            else
                nivel = "Fã Iniciante";

            
            fan.NivelFuria = nivel; // Adicione o nível ao objeto Fan antes de salvar

            await _mongoDbService.CreateAsync(fan);

            return Ok(new
            {
                mensagem = "Fã registrado com sucesso!",
                nivel
            });
        }
    }
}


/*
string nivel = fan.TempoFuria switch
            {
                "menos de 1 ano" => "Iniciante",
                "1 a 3 anos" => "Fã Raiz",
                "mais de 3 anos" => "FURIOSO MASTER",
                _ => "Desconhecido"
            };
            */