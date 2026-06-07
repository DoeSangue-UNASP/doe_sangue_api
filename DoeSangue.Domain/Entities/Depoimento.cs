namespace DoeSangue.Domain.Entities;

public class Depoimento
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    // Autor do depoimento: qualquer Usuario (hemocentro, doador ou outro papel); associação sempre via Usuario.
    public Usuario Autor { get; set; }

    private Depoimento(Guid id, string titulo, string conteudo, DateTime criadoEm, DateTime? atualizadoEm, Usuario autor)
    {
        Id = id;
        Titulo = titulo;
        Conteudo = conteudo;
        CriadoEm = criadoEm;
        AtualizadoEm = atualizadoEm;
        Autor = autor;
    }

    public static Depoimento Criar(string titulo, string conteudo, Usuario autor)
    {
        var depoimento = new Depoimento(id: Guid.NewGuid(),
                                        titulo: titulo,
                                        conteudo: conteudo,
                                        criadoEm: DateTime.UtcNow,
                                        atualizadoEm: null,
                                        autor: autor);

        depoimento.Validar();

        return depoimento;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
            throw new ArgumentException("O título do depoimento é obrigatório.");

        if (string.IsNullOrWhiteSpace(Conteudo))
            throw new ArgumentException("O conteúdo do depoimento é obrigatório.");

        if (Autor == null)
            throw new ArgumentException("O autor do depoimento é obrigatório.");
    }
}