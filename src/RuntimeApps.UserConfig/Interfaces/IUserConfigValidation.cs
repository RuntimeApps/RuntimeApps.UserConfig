namespace RuntimeApps.UserConfig {
    public interface IUserConfigValidation {
        Task<bool> ValidateKeyAsync(string key, ActionType actionType, string? userId = null, CancellationToken cancellationToken = default);
        Task<bool> ValidateValueAsync<T>(UserConfigModel<T> inputModel, CancellationToken cancellationToken);
    }
}
