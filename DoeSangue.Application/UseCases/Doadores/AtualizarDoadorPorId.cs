using DoeSangue.Application.Dtos.Doador;
using DoeSangue.Domain.Entities;
using DoeSangue.Domain.Interfaces;

namespace DoeSangue.Application.UseCases.Doadores
{
    public class AtualizarDoadorPorId
    {
        private readonly IDoadorRepository _doadorRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        public AtualizarDoadorPorId(IDoadorRepository doadorRepository, IEnderecoRepository enderecoRepository)
        {
            _doadorRepository = doadorRepository;
            _enderecoRepository = enderecoRepository;
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
                UsuarioId = doadorExistente.UsuarioId,
                HemocentroId = doadorExistente.HemocentroId,
                Usuario = doadorExistente.Usuario
            };

            doador.Validar();

            Endereco? enderecoAtualizado = null;
            var enderecoExistente = doadorExistente.Usuario?.Endereco;
            if (doadorDto.Endereco is not null && enderecoExistente is not null)
            {
                enderecoAtualizado = new Endereco
                {
                    Id = enderecoExistente.Id,
                    Cep = doadorDto.Endereco.Cep,
                    Numero = doadorDto.Endereco.Numero,
                    Rua = doadorDto.Endereco.Rua,
                    Bairro = doadorDto.Endereco.Bairro,
                    Cidade = doadorDto.Endereco.Cidade,
                    Estado = doadorDto.Endereco.Estado
                };

                enderecoAtualizado.Validar();
            }

            await _doadorRepository.Atualizar(doador);

            if (enderecoAtualizado is not null)
            {
                await _enderecoRepository.Atualizar(enderecoAtualizado);
            }
        }
    }
}
