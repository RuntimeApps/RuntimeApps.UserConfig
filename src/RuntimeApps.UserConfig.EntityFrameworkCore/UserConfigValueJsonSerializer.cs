using System.Text.Json;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public class UserConfigValueJsonSerializer: IUserConfigValueSerializer {
        public Task<string?> SerializeAsync<TValue>(TValue? value) {
            if(value == null)
                return Task.FromResult<string?>(null);
            string result = JsonSerializer.Serialize(value);
            return Task.FromResult<string?>(result);
        }

        public Task<TValue?> DeserializeAsync<TValue>(string? value) {
            TValue? result = string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<TValue>(value!);
            return Task.FromResult(result);
        }

    }
}
