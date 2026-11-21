using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    // Leads pertencem ao hemocentro que os cadastrou e podem ser editados por completo.
    // Para doadores usuários, o hemocentro só registra sexo e tipo sanguíneo; os demais dados são do próprio doador.
    public class AtualizarDoadorPeloHemocentroUseCase
    {
        private readonly IDoadorRepository _doadorRepository;

        public AtualizarDoadorPeloHemocentroUseCase(IDoadorRepository doadorRepository)
        {
            _doadorRepository = doadorRepository;
        }

        public async Task Executar(Guid doadorId, Guid usuarioHemocentroId, AtualizarDoadorPeloHemocentroDto dto)
        {
            var doador = await _doadorRepository.BuscarPorIdDoHemocentro(doadorId, usuarioHemocentroId);
            if (doador is null)
            {
                throw new ArgumentException("Doador não encontrado.");
            }

            if (doador.EhLead)
            {
                doador.Nome = dto.Nome;
                doador.Documento = dto.Documento;
                doador.Telefone = dto.Telefone;
                doador.DataNascimento = dto.DataNascimento;
            }

            doador.AtualizarDadosClinicos(dto.Sexo ?? doador.Sexo, dto.TipoSanguineo ?? doador.TipoSanguineo);

            await _doadorRepository.Atualizar(doador);
        }
    }
}
