using DoeSangue.Application.Dtos.Agendamentos;
using DoeSangue.Application.UseCases.Agendamentos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Agendamentos
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AgendamentoController : ControllerBase
    {
        private readonly CriarAgendamentoUseCase _criarAgendamentoUseCase;

        public AgendamentoController(CriarAgendamentoUseCase criarAgendamentoUseCase)
        {
            _criarAgendamentoUseCase = criarAgendamentoUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarAgendamentoDto dto)
        {
            try
            {
                await _criarAgendamentoUseCase.Executar(dto);
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
