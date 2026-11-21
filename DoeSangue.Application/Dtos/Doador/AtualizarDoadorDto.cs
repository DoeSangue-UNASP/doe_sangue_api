using DoeSangue.Application.Dtos.Enderecos;

namespace DoeSangue.Application.Dtos.Doador
{
    public record AtualizarDoadorDto(string Nome, string Documento, string Telefone, DateOnly DataNascimento, EnderecoDto? Endereco = null, string? Sexo = null, string? TipoSanguineo = null);
}
