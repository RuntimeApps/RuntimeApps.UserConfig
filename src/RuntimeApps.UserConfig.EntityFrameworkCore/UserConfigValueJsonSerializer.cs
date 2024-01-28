using System.Text.Json;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public class UserConfigValueJsonSerializer: IUserConfigValueSerializer {
        public virtual Task<string?> SerializeAsync<TValue>(TValue? value) {
            if(value == null)
                return Task.FromResult<string?>(null);
            string result = JsonSerializer.Serialize(value);
            return Task.FromResult<string?>(result);
        }

        public virtual Task<TValue?> DeserializeAsync<TValue>(string? value) {
            TValue? result = string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<TValue>(value!);
            return Task.FromResult(result);
        }

    }
}
