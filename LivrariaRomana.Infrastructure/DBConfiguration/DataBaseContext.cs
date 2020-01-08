using LivrariaRomana.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LivrariaRomana.Infrastructure.DBConfiguration
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {                
                optionsBuilder.UseSqlServer(DataBaseConnection.ConnectionConfiguration.ToString());
            }
        }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
     
        }      
        public DbSet<Book> Livros { get; set; }
        public DbSet<User> Usuarios { get; set; }
    }
}
