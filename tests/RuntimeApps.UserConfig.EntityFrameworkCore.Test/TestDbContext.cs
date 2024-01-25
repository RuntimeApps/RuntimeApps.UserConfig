using Microsoft.EntityFrameworkCore;

namespace RuntimeApps.UserConfig.EntityFrameworkCore.Test {
    internal class TestDbContext: DbContext, IUserConfigDbContext {

        public DbSet<DbUserConfigModel> UserConfigs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            new DbUserConfigEntityConfiguration().Configure(modelBuilder.Entity<DbUserConfigModel>());
        }
    }
}
