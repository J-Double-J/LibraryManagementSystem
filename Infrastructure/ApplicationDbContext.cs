using Application;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }

        public DbSet<Patron> Patrons { get; set; }

        public DbSet<Checkout> Checkout { get; set; }

        public new async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Checkout>()
                .HasKey(c => c.CheckoutId);

            modelBuilder.Entity<Checkout>()
                .HasOne(c => c.Patron)
                .WithMany(p => p.Checkouts)
                .HasForeignKey(c => c.PatronId);

            modelBuilder.Entity<Checkout>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Checkouts)
                .HasForeignKey(c => c.BookId);
        }
    }
}
