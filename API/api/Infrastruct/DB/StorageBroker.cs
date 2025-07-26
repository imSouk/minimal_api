using Microsoft.EntityFrameworkCore;
using minimal_api.api.Domain.Entities;

namespace minimal_api.api.Infrastruct.DB
{
    public class StorageBroker : DbContext
    {
        public readonly IConfiguration _config;
        public StorageBroker(IConfiguration config)
        {
            _config = config;
        }
        public DbSet<Administrator> Administrators { get; set; } = default!;
        public DbSet<Vehicle> Vehicles{ get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator
                {
                    Id = 1,
                    Email = "administrador@teste.com",
                    Senha = "123456",
                    Perfil = "Adm"
                }
            );
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured) 
            {
                string? connectionString = _config.GetConnectionString("mysql")?.ToString();
                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseMySql(
                            connectionString,
                            ServerVersion.AutoDetect(connectionString)
                        );
                }
            }
        }
    }
}
