using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Enums;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class CriarDoadorUseCase
    {
        private readonly IDoadorRepository _doadorRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        public CriarDoadorUseCase(IDoadorRepository doadorRepository, IUsuarioRepository usuarioRepository, IEnderecoRepository enderecoRepository)
        {
            _doadorRepository = doadorRepository;
            _usuarioRepository = usuarioRepository;
            _enderecoRepository = enderecoRepository;
        }

        public async Task Executar(CriarDoadorDto doadorDto)
        {

            var endereco = Endereco.Criar(doadorDto.Endereco.Cep,
                                          doadorDto.Endereco.Numero,
                                          doadorDto.Endereco.Rua,
                                          doadorDto.Endereco.Bairro,
                                          doadorDto.Endereco.Cidade,
                                          doadorDto.Endereco.Estado);

            var usuario = Usuario.Criar(doadorDto.Email, UsuarioRole.DOADOR, endereco);
            var doador = Doador.Criar(doadorDto.Nome, doadorDto.Documento, doadorDto.Telefone, doadorDto.DataNasimento, usuario);

            endereco.Validar();
            usuario.Validar();
            doador.Validar();

            await _enderecoRepository.Criar(endereco);
            await _usuarioRepository.Criar(usuario, doadorDto.Senha);
            await _doadorRepository.Criar(doador);
        }
    }
}
