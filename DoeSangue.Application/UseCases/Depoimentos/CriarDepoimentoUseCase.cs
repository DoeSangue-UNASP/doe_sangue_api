using DoeSangue.Application.Dtos.Depoimentos;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Depoimentos
{
    public class CriarDepoimentoUseCase
    {
        private readonly IDepoimentoRepository _depoimentoRepository;

        public CriarDepoimentoUseCase(IDepoimentoRepository depoimentoRepository)
        {
            _depoimentoRepository = depoimentoRepository;
        }

        public async Task Executar(Usuario autor, CriarDepoimentoDto dto)
        {
            var depoimento = Depoimento.Criar(dto.Titulo, dto.Conteudo, autor);
            await _depoimentoRepository.Criar(depoimento);
        }
    }
}
