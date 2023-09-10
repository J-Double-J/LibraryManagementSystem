using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public interface IApplicationDbContext
    {
        DbSet<Book> Books { get; set; }

        Task<int> SaveChanges();
    }
}