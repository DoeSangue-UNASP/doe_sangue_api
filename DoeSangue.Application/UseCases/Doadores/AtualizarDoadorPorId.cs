using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class AtualizarDoadorPorId
    {
        private readonly IDoadorRepository _doadorRepository;

        public AtualizarDoadorPorId(IDoadorRepository doadorRepository)
        {
            _doadorRepository = doadorRepository;
        }

        public async Task Executar(Guid doadorId, AtualizarDoadorDto doadorDto)
        {
            var doadorExistente = await _doadorRepository.BuscarPorId(doadorId);

            if (doadorExistente is null)
            {
                throw new ArgumentException("Doador não encontrado.");
            }

            var doador = new Doador
            {
                Id = doadorId,
                Nome = doadorDto.Nome,
                DataNascimento = doadorDto.DataNascimento,
                Documento = doadorDto.Documento,
                Telefone = doadorDto.Telefone,
                Usuario = doadorExistente.Usuario
            };

            doador.Validar();

            await _doadorRepository.Atualizar(doador);
        }
    }
}
