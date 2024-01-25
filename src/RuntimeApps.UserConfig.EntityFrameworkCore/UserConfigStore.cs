using Microsoft.EntityFrameworkCore;
using RuntimeApps.UserConfig.Interfaces;
using RuntimeApps.UserConfig.Models;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public class UserConfigStore<TContext>: IUserConfigStore where TContext : DbContext {
        private readonly TContext _dbContext;
        private readonly IUserConfigValueSerializer _serializer;

        public UserConfigStore(TContext dbContext, IUserConfigValueSerializer serializer) {
            _dbContext = dbContext;
            _serializer = serializer;
        }

        protected virtual DbSet<DbUserConfigModel> UserConfigs { get => _dbContext.Set<DbUserConfigModel>(); }

        public virtual async Task<UserConfigModel<TConfig>?> GetAsync<TConfig>(string key, string? userId, CancellationToken cancellationToken = default) {
            DbUserConfigModel? result = await FindAsync(key, userId, cancellationToken);
            if(result == default)
                return default;

            return new UserConfigModel<TConfig> {
                Key = result.Key,
                UserId = result.UserId,
                Value = await _serializer.DeserializeAsync<TConfig?>(result.Value)
            };
        }

        protected virtual Task<DbUserConfigModel?> FindAsync(string key, string? userId, CancellationToken cancellationToken = default) =>
            UserConfigs
                .Where(uc => uc.Key.Equals(uc.Key, StringComparison.OrdinalIgnoreCase) && uc.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

        public virtual async Task ResetAsync(string key, string? userId = default, CancellationToken cancellationToken = default) {
            DbUserConfigModel? dbUserConfig = await FindAsync(key, userId, cancellationToken);
            if(dbUserConfig == null)
                return;

            UserConfigs.Remove(dbUserConfig);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task SetAsync<TConfig>(UserConfigModel<TConfig> userConfig, CancellationToken cancellationToken = default) {
            DbUserConfigModel? dbUserConfig = await FindAsync(userConfig.Key, userConfig.UserId, cancellationToken);
            if(dbUserConfig == default) {
                DbUserConfigModel insertModel = new() {
                    Key = userConfig.Key,
                    UserId = userConfig.UserId,
                    Value = await _serializer.SerializeAsync(userConfig.Value)
                };

                await UserConfigs.AddAsync(insertModel, cancellationToken);
            }
            else {
                dbUserConfig.Value = await _serializer.SerializeAsync(userConfig.Value);
                UserConfigs.Update(dbUserConfig);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
