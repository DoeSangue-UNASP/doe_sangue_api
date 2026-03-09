namespace DoeSangue.Domain.Entities;

public class Depoimento
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; }

    // Tanto faz o usuário, pode ser hemocentro ou doador
    public Usuario Autor { get; set; }
}