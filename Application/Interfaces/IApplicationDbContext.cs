using Domain.Entities;
using Domain.Entities.Patron;
using Microsoft.EntityFrameworkCore;

namespace Application
{
    public interface IApplicationDbContext
    {
        DbSet<Book> Books { get; set; }
        DbSet<Patron> Patrons { get; set; }

        Task<int> SaveChanges();
    }
}