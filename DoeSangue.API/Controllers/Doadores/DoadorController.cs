using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Application.UseCases.Doadores;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Doadores
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoadorController : ControllerBase
    {
        private readonly CriarDoadorUseCase _criarDoadorUseCase;

        public DoadorController(CriarDoadorUseCase criarDoadorUseCase)
        {
            _criarDoadorUseCase = criarDoadorUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarDoadorDto doadorDto)
        {
            try
            {
                await _criarDoadorUseCase.Executar(doadorDto);
                return Created();
            }
            catch (ArgumentException exception)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, exception.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
