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
}