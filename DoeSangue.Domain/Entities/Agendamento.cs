using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class Agendamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime? AtualizadoEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AgendadoPara { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.AGENDADO;

    public Hemocentro Hemocentro { get; set; }
    public Doador Doador { get; set; }

    private Agendamento(Guid id, DateTime? atualizadoEm, DateTime criadoEm, DateTime agendadoPara, StatusAgendamento status, Hemocentro hemocentro, Doador doador)
    {
        Id = id;
        AtualizadoEm = atualizadoEm;
        CriadoEm = criadoEm;
        AgendadoPara = agendadoPara;
        Status = status;
        Hemocentro = hemocentro;
        Doador = doador;
    }

    public static Agendamento Criar(DateTime agendadoPara, Hemocentro hemocentro, Doador doador)
    {
        var agendamento = new Agendamento(id: Guid.NewGuid(),
                                          atualizadoEm: null,
                                          criadoEm: DateTime.UtcNow,
                                          agendadoPara: agendadoPara,
                                          status: StatusAgendamento.AGENDADO,
                                          hemocentro: hemocentro,
                                          doador: doador);

        agendamento.Validar();

        return agendamento;
    }

    public void Validar()
    {
        if (AgendadoPara < DateTime.UtcNow)
            throw new ArgumentException("A data do agendamento deve ser futura.");

        if (Hemocentro == null)
            throw new ArgumentException("O hemocentro é obrigatório.");

        if (Doador == null)
            throw new ArgumentException("O doador é obrigatório.");
    }
}