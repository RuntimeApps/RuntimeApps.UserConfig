using RuntimeApps.UserConfig.Exceptions;

namespace RuntimeApps.UserConfig.Services {
    public class UserConfigService: IUserConfigService {
        private readonly IUserConfigStore _store;
        private readonly IUserConfigValidation _userConfigValidation;
        private readonly IUserConfigCache _cacheService;

        public UserConfigService(IUserConfigStore store, IUserConfigValidation userConfigValidation, IUserConfigCache cacheService) {
            _store = store;
            _userConfigValidation = userConfigValidation;
            _cacheService = cacheService;
        }

        public virtual async Task<TValue?> GetAsync<TValue>(string key, string? userId = null, CancellationToken cancellationToken = default) {
            await CheckKey(key, ActionType.Get, userId, cancellationToken);
            var cachedItem = await _cacheService.GetAsync<TValue>(key, userId);
            if(cachedItem != null)
                return cachedItem.Value;

            var result = await _store.GetAsync<TValue>(key, userId, cancellationToken);
            if(result == null) {
                result = await GetDefaultConfig<TValue>(key, cancellationToken);
            }
            else {
                await _cacheService.SetAsync(result);
            }

            if(result == null)
                return default;
            return result.Value;
        }

        public virtual async Task ResetAsync(string key, string? userId, CancellationToken cancellationToken = default) {
            await CheckKey(key, ActionType.Reset, userId, cancellationToken);
            await _store.ResetAsync(key, userId, cancellationToken);
            await _cacheService.RemoveAsync(key, userId);
        }

        public virtual async Task SetAsync<TConfig>(UserConfigModel<TConfig> userConfig, CancellationToken cancellationToken = default) {
            await CheckKey(userConfig.Key, ActionType.Set, userConfig.UserId, cancellationToken);
            var checkValue = await _userConfigValidation.ValidateValueAsync(userConfig, cancellationToken);
            if(!checkValue)
                throw new InvalidValueModelException(userConfig.Key, userConfig.Value);

            await _store.SetAsync(userConfig, cancellationToken);
            await _cacheService.RemoveAsync(userConfig.Key, userConfig.UserId);
        }

        protected virtual async Task CheckKey(string key, ActionType actionType, string? userId = null, CancellationToken cancellationToken = default) {
            if(string.IsNullOrEmpty(key))
                throw new NullReferenceException(nameof(key));

            var result = await _userConfigValidation.ValidateKeyAsync(key, actionType, userId, cancellationToken);
            if(!result)
                throw new InvalidConfigKeyException(key);
        }

        private async Task<UserConfigModel<TValue>?> GetDefaultConfig<TValue>(string key, CancellationToken cancellationToken = default) {
            var cachedItem = await _cacheService.GetAsync<TValue>(key);
            if(cachedItem != null)
                return cachedItem;

            var result = await _store.GetAsync<TValue>(key, null, cancellationToken);
            if(result != null) {
                await _cacheService.SetAsync(result);
            }
            return result;
        }
    }
}
