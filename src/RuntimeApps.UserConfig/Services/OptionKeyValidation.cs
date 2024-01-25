using Microsoft.Extensions.Options;
using RuntimeApps.UserConfig.Interfaces;
using RuntimeApps.UserConfig.Models;

namespace RuntimeApps.UserConfig.Services {
    public class OptionKeyValidation: IKeyValidation {
        private readonly IOptions<UserConfigOption> _option;

        public OptionKeyValidation(IOptions<UserConfigOption> option) => _option = option;

        public Task<bool> Validate(string key, ActionType actionType, string? userId = null, CancellationToken cancellationToken = default) {
            if(!_option.Value.ValidateKey)
                return Task.FromResult(true);

            var result = _option.Value.ValidKeys.Contains(key);
            return Task.FromResult(result);
        }
    }
}
