namespace DoeSangue.Domain.Interfaces
{
    public class UsuarioAuthData
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public interface IAuthenticationService
    {
        Task<UsuarioAuthData?> LoginAsync(string email, string password);
    }
}
