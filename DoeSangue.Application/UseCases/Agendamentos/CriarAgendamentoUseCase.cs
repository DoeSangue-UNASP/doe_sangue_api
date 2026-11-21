using DoeSangue.Application.Dtos.Agendamentos;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Agendamentos
{
    public class CriarAgendamentoUseCase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IDoadorRepository _doadorRepository;
        private readonly IHemocentroRepository _hemocentroRepository;

        public CriarAgendamentoUseCase(IAgendamentoRepository agendamentoRepository, IDoadorRepository doadorRepository, IHemocentroRepository hemocentroRepository)
        {
            _agendamentoRepository = agendamentoRepository;
            _doadorRepository = doadorRepository;
            _hemocentroRepository = hemocentroRepository;
        }

        public async Task Executar(Guid usuarioId, CriarAgendamentoDto dto)
        {
            var doador = await _doadorRepository.BuscarPorId(dto.DoadorId);
            if (doador is null)
            {
                throw new ArgumentException("Doador inexistente.");
            }

            if (!doador.EhUsuario || doador.UsuarioId != usuarioId)
            {
                throw new UnauthorizedAccessException("O agendamento deve ser criado pelo próprio doador. Apenas usuários cadastrados podem agendar doações.");
            }

            var hemocentro = await _hemocentroRepository.BuscarPorId(dto.HemocentroId);
            if (hemocentro is null)
            {
                throw new ArgumentException("Hemocentro inexistente.");
            }

            var horario = TimeOnly.FromTimeSpan(dto.AgendadoPara.TimeOfDay);
            var abertura = hemocentro.AbreEm;
            var fechamento = hemocentro.FechaEm;
            var horarioDisponivel = fechamento != abertura &&
                (fechamento > abertura
                    ? horario >= abertura && horario < fechamento
                    : horario >= abertura || horario < fechamento);

            if (!horarioDisponivel)
            {
                throw new ArgumentException("O horário solicitado está fora do horário de funcionamento do hemocentro.");
            }

            if (await _agendamentoRepository.ExisteAgendamentoAtivo(hemocentro.Id, dto.AgendadoPara))
            {
                throw new ArgumentException("Já existe um agendamento ativo para esse horário.");
            }

            var agendamento = Agendamento.Criar(dto.AgendadoPara, hemocentro, doador);

            await _agendamentoRepository.Criar(agendamento);
        }
    }
}

