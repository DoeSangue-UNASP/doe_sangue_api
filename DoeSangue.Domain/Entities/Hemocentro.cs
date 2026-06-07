namespace DoeSangue.Domain.Entities;

public class Hemocentro
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Cnpj { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public DateTime AbreEm { get; set; }
    public DateTime FechaEm { get; set; }

    public Usuario Usuario { get; set; }

    public Hemocentro() { }

    private Hemocentro(Guid id, string cnpj, string site, string nomeFantasia, DateTime abreEm, DateTime fechaEm, Usuario usuario)
    {
        Id = id;
        Cnpj = cnpj;
        Site = site;
        NomeFantasia = nomeFantasia;
        AbreEm = abreEm;
        FechaEm = fechaEm;
        Usuario = usuario;
    }

    public static Hemocentro Criar(string cnpj, string site, string nomeFantasia, DateTime abreEm, DateTime fechaEm, Usuario usuario)
    {
        var hemocentro = new Hemocentro(id: Guid.NewGuid(),
                                        cnpj: cnpj,
                                        site: site,
                                        nomeFantasia: nomeFantasia,
                                        abreEm: abreEm,
                                        fechaEm: fechaEm,
                                        usuario: usuario);

        hemocentro.Validar();

        return hemocentro;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Cnpj))
            throw new ArgumentException("O CNPJ é obrigatório.");

        if (string.IsNullOrWhiteSpace(Site))
            throw new ArgumentException("O site é obrigatório.");

        if (string.IsNullOrWhiteSpace(NomeFantasia))
            throw new ArgumentException("O nome fantasia é obrigatório.");

        if (Usuario == null)
            throw new ArgumentException("O usuário associado ao hemocentro é obrigatório.");
    }

    public void AdicionarUsuario(Usuario usuario) => Usuario = usuario;
}