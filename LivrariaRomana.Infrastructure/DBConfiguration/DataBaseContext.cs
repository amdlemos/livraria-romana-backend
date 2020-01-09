using LivrariaRomana.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivrariaRomana.Infrastructure.DBConfiguration
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {                
                optionsBuilder.UseSqlServer(DataBaseConnection.ConnectionConfiguration);
            }
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
     
        }      
        public DbSet<Book> Livros { get; set; }
        public DbSet<User> Usuarios { get; set; }
    }
}
