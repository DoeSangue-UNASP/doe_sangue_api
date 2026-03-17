using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class BolsaSangue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Codigo { get; set; } = string.Empty;
    public string TesteSorologico { get; set; } = string.Empty;
    public string TipoComponente { get; set; } = string.Empty;
    public string FatorRh { get; set; } = string.Empty;
    public string TipoSanguineo { get; set; } = string.Empty;
    public float Volume { get; set; }
    public StatusBolsa Status { get; set; } = StatusBolsa.EM_ANALISE;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    public required Agendamento Agendamento { get; set; }
}