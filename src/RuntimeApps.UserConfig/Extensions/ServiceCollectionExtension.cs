using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RuntimeApps.UserConfig.Services;

namespace RuntimeApps.UserConfig {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddUserConfigServices(this IServiceCollection services, Action<UserConfigOption> option) {
            services.TryAddScoped<IUserConfigValidation, OptionUserConfigValidation>();
            services.TryAddScoped<IUserConfigCache, UserConfigCache>();
            services.TryAddScoped<IUserConfigService, UserConfigService>();

            services.Configure(option);
            return services;
        }

    }
}
