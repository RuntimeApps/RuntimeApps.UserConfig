namespace RuntimeApps.UserConfig {
    public interface IUserConfigService {
        Task<TConfig?> GetAsync<TConfig>(string key, string? userId = default, CancellationToken cancellationToken = default);

        Task SetAsync<TConfig>(UserConfigModel<TConfig> userConfig, CancellationToken cancellationToken = default);

        Task ResetAsync(string key, string? userId = default, CancellationToken cancellationToken = default);

    }
}
