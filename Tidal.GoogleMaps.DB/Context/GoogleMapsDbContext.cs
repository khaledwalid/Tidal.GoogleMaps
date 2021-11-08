using Microsoft.EntityFrameworkCore;
using Tidal.GoogleMaps.DB.Models;

namespace Tidal.GoogleMaps.DB.Context
{
    public class GoogleMapsDbContext : DbContext
    {
        public GoogleMapsDbContext()
        {
        }
        public GoogleMapsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseNpgsql("User ID =postgres;Password=123;Server=localhost;Port=5432;Database=Tidal;Integrated Security=true;Pooling=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #region Models

        public DbSet<Map> Maps { get; set; }

        #endregion


    }
}