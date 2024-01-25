namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public interface IUserConfigValueSerializer {
        Task<string?> SerializeAsync<TValue>(TValue? value);
        Task<TValue?> DeserializeAsync<TValue>(string? value);
    }
}
