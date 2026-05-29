namespace DoeSangue.Domain.Entities;

public class Endereco
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public string Cep { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    private Endereco(Guid id, string cep, string numero, string rua, string bairro, string cidade, string estado)
    {
        Id = id;
        Cep = cep;
        Numero = numero;
        Rua = rua;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
    }

    public static Endereco Criar(string cep, string numero, string rua, string bairro, string cidade, string estado)
    {
        var endereco = new Endereco(id: Guid.NewGuid(),
                                    cep: cep,
                                    numero: numero,
                                    rua: rua,
                                    bairro: bairro,
                                    cidade: cidade,
                                    estado: estado);

        endereco.Validar();

        return endereco;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Cep))
            throw new ArgumentException("O CEP é obrigatório.");

        if (string.IsNullOrWhiteSpace(Numero))
            throw new ArgumentException("O número é obrigatório.");

        if (string.IsNullOrWhiteSpace(Rua))
            throw new ArgumentException("A rua é obrigatória.");

        if (string.IsNullOrWhiteSpace(Bairro))
            throw new ArgumentException("O bairro é obrigatório.");

        if (string.IsNullOrWhiteSpace(Cidade))
            throw new ArgumentException("A cidade é obrigatória.");

        if (string.IsNullOrWhiteSpace(Estado))
            throw new ArgumentException("O estado é obrigatório.");
    }
}