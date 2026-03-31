using Microsoft.EntityFrameworkCore;

namespace mf_apis_web_services_fuel_manager.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Define que a tabela VeiculoUsuarios terá chave composta
            builder.Entity<VeiculoUsuarios>()
                .HasKey(c => new {c.VeiculoId, c.UsuarioId });

            // Define relacionamento: VeiculoUsuarios -> Veiculo
            // Um VeiculoUsuarios tem um Veiculo
            // Um Veiculo tem vários VeiculoUsuarios

            builder.Entity<VeiculoUsuarios>()
                .HasOne (c => c.Veiculo).WithMany(c => c.Usuarios)
                .HasForeignKey(c => c.VeiculoId);

            // Define relacionamento: VeiculoUsuarios->Usuario
            // Um VeiculoUsuarios tem um Usuario
            // Um Usuario tem vários VeiculoUsuarios

            builder.Entity<VeiculoUsuarios>()
                .HasOne(c => c.Usuario).WithMany(c => c.Veiculos)
                .HasForeignKey(c => c.UsuarioId);
        }

        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Consumo> Consumos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<VeiculoUsuarios> VeiculoUsuarios { get; set; }
    }
}
