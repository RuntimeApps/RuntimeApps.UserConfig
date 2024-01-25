using Microsoft.EntityFrameworkCore;
using RuntimeApps.UserConfig.Models;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public interface IUserConfigDbContext {
        DbSet<DbUserConfigModel> UserConfigs { get; }
    }
}
