using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class CadastrarLeadUseCase
    {
        private readonly IDoadorRepository _doadorRepository;
        private readonly IHemocentroRepository _hemocentroRepository;

        public CadastrarLeadUseCase(IDoadorRepository doadorRepository, IHemocentroRepository hemocentroRepository)
        {
            _doadorRepository = doadorRepository;
            _hemocentroRepository = hemocentroRepository;
        }

        public async Task Executar(CadastrarLeadDto leadDto, Guid usuarioHemocentroId)
        {
            var hemocentro = await _hemocentroRepository.BuscarPorUsuarioId(usuarioHemocentroId);
            if (hemocentro is null)
            {
                throw new InvalidOperationException("Hemocentro não encontrado.");
            }

            var lead = Doador.CriarLead(leadDto.Nome, leadDto.Documento, leadDto.Telefone, leadDto.DataNascimento, hemocentro.Id);
            lead.Validar();

            await _doadorRepository.Criar(lead);
        }
    }
}
