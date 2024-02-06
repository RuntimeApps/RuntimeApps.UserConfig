using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RuntimeApps.UserConfig.EntityFrameworkCore;

namespace RuntimeApps.UserConfig.AspNetSample {
    public class ApplicationDbContext: IdentityDbContext, IUserConfigDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DbUserConfigModel> UserConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            new DbUserConfigEntityConfiguration().Configure(builder.Entity<DbUserConfigModel>());

            builder.Entity<IdentityRole>().HasData(new IdentityRole() {
                Id = Guid.NewGuid().ToString(),
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole() {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            builder.Entity<DbUserConfigModel>().HasData(new DbUserConfigModel {
                Id = 1,
                Key = "Key-Changeable",
                Value = "{\"edit\":true}",
                CreateDate = new System.DateTime(2024, 1, 1)
            }, new DbUserConfigModel {
                Id = 2,
                Key = "Key-Admin-Change",
                Value = "\"Admin message\"",
                CreateDate = new System.DateTime(2024, 1, 1)
            }, new DbUserConfigModel {
                Id = 3,
                Key = "Key-Not-Changeable",
                Value = "{\"canEdit\": false}",
                CreateDate = new System.DateTime(2024, 1, 1)
            });
        }
    }
}
