using RuntimeApps.UserConfig.Models;

namespace RuntimeApps.UserConfig.Interfaces {
    public interface IKeyValidation {
        Task<bool> Validate(string key, ActionType actionType, string? userId = null, CancellationToken cancellationToken = default);
    }
}
