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
                            Guid agendamentoId)
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
    }
}