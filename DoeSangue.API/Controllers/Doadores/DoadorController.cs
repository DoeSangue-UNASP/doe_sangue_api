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
        private readonly BuscarDoadorPorId _buscarDoadorPorId;

        public DoadorController(CriarDoadorUseCase criarDoadorUseCase, BuscarDoadorPorId buscarDoadorPorUsuarioId)
        {
            _criarDoadorUseCase = criarDoadorUseCase;
            _buscarDoadorPorId = buscarDoadorPorUsuarioId;
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

        [HttpGet("{doadorId}")]
        public async Task<IActionResult> BuscarPorId(Guid doadorId)
        {
            try
            {
                var doador = await _buscarDoadorPorId.Executar(doadorId);

                if (doador is null)
                {
                    return NotFound();
                }

                return Ok(doador);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
