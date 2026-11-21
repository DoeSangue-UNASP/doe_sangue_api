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
    public string? Sexo { get; set; }
    public string? TipoSanguineo { get; set; }

    public Usuario? Usuario { get; set; }
    public Hemocentro? Hemocentro { get; set; }

    public static readonly string[] SexosValidos = { "M", "F" };
    public static readonly string[] TiposSanguineosValidos = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

    public bool EhLead => UsuarioId == null && HemocentroId.HasValue;
    public bool EhUsuario => UsuarioId.HasValue;

    public Doador() { }

    private Doador(Guid id, string nome, string documento, string telefone, DateOnly dataNascimento, Guid? usuarioId, Guid? hemocentroId, Usuario? usuario = null, Hemocentro? hemocentro = null, string? sexo = null, string? tipoSanguineo = null)
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
        Sexo = Normalizar(sexo);
        TipoSanguineo = Normalizar(tipoSanguineo);
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

    public static Doador CriarLead(string nome, string documento, string telefone, DateOnly dataNascimento, Guid hemocentroId, string? sexo = null, string? tipoSanguineo = null)
    {
        var doador = new Doador(id: Guid.NewGuid(),
                                nome: nome,
                                documento: documento,
                                telefone: telefone,
                                dataNascimento: dataNascimento,
                                usuarioId: null,
                                hemocentroId: hemocentroId,
                                sexo: sexo,
                                tipoSanguineo: tipoSanguineo);

        doador.Validar();

        return doador;
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim().ToUpperInvariant();

    public void AtualizarDadosClinicos(string? sexo, string? tipoSanguineo)
    {
        Sexo = Normalizar(sexo);
        TipoSanguineo = Normalizar(tipoSanguineo);
        Validar();
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new ArgumentException("O nome do doador é obrigatório.");

        if (string.IsNullOrWhiteSpace(Documento))
            throw new ArgumentException("O documento do doador é obrigatório.");

        if (string.IsNullOrWhiteSpace(Telefone))
            throw new ArgumentException("O telefone do doador é obrigatório.");

        if (Sexo is not null && !SexosValidos.Contains(Sexo))
            throw new ArgumentException("O sexo deve ser M ou F.");

        if (TipoSanguineo is not null && !TiposSanguineosValidos.Contains(TipoSanguineo))
            throw new ArgumentException("O tipo sanguíneo é inválido.");

        if (!UsuarioId.HasValue && !HemocentroId.HasValue)
            throw new ArgumentException("O doador deve ser um usuário ou um lead de um hemocentro.");

        if (UsuarioId.HasValue && HemocentroId.HasValue)
            throw new ArgumentException("Um doador não pode ser simultaneamente um usuário e um lead.");
    }

    public void AdicionarUsuario(Usuario usuario) => Usuario = usuario;
    public void AdicionarHemocentro(Hemocentro hemocentro) => Hemocentro = hemocentro;
}