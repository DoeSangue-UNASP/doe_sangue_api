using Microsoft.AspNetCore.Identity;

namespace DoeSangue.Infrastructure.Models
{
    public class UsuarioIdentity : IdentityUser<Guid>
    {
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }

        public Guid EnderecoId { get; set; }
        public EnderecoModel? Endereco { get; set; }
    }
}
