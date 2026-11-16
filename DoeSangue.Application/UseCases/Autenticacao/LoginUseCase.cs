using DoeSangue.Application.Dtos.Autenticacao;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Autenticacao
{
    public class LoginUseCase
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginUseCase(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<LoginResponseDto?> Executar(LoginDto loginDto)
        {
            var authResult = await _authenticationService.LoginAsync(loginDto.Email, loginDto.Senha);

            if (authResult is null)
            {
                return null;
            }

            return new LoginResponseDto(authResult.Token, authResult.Email, authResult.Role);
        }
    }
}
