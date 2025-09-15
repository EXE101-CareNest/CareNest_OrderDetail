using CareNest_OrderDetail.Domain.Entitites;
using Microsoft.EntityFrameworkCore;


namespace CareNest_OrderDetail.Infrastructure.Persistences.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
