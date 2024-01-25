using RuntimeApps.UserConfig.Models;

namespace RuntimeApps.UserConfig.Interfaces {
    public interface IUserConfigStore {
        Task<UserConfigModel<TConfig>?> GetAsync<TConfig>(string key, string? user = default, CancellationToken cancellationToken = default);

        Task SetAsync<TConfig>(UserConfigModel<TConfig> userConfig, CancellationToken cancellationToken = default);

        Task ResetAsync(string key, string? userId = default, CancellationToken cancellationToken = default);
    }
}
