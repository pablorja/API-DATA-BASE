using Microsoft.EntityFrameworkCore;
using API_CON_DB.Models;

namespace API_CON_DB.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cafe> Cafes => Set<Cafe>();
    }
}
