namespace DoeSangue.Infrastructure.Models;

public class DepoimentoModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Conteudo { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }

    public Guid AutorId { get; set; }
    public UsuarioIdentity Autor { get; set; }

    public DepoimentoModel() { }

    public DepoimentoModel(Guid id, string titulo, string conteudo, DateTime criadoEm, DateTime? atualizadoEm, Guid autorId)
    {
        Id = id;
        Titulo = titulo;
        Conteudo = conteudo;
        CriadoEm = criadoEm;
        AtualizadoEm = atualizadoEm;
        AutorId = autorId;
    }
}