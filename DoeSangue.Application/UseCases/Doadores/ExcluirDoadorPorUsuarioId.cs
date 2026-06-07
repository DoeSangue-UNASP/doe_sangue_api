using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class ExcluirDoadorPorUsuarioId
    {
        private readonly IDoadorRepository _doadorRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        public ExcluirDoadorPorUsuarioId(IDoadorRepository doadorRepository, IUsuarioRepository usuarioRepository, IEnderecoRepository enderecoRepository)
        {
            _doadorRepository = doadorRepository;
            _usuarioRepository = usuarioRepository;
            _enderecoRepository = enderecoRepository;
        }

        public async Task Executar(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.BuscarPorId(usuarioId);
            var enderecoId = usuario.Endereco.Id;

            await _doadorRepository.Remover(usuarioId);
            await _enderecoRepository.Remover(enderecoId);
            await _usuarioRepository.Remover(usuarioId);
        }
    }
}
