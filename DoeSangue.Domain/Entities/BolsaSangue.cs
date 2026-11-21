using DoeSangue.Domain.Enums;

namespace DoeSangue.Domain.Entities;

public class BolsaSangue
{
    public BolsaSangue() { }

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
    public DateTime? DataColeta { get; set; }
    public DateTime? DataValidade { get; set; }
    public string? LocalColeta { get; set; }
    public string? Destino { get; set; }
    public string? ResponsavelColeta { get; set; }

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
                        Agendamento agendamento,
                        DateTime? dataColeta,
                        DateTime? dataValidade,
                        string? localColeta,
                        string? destino,
                        string? responsavelColeta)
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
        DataColeta = dataColeta;
        DataValidade = dataValidade;
        LocalColeta = Normalizar(localColeta);
        Destino = Normalizar(destino);
        ResponsavelColeta = Normalizar(responsavelColeta);
    }

    public static BolsaSangue Criar(string codigo,
                                    string testeSorologico,
                                    string tipoComponente,
                                    string fatorRh,
                                    string tipoSanguineo,
                                    float volume,
                                    Agendamento agendamento,
                                    DateTime? dataColeta,
                                    DateTime? dataValidade,
                                    string? localColeta = null,
                                    string? destino = null,
                                    string? responsavelColeta = null)
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
                                    agendamento: agendamento,
                                    dataColeta: dataColeta,
                                    dataValidade: dataValidade,
                                    localColeta: localColeta,
                                    destino: destino,
                                    responsavelColeta: responsavelColeta);

        bolsa.Validar();

        return bolsa;
    }

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

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

        if (DataColeta is null)
            throw new ArgumentException("A data de coleta é obrigatória.");

        if (DataValidade is null)
            throw new ArgumentException("A data de validade é obrigatória.");

        if (DataValidade < DataColeta)
            throw new ArgumentException("A data de validade deve ser posterior à data de coleta.");

        if (Agendamento == null)
            throw new ArgumentException("O agendamento associado é obrigatório.");
    }

    public void AtualizarStatus(StatusBolsa novoStatus)
    {
        var transicaoPermitida = Status switch
        {
            StatusBolsa.EM_ANALISE => novoStatus is StatusBolsa.DISPONIVEL or StatusBolsa.DESCARTADA or StatusBolsa.VENCIDA,
            StatusBolsa.DISPONIVEL => novoStatus is StatusBolsa.DESCARTADA or StatusBolsa.VENCIDA,
            _ => false
        };

        if (!transicaoPermitida)
        {
            throw new ArgumentException($"Não é permitido alterar o status de {Status} para {novoStatus}.");
        }

        Status = novoStatus;
        AtualizadoEm = DateTime.UtcNow;
    }
}