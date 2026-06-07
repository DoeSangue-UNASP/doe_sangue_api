using DoeSangue.Application.Dtos.Hemocentros;
using DoeSangue.Application.UseCases.Hemocentros;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Hemocentros
{
    [Route("api/[controller]")]
    [ApiController]
    public class HemocentroController : ControllerBase
    {
        private readonly CriarHemocentroUseCase _criarHemocentroUseCase;

        public HemocentroController(CriarHemocentroUseCase criarHemocentroUseCase)
        {
            _criarHemocentroUseCase = criarHemocentroUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarHemocentroDto hemocentroDto)
        {
            try
            {
                await _criarHemocentroUseCase.Executar(hemocentroDto);

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
