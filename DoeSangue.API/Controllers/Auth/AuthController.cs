using System.Security.Claims;
using DoeSangue.Application.Dtos.Autenticacao;
using DoeSangue.Application.UseCases.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoeSangue.API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase;

        public AuthController(LoginUseCase loginUseCase)
        {
            _loginUseCase = loginUseCase;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _loginUseCase.Executar(loginDto);

            if (result is null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
