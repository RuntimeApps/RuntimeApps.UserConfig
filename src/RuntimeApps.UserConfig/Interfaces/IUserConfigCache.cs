namespace RuntimeApps.UserConfig {
    public interface IUserConfigCache {
        Task<UserConfigModel<TValue>?> GetAsync<TValue>(string key, string? userId = null);
        Task SetAsync<TValue>(UserConfigModel<TValue> model);
        Task RemoveAsync(string key, string? userId);
    }
}
