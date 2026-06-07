namespace DoeSangue.Application.Dtos.Doador
{
    public record AtualizarDoadorDto(string Nome, string Documento, string Telefone, DateOnly DataNascimento);
}
