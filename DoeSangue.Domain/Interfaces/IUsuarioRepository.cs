using DoeSangue.Domain.Entities;

namespace DoeSangue.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task Criar(Usuario usuario, string password);
        Task AtualizarEndereco(Endereco endereco);
        Task<Usuario?> BuscarPorId(Guid usuarioId);
        Task Remover(Guid usuarioId);
    }
}
