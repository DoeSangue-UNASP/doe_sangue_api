using DoeSangue.Domain.Entities;
using DoeSangue.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoeSangue.Infrastructure.Data
{
    public class DoeSangueContext : IdentityDbContext<UsuarioIdentity, IdentityRole<Guid>, Guid>
    {
        public DbSet<AgendamentoModel> Agendamentos { get; set; }
        public DbSet<BolsaSangueModel> BolsasSangue { get; set; }
        public DbSet<DepoimentoModel> Depoimentos { get; set; }
        public DbSet<DoadorModel> Doadores { get; set; }
        public DbSet<EnderecoModel> Enderecos { get; set; }
        public DbSet<HemocentroModel> Hemocentros { get; set; }

        public DoeSangueContext(DbContextOptions options) : base(options)
        {
        }

        protected DoeSangueContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Agendamento
            builder.Entity<AgendamentoModel>()
                   .HasKey(x => x.Id);

            builder.Entity<AgendamentoModel>()
                   .Property(x => x.AgendadoPara);
            
            builder.Entity<AgendamentoModel>()
                   .Property(x => x.AtualizadoEm);
            
            builder.Entity<AgendamentoModel>()
                   .Property(x => x.CriadoEm);
            
            builder.Entity<AgendamentoModel>()
                   .Property(x => x.Status)
                   .HasConversion<string>();

            builder.Entity<AgendamentoModel>()
                   .HasOne(x => x.Doador)
                   .WithMany()
                   .HasForeignKey(x => x.DoadorId);
            
            builder.Entity<AgendamentoModel>()
                   .HasOne(x => x.Hemocentro)
                   .WithMany()
                   .HasForeignKey(x => x.HemocentroId)
                   .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region BolsaSangue
            builder.Entity<BolsaSangueModel>()
                   .HasKey(x => x.Id);

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.AtualizadoEm);
            
            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.Codigo);

            builder.Entity<BolsaSangueModel>()
                   .HasIndex(x => x.Codigo);

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.CriadoEm);

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.FatorRh);

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.Status)
                   .HasConversion<string>();

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.TesteSorologico);

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.TipoComponente);

            builder.Entity<BolsaSangueModel>()
                   .Property(x => x.Volume);

            builder.Entity<BolsaSangueModel>()
                   .HasOne(x => x.Agendamento)
                   .WithOne()
                   .HasForeignKey<BolsaSangueModel>(x => x.AgendamentoId);
            #endregion

            #region Depoimento
            builder.Entity<DepoimentoModel>()
                   .HasKey(x => x.Id);

            builder.Entity<DepoimentoModel>()
                   .Property(x => x.AutorId);

            builder.Entity<DepoimentoModel>()
                   .Property(x => x.CriadoEm);

            builder.Entity<DepoimentoModel>()
                   .Property(x => x.AtualizadoEm);

            builder.Entity<DepoimentoModel>()
                   .Property(x => x.Conteudo);

            builder.Entity<DepoimentoModel>()
                   .Property(x => x.CriadoEm);

            builder.Entity<DepoimentoModel>()
                   .Property(x => x.Titulo);

            builder.Entity<DepoimentoModel>()
                   .HasOne(x => x.Autor)
                   .WithMany()
                   .HasForeignKey(x => x.AutorId);
            #endregion

            #region Doador
            builder.Entity<DoadorModel>()
                   .HasKey(x => x.Id);

            builder.Entity<DoadorModel>()
                   .Property(x => x.DataNascimento);

            builder.Entity<DoadorModel>()
                   .Property(x => x.Documento);

            builder.Entity<DoadorModel>()
                   .Property(x => x.Nome);

            builder.Entity<DoadorModel>()
                   .Property(x => x.Telefone);

            builder.Entity<DoadorModel>()
                   .Property(x => x.UsuarioId);

            builder.Entity<DoadorModel>()
                   .HasOne(x => x.Usuario)
                   .WithOne()
                   .HasForeignKey<DoadorModel>(x => x.UsuarioId);
            #endregion

            #region Endereco
            builder.Entity<EnderecoModel>()
                   .HasKey(x => x.Id);

            builder.Entity<EnderecoModel>()
                   .Property(x => x.Bairro);

            builder.Entity<EnderecoModel>()
                   .Property(x => x.Cep);

            builder.Entity<EnderecoModel>()
                   .Property(x => x.Cidade);

            builder.Entity<EnderecoModel>()
                   .Property(x => x.Estado);

            builder.Entity<EnderecoModel>()
                   .Property(x => x.Numero);

            builder.Entity<EnderecoModel>()
                   .Property(x => x.Rua);
            #endregion

            #region Hemocentro
            builder.Entity<HemocentroModel>()
                   .HasKey(x => x.Id);

            builder.Entity<HemocentroModel>()
                   .Property(x => x.AbreEm);

            builder.Entity<HemocentroModel>()
                   .Property(x => x.Cnpj);

            builder.Entity<HemocentroModel>()
                   .Property(x => x.FechaEm);

            builder.Entity<HemocentroModel>()
                   .Property(x => x.NomeFantasia);

            builder.Entity<HemocentroModel>()
                   .Property(x => x.Site);

            builder.Entity<HemocentroModel>()
                   .HasOne(x => x.Usuario)
                   .WithOne()
                   .HasForeignKey<HemocentroModel>(x => x.UsuarioId);
            #endregion

            #region UsuarioIdentity
            builder.Entity<UsuarioIdentity>()
                   .Property(x => x.EnderecoId);

            builder.Entity<UsuarioIdentity>()
                   .Property(x => x.CriadoEm);

            builder.Entity<UsuarioIdentity>()
                   .Property(x => x.AtualizadoEm);

            builder.Entity<UsuarioIdentity>()
                   .HasOne(x => x.Endereco)
                   .WithOne()
                   .HasForeignKey<UsuarioIdentity>(x => x.EnderecoId);
            #endregion

            base.OnModelCreating(builder);
        }
    }
}
