namespace DoeSangue.Domain.Entities;

public class Doador
{
    public Guid Id { get; set; }  = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }

    public Usuario Usuario { get; set; }

    public Doador() { }
    private Doador(Guid id, string nome, string documento, string telefone, DateOnly dataNascimento, Usuario usuario)
    {
        Id = id;
        Nome = nome;
        Documento = documento;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        Usuario = usuario;
    }

    public static Doador Criar(string nome, string documento, string telefone, DateOnly dataNascimento, Usuario usuario)
    {
        var doador = new Doador(id: Guid.NewGuid(),
                                nome: nome,
                                documento: documento,
                                telefone: telefone,
                                dataNascimento: dataNascimento,
                                usuario: usuario);

        doador.Validar();

        return doador;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new ArgumentException("O nome do doador é obrigatório.");

        if (string.IsNullOrWhiteSpace(Documento))
            throw new ArgumentException("O documento do doador é obrigatório.");

        if (string.IsNullOrWhiteSpace(Telefone))
            throw new ArgumentException("O telefone do doador é obrigatório.");

        if (Usuario == null)
            throw new ArgumentException("O usuário associado ao doador é obrigatório.");
    }
}