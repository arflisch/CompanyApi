using Microsoft.EntityFrameworkCore;
using Domain;

namespace Database
{
    public partial class dbContext : DbContext
    {
        public dbContext() { }

        // Fix: Change the type parameter from 'Company' to 'dbContext' in DbContextOptions
        public dbContext(DbContextOptions<dbContext> options) : base(options)
        {
        }

        public virtual DbSet<Company> Companys { get; set; } = null!;
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Vat);
            });
        }
    }
}
