namespace DoeSangue.Application.Dtos.Agendamentos
{
    public record CriarAgendamentoDto(Guid DoadorId, Guid HemocentroId, DateTime AgendadoPara);
}
