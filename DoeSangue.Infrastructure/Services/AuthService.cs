using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DoeSangue.Infrastructure.Services
{
    public class AuthService : IAuthenticationService
    {
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<UsuarioIdentity> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<UsuarioAuthData?> LoginAsync(string email, string password)
        {
            var usuarioIdentity = await _userManager.FindByEmailAsync(email);

            if (usuarioIdentity is null)
            {
                return null;
            }

            if (!await _userManager.CheckPasswordAsync(usuarioIdentity, password))
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(usuarioIdentity);
            var role = roles.FirstOrDefault() ?? nameof(UsuarioRole.DOADOR);

            var jwtSecret = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT secret não configurada.");
            var issuer = _configuration["JwtSettings:Issuer"] ?? "DoeSangue.API";
            var audience = _configuration["JwtSettings:Audience"] ?? "DoeSangue.Client";

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuarioIdentity.Id.ToString()),
                new(ClaimTypes.Email, usuarioIdentity.Email ?? string.Empty),
                new(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UsuarioAuthData
            {
                UserId = usuarioIdentity.Id,
                Email = usuarioIdentity.Email ?? string.Empty,
                Role = role,
                Token = tokenString
            };
        }
    }
}
