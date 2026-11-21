namespace DoeSangue.Application.Dtos.Doador
{
    public record AtualizarDoadorPeloHemocentroDto(string Nome, string Documento, string Telefone, DateOnly DataNascimento, string? Sexo = null, string? TipoSanguineo = null);
}
