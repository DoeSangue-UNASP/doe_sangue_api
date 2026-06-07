using DoeSangue.Domain.Entities;

namespace DoeSangue.Infrastructure.Models;

public class HemocentroModel
{
    public Guid Id { get; set; }
    public string Cnpj { get; set; }
    public string Site { get; set; }
    public string NomeFantasia { get; set; }
    public DateTime AbreEm { get; set; }
    public DateTime FechaEm { get; set; }

    public Guid UsuarioId { get; set; }
    public UsuarioIdentity Usuario { get; set; }

    public HemocentroModel() { }
    public HemocentroModel(Guid id, string cnpj, string site, string nomeFantasia, DateTime abreEm, DateTime fechaEm, Guid usuarioId)
    {
        Id = id;
        Cnpj = cnpj;
        Site = site;
        NomeFantasia = nomeFantasia;
        AbreEm = abreEm;
        FechaEm = fechaEm;
        UsuarioId = usuarioId;
    }

    public HemocentroModel(Hemocentro hemocentro)
    {
        Id = hemocentro.Id;
        Cnpj = hemocentro.Cnpj;
        Site = hemocentro.Site;
        NomeFantasia = hemocentro.NomeFantasia;
        AbreEm = hemocentro.AbreEm;
        FechaEm = hemocentro.FechaEm;
        UsuarioId = hemocentro.Usuario.Id;
    }

    public Hemocentro ToDomain()
    {
        return new Hemocentro
        {
            Id = Id,
            Cnpj = Cnpj,
            Site = Site,
            NomeFantasia = NomeFantasia,
            AbreEm = AbreEm,
            FechaEm = FechaEm
        };
    }
}