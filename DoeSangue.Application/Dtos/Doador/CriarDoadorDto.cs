namespace DoeSangue.Application.Dtos.Doador
{
    public record CriarDoadorDto(string Nome, string Email, string Senha, string Documento, string Telefone, DateOnly DataNasimento, EnderecoDto Endereco);
}
