using DoeSangue.Application.Dtos.BolsasSangue;
using DoeSangue.Application.UseCases.BolsasSangue;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.BolsasSangue
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BolsaSangueController : ControllerBase
    {
        private readonly CriarBolsaSangueUseCase _criarBolsaSangueUseCase;

        public BolsaSangueController(CriarBolsaSangueUseCase criarBolsaSangueUseCase)
        {
            _criarBolsaSangueUseCase = criarBolsaSangueUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarBolsaSangueDto dto)
        {
            try
            {
                await _criarBolsaSangueUseCase.Executar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
