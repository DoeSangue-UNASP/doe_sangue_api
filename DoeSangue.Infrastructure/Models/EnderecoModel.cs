namespace DoeSangue.Infrastructure.Models;

public class EnderecoModel
{
    public Guid Id { get; set; }
    public string Cep { get; set; }
    public string Numero { get; set; }
    public string Rua { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }

    public EnderecoModel() { }
    public EnderecoModel(Guid id, string cep, string numero, string rua, string bairro, string cidade, string estado)
    {
        Id = id;
        Cep = cep;
        Numero = numero;
        Rua = rua;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
    }
}