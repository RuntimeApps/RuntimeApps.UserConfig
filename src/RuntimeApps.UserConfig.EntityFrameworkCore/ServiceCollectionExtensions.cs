using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RuntimeApps.UserConfig.EntityFrameworkCore {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddAddUserConfigServicesWithEFCore<TContext>(this IServiceCollection services, Action<UserConfigOption> option) where TContext : DbContext {
            services.AddUserConfigServices(option)
                .AddAddUserConfigEFCoreStore<TContext>();
            return services;
        }

        public static IServiceCollection AddAddUserConfigEFCoreStore<TContext>(this IServiceCollection services) where TContext : DbContext {
            services.TryAddScoped<IUserConfigValueSerializer, UserConfigValueJsonSerializer>();
            services.TryAddScoped<IUserConfigStore, UserConfigStore<TContext>>();
            return services;
        }
    }
}
