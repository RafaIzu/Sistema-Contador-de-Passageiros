
using Microsoft.EntityFrameworkCore;

namespace BusPrime.Models
{
    public partial class ModelDBContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public ModelDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual DbSet<Onibus> Onibus { get; set; }
        public virtual DbSet<Motorista> Motorista { get; set; }
        public virtual DbSet<Contagem> Contagem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(Configuration.GetConnectionString("DBConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contagem>()
                .HasOne(x => x.Onibus)
                .WithMany(x => x.Contagem)
                .HasForeignKey(x => x.Onibus_Id);

            modelBuilder.Entity<Contagem>()
                .HasOne(x => x.Motorista)
                .WithMany(x => x.Contagem)
                .HasForeignKey(x => x.Motorista_Id);

            OnModelCreatingPartial(modelBuilder);

        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
