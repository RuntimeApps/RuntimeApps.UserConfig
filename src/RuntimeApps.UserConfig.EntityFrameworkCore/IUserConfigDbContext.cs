using Microsoft.EntityFrameworkCore;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public interface IUserConfigDbContext {
        DbSet<DbUserConfigModel> UserConfigs { get; }
    }
}
