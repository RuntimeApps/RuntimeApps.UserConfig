using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public class DbUserConfigEntityConfiguration: IEntityTypeConfiguration<DbUserConfigModel> {
        public void Configure(EntityTypeBuilder<DbUserConfigModel> builder) {
            builder.Property(e => e.Key)
                .HasMaxLength(64)
                .IsRequired();

            builder.HasIndex(e => e.Key);

            builder.Property(e => e.UserId)
                .HasMaxLength(64);

            builder.HasIndex(e => e.UserId);

            builder.Property(e => e.Value)
                .HasMaxLength(4000);
        }
    }
}
