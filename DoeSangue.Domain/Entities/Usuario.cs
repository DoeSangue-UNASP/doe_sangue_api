using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } =  string.Empty;
    public UsuarioRole UsuarioRole { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; }

    public Endereco? Endereco { get; set; }
}