using Microsoft.EntityFrameworkCore;
using WeekInDotnet.Models;

namespace WeekInDotnet.Data
{
    public class LinksContext : DbContext
    {
        public LinksContext(DbContextOptions<LinksContext> options) : base(options)
        { }

        public DbSet<Link> Links { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>().ForSqlServerToTable("Link");
        }
    }
}
