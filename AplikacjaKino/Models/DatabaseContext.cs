using AplikacjaKino.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AplikacjaKino.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Film> Filmy { get; set; }
        public DbSet<Sala> Sale { get; set; }
        public DbSet<Seans> Seanse { get; set; }
        public DbSet<Rezerwacja> Rezerwacje { get; set; }
    }
}