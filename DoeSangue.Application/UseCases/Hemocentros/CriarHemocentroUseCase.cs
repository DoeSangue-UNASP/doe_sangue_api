using DoeSangue.Application.Dtos.Hemocentros;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Hemocentros
{
    public class CriarHemocentroUseCase
    {
        private readonly IHemocentroRepository _hemocentroRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        public CriarHemocentroUseCase(IHemocentroRepository hemocentroRepository, IUsuarioRepository usuarioRepository, IEnderecoRepository enderecoRepository)
        {
            _hemocentroRepository = hemocentroRepository;
            _usuarioRepository = usuarioRepository;
            _enderecoRepository = enderecoRepository;
        }

        public async Task Executar(CriarHemocentroDto hemocentroDto)
        {
            var endereco = Endereco.Criar(hemocentroDto.Endereco.Cep,
                                          hemocentroDto.Endereco.Numero,
                                          hemocentroDto.Endereco.Rua,
                                          hemocentroDto.Endereco.Bairro,
                                          hemocentroDto.Endereco.Cidade,
                                          hemocentroDto.Endereco.Estado);

            var usuario = Usuario.Criar(hemocentroDto.Email, UsuarioRole.HEMOCENTRO, endereco);
            var hemocentro = Hemocentro.Criar(hemocentroDto.Cnpj, hemocentroDto.Site, hemocentroDto.NomeFantasia, hemocentroDto.AbreEm, hemocentroDto.FechaEm, usuario);

            endereco.Validar();
            usuario.Validar();
            hemocentro.Validar();

            await _enderecoRepository.Criar(endereco);
            await _usuarioRepository.Criar(usuario, hemocentroDto.Senha);
            await _hemocentroRepository.Criar(hemocentro);
        }
    }
}
