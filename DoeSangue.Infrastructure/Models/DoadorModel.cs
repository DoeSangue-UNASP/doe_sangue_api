using DoeSangue.Domain.Entities;

namespace DoeSangue.Infrastructure.Models;

public class DoadorModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Documento { get; set; }
    public string Telefone { get; set; }
    public DateOnly DataNascimento { get; set; }

    public Guid UsuarioId { get; set; }
    public UsuarioIdentity Usuario { get; set; }

    public DoadorModel() { }
    public DoadorModel(Guid id, string nome, string documento, string telefone, DateOnly dataNascimento, Guid usuarioId)
    {
        Id = id;
        Nome = nome;
        Documento = documento;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        UsuarioId = usuarioId;
    }

    public DoadorModel(Doador doador)
    {
        Id = doador.Id;
        Nome = doador.Nome;
        Documento = doador.Documento;
        Telefone = doador.Telefone;
        DataNascimento = doador.DataNascimento;
        UsuarioId = doador.Usuario.Id;
    }

    public Doador ToDomain()
    {
        return new Doador
        {
            Id = Id,
            Nome = Nome,
            Documento = Documento,
            Telefone = Telefone,
            DataNascimento = DataNascimento
        };
    }
}