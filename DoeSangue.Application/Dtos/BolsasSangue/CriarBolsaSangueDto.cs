namespace DoeSangue.Application.Dtos.BolsasSangue
{
    public record CriarBolsaSangueDto(Guid AgendamentoId, string Codigo, string TesteSorologico, string TipoComponente, string FatorRh, string TipoSanguineo, float Volume);
}
