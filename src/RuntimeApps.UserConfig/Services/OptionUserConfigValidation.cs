using Microsoft.Extensions.Options;

namespace RuntimeApps.UserConfig.Services {
    public class OptionUserConfigValidation: IUserConfigValidation {
        private readonly IOptions<UserConfigOption> _option;

        public OptionUserConfigValidation(IOptions<UserConfigOption> option) => _option = option;

        public virtual Task<bool> ValidateKeyAsync(string key, ActionType actionType, string? userId = null, CancellationToken cancellationToken = default) {
            if(!_option.Value.ValidateKey)
                return Task.FromResult(true);

            var result = _option.Value.ValidKeys.Contains(key);
            if(result)
                return Task.FromResult(result);

            result = _option.Value.ValidReadonlyKeys.Contains(key);
            if(result && actionType == ActionType.Get)
                return Task.FromResult(result);

            result = _option.Value.ValidDefaultKeus.Contains(key);
            if(result &&
                (actionType == ActionType.Get || string.IsNullOrEmpty(userId)))
                return Task.FromResult(result);

            return Task.FromResult(false);
        }

        public virtual Task<bool> ValidateValueAsync<T>(UserConfigModel<T> inputModel, CancellationToken cancellationToken) =>
            Task.FromResult(true);
    }
}
