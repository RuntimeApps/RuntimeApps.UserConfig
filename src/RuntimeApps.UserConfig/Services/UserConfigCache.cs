﻿using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RuntimeApps.UserConfig.Interfaces;
using RuntimeApps.UserConfig.Models;

namespace RuntimeApps.UserConfig.Services {
    public class UserConfigCache: IUserConfigCache {
        private readonly IOptions<UserConfigOption> _option;
        private readonly IMemoryCache? _memoryCache = null;

        public UserConfigCache(IOptions<UserConfigOption> option, IMemoryCache? memoryCache = null) {
            _option = option;
            _memoryCache = memoryCache;

            if(option.Value.UseCache && memoryCache == null)
                throw new NullReferenceException(nameof(memoryCache));
        }

        public Task<UserConfigModel<TValue>?> GetAsync<TValue>(string key, string? userId) {
            if(!_option.Value.UseCache)
                return Task.FromResult<UserConfigModel<TValue>?>(default);

            if(_memoryCache!.TryGetValue<UserConfigModel<TValue>>(GetCacheKey(key, userId), out var value))
                return Task.FromResult(value);

            return Task.FromResult<UserConfigModel<TValue>?>(default);
        }

        public Task RemoveAsync(string key, string? userId) {
            if(!_option.Value.UseCache)
                return Task.CompletedTask;

            string cacheKey = GetCacheKey(key, userId);
            _memoryCache!.Remove(cacheKey);
            return Task.CompletedTask;
        }

        public Task SetAsync<TValue>(UserConfigModel<TValue> model) {
            if(!_option.Value.UseCache)
                return Task.CompletedTask;

            string cacheKey = GetCacheKey(model.Key, model.UserId);
            _memoryCache!.Set(cacheKey, model);
            return Task.CompletedTask;
        }

        protected virtual string GetCacheKey(string key, string? userId) {
            StringBuilder builder = new();
            builder.Append(_option.Value.CachePrefix);
            builder.Append('_');
            builder.Append(key);
            if(userId != null) {
                builder.Append('_');
                builder.Append(userId);
            }

            return builder.ToString();
        }
    }
}