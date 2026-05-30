using System.Net.Mail;
using System.Text.RegularExpressions;
using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } =  string.Empty;
    public UsuarioRole Role { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    public Endereco? Endereco { get; set; }

    private Usuario(Guid id, string email, UsuarioRole role, DateTime criadoEm, DateTime? atualizadoEm, Endereco? endereco)
    {
        Id = id;
        Email = email;
        Role = role;
        CriadoEm = criadoEm;
        AtualizadoEm = atualizadoEm;
        Endereco = endereco;
    }

    public static Usuario Criar(string email, UsuarioRole role, Endereco? endereco = null)
    {
        var usuario = new Usuario(id: Guid.NewGuid(),
                                  email: email,
                                  role: role,
                                  criadoEm: DateTime.UtcNow,
                                  atualizadoEm: null,
                                  endereco: endereco);

        usuario.Validar();

        return usuario;
    }

    public void Validar()
    {
        if (Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") == false)
            throw new ArgumentException("O email do usuário é inválido.");
    }
}