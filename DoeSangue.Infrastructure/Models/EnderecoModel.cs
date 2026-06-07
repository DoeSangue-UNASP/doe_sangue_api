using DoeSangue.Domain.Entities;

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

    public EnderecoModel(Endereco endereco)
    {
        Id = endereco.Id;
        Cep = endereco.Cep;
        Numero = endereco.Numero;
        Rua = endereco.Rua;
        Bairro = endereco.Bairro;
        Cidade = endereco.Cidade;
        Estado = endereco.Estado;
    }

    public Endereco ToDomain()
    {
        return new Endereco()
        {
            Id = Id,
            Cep = Cep,
            Numero = Numero,
            Rua = Rua,
            Bairro = Bairro,
            Cidade = Cidade,
            Estado = Estado
        };
    }
}