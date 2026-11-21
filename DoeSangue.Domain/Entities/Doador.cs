namespace DoeSangue.Domain.Entities;

public class Doador
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }
    public Guid? UsuarioId { get; set; }
    public Guid? HemocentroId { get; set; }

    public Usuario? Usuario { get; set; }
    public Hemocentro? Hemocentro { get; set; }

    public bool EhLead => UsuarioId == null && HemocentroId.HasValue;
    public bool EhUsuario => UsuarioId.HasValue;

    public Doador() { }

    private Doador(Guid id, string nome, string documento, string telefone, DateOnly dataNascimento, Guid? usuarioId, Guid? hemocentroId, Usuario? usuario = null, Hemocentro? hemocentro = null)
    {
        Id = id;
        Nome = nome;
        Documento = documento;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        UsuarioId = usuarioId;
        HemocentroId = hemocentroId;
        Usuario = usuario;
        Hemocentro = hemocentro;
    }

    public static Doador CriarUsuario(string nome, string documento, string telefone, DateOnly dataNascimento, Usuario usuario)
    {
        var doador = new Doador(id: Guid.NewGuid(),
                                nome: nome,
                                documento: documento,
                                telefone: telefone,
                                dataNascimento: dataNascimento,
                                usuarioId: usuario.Id,
                                hemocentroId: null,
                                usuario: usuario);

        doador.Validar();

        return doador;
    }

    public static Doador CriarLead(string nome, string documento, string telefone, DateOnly dataNascimento, Guid hemocentroId)
    {
        var doador = new Doador(id: Guid.NewGuid(),
                                nome: nome,
                                documento: documento,
                                telefone: telefone,
                                dataNascimento: dataNascimento,
                                usuarioId: null,
                                hemocentroId: hemocentroId);

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

        if (!UsuarioId.HasValue && !HemocentroId.HasValue)
            throw new ArgumentException("O doador deve ser um usuário ou um lead de um hemocentro.");

        if (UsuarioId.HasValue && HemocentroId.HasValue)
            throw new ArgumentException("Um doador não pode ser simultaneamente um usuário e um lead.");
    }

    public void AdicionarUsuario(Usuario usuario) => Usuario = usuario;
    public void AdicionarHemocentro(Hemocentro hemocentro) => Hemocentro = hemocentro;
}