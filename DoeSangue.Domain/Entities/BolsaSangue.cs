using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class BolsaSangue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Codigo { get; set; } = string.Empty;
    public string TesteSorologico { get; set; } = string.Empty;
    public string TipoComponente { get; set; } = string.Empty;
    public string FatorRh { get; set; } = string.Empty;
    public string TipoSanguineo { get; set; } = string.Empty;
    public float Volume { get; set; }
    public StatusBolsa Status { get; set; } = StatusBolsa.EM_ANALISE;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    public Agendamento Agendamento { get; set; }

    private BolsaSangue(Guid id,
                        string codigo,
                        string testeSorologico,
                        string tipoComponente,
                        string fatorRh,
                        string tipoSanguineo,
                        float volume,
                        StatusBolsa status,
                        DateTime criadoEm,
                        DateTime? atualizadoEm,
                        Agendamento agendamento)
    {
        Id = id;
        Codigo = codigo;
        TesteSorologico = testeSorologico;
        TipoComponente = tipoComponente;
        FatorRh = fatorRh;
        TipoSanguineo = tipoSanguineo;
        Volume = volume;
        Status = status;
        CriadoEm = criadoEm;
        AtualizadoEm = atualizadoEm;
        Agendamento = agendamento;
    }

    public static BolsaSangue Criar(string codigo,
                                    string testeSorologico,
                                    string tipoComponente,
                                    string fatorRh,
                                    string tipoSanguineo,
                                    float volume,
                                    Agendamento agendamento)
    {
        var bolsa = new BolsaSangue(id: Guid.NewGuid(),
                                    codigo: codigo,
                                    testeSorologico: testeSorologico,
                                    tipoComponente: tipoComponente,
                                    fatorRh: fatorRh,
                                    tipoSanguineo: tipoSanguineo,
                                    volume: volume,
                                    status: StatusBolsa.EM_ANALISE,
                                    criadoEm: DateTime.UtcNow,
                                    atualizadoEm: null,
                                    agendamento: agendamento);

        bolsa.Validar();

        return bolsa;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Codigo))
            throw new ArgumentException("O código da bolsa de sangue é obrigatório.");

        if (string.IsNullOrWhiteSpace(TesteSorologico))
            throw new ArgumentException("O teste sorológico é obrigatório.");

        if (string.IsNullOrWhiteSpace(TipoComponente))
            throw new ArgumentException("O tipo de componente é obrigatório.");

        if (string.IsNullOrWhiteSpace(FatorRh))
            throw new ArgumentException("O fator Rh é obrigatório.");

        if (string.IsNullOrWhiteSpace(TipoSanguineo))
            throw new ArgumentException("O tipo sanguíneo é obrigatório.");

        if (Volume <= 0)
            throw new ArgumentException("O volume deve ser maior que zero.");

        if (Agendamento == null)
            throw new ArgumentException("O agendamento associado é obrigatório.");
    }
}