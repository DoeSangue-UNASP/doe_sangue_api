namespace DoeSangue.Application.Dtos.Doador
{
    public record CadastrarLeadDto(string Nome, string Documento, string Telefone, DateOnly DataNascimento, string? Sexo = null, string? TipoSanguineo = null);
}
