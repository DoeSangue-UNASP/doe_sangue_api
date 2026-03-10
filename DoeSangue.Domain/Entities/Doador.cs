namespace DoeSangue.Domain.Entities;

public class Doador
{
    public Guid Id { get; set; }  = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }

    public required Usuario Usuario { get; set; }
}