using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class Agendamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime AtualizadoEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AgendadoPara { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.AGENDADO;

    public required Hemocentro Hemocentro { get; set; }
    public required Doador Doador { get; set; }
}