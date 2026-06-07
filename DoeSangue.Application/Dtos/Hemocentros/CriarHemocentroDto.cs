using DoeSangue.Application.Dtos.Enderecos;

namespace DoeSangue.Application.Dtos.Hemocentros
{
    public record CriarHemocentroDto(string Cnpj, string Site, string NomeFantasia, DateTime AbreEm, DateTime FechaEm, string Email, string Senha, EnderecoDto Endereco);
}
