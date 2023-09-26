﻿using Domain.Entities;
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