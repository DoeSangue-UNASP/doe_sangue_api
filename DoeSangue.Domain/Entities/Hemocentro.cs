namespace DoeSangue.Domain.Entities;

public class Hemocentro
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Cnpj { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public DateTime AbreEm { get; set; }
    public DateTime FechaEm { get; set; }

    public required Usuario Usuario { get; set; }
}