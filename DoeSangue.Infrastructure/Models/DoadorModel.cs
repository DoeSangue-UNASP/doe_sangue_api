using DoeSangue.Domain.Entities;

namespace DoeSangue.Infrastructure.Models;

public class DoadorModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Documento { get; set; }
    public string Telefone { get; set; }
    public DateOnly DataNascimento { get; set; }
    public Guid? UsuarioId { get; set; }
    public Guid? HemocentroId { get; set; }
    public string? Sexo { get; set; }
    public string? TipoSanguineo { get; set; }

    public UsuarioIdentity? Usuario { get; set; }
    public HemocentroModel? Hemocentro { get; set; }

    public DoadorModel() { }

    public DoadorModel(Guid id, string nome, string documento, string telefone, DateOnly dataNascimento, Guid? usuarioId, Guid? hemocentroId, string? sexo = null, string? tipoSanguineo = null)
    {
        Id = id;
        Nome = nome;
        Documento = documento;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        UsuarioId = usuarioId;
        HemocentroId = hemocentroId;
        Sexo = sexo;
        TipoSanguineo = tipoSanguineo;
    }

    public DoadorModel(Doador doador)
    {
        Id = doador.Id;
        Nome = doador.Nome;
        Documento = doador.Documento;
        Telefone = doador.Telefone;
        DataNascimento = doador.DataNascimento;
        UsuarioId = doador.UsuarioId;
        HemocentroId = doador.HemocentroId;
        Sexo = doador.Sexo;
        TipoSanguineo = doador.TipoSanguineo;
    }

    public Doador ToDomain()
    {
        return new Doador
        {
            Id = Id,
            Nome = Nome,
            Documento = Documento,
            Telefone = Telefone,
            DataNascimento = DataNascimento,
            UsuarioId = UsuarioId,
            HemocentroId = HemocentroId,
            Sexo = Sexo,
            TipoSanguineo = TipoSanguineo
        };
    }
}