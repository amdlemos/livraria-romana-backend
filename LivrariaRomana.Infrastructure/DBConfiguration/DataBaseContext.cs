using LivrariaRomana.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LivrariaRomana.Infrastructure.DBConfiguration
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            if (!dbContextOptionsBuilder.IsConfigured)
            {
                dbContextOptionsBuilder.UseSqlServer(DatabaseConnection.ConnectionConfiguration.GetConnectionString("DevConnection"));
            }
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
     
        }      
        public DbSet<Book> Livros { get; set; }
        public DbSet<User> Usuarios { get; set; }
    }
}
