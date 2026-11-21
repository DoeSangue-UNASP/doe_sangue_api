using DoeSangue.Domain.Enums;

namespace DoeSangue.Infrastructure.Models;

public class BolsaSangueModel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; }
    public string TesteSorologico { get; set; }
    public string TipoComponente { get; set; }
    public string FatorRh { get; set; }
    public string TipoSanguineo { get; set; }
    public float Volume { get; set; }
    public StatusBolsa Status { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? DataColeta { get; set; }
    public DateTime? DataValidade { get; set; }
    public string? LocalColeta { get; set; }
    public string? Destino { get; set; }
    public string? ResponsavelColeta { get; set; }

    public Guid AgendamentoId { get; set; }
    public AgendamentoModel Agendamento { get; set; }

    public BolsaSangueModel() { }

    public BolsaSangueModel(Guid id,
                            string codigo,
                            string testeSorologico,
                            string tipoComponente,
                            string fatorRh,
                            string tipoSanguineo,
                            float volume,
                            StatusBolsa status,
                            DateTime criadoEm,
                            DateTime? atualizadoEm,
                            Guid agendamentoId,
                            DateTime? dataColeta = null,
                            DateTime? dataValidade = null,
                            string? localColeta = null,
                            string? destino = null,
                            string? responsavelColeta = null)
    {
        Id = id;
        Codigo = codigo;
        TesteSorologico = testeSorologico;
        TipoComponente = tipoComponente;
        FatorRh = fatorRh;
        TipoSanguineo = tipoSanguineo;
        Volume = volume;
        Status = status;
        CriadoEm = criadoEm;
        AtualizadoEm = atualizadoEm;
        AgendamentoId = agendamentoId;
        DataColeta = dataColeta;
        DataValidade = dataValidade;
        LocalColeta = localColeta;
        Destino = destino;
        ResponsavelColeta = responsavelColeta;
    }
}