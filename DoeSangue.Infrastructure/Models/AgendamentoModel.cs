using DoeSangue.Domain.Enums;

namespace DoeSangue.Infrastructure.Models;

public class AgendamentoModel
{
    public Guid Id { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AgendadoPara { get; set; }
    public StatusAgendamento Status { get; set; }

    public Guid HemocentroId { get; set; }
    public HemocentroModel Hemocentro { get; set; }
    public Guid DoadorId { get; set; }
    public DoadorModel Doador { get; set; }

    public AgendamentoModel() { }

    public AgendamentoModel(Guid id,
                            DateTime? atualizadoEm,
                            DateTime criadoEm,
                            DateTime agendadoPara,
                            StatusAgendamento status,
                            Guid hemocentroId,
                            HemocentroModel hemocentro,
                            Guid doadorId)
    {
        Id = id;
        AtualizadoEm = atualizadoEm;
        CriadoEm = criadoEm;
        AgendadoPara = agendadoPara;
        Status = status;
        HemocentroId = hemocentroId;
        Hemocentro = hemocentro;
        DoadorId = doadorId;
    }
}