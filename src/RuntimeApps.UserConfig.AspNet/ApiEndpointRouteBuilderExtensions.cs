using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RuntimeApps.UserConfig.AspNet {
    public static class ApiEndpointRouteBuilderExtensions {

        public static IEndpointRouteBuilder MapUserConfigApi(this IEndpointRouteBuilder endpoints) {
            var routeGroup = endpoints.MapGroup("").RequireAuthorization();

            routeGroup.MapGet("/{key}", async (string key, HttpContext httpContext, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await userConfigService.GetAsync<object>(key, userId, cancellationToken);
                return result;
            });

            routeGroup.MapPost("/{key}", async (string key, [FromBody] object body, HttpContext httpContext, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await userConfigService.SetAsync(new UserConfigModel<object> {
                    Key = key,
                    Value = body,
                    UserId = userId
                }, cancellationToken);
            });

            routeGroup.MapDelete("/{key}", async (string key, HttpContext httpContext, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await userConfigService.ResetAsync(key, userId!, cancellationToken);
            });

            return endpoints;
        }

        public static IEndpointRouteBuilder MapUserConfigAdminApi(this IEndpointRouteBuilder endpoints) {
            var routeGroup = endpoints.MapGroup("").RequireAuthorization();

            routeGroup.MapGet("/{key}", async (string key, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                var result = await userConfigService.GetAsync<object>(key, null, cancellationToken);
                return result;
            });

            routeGroup.MapPost("/{key}", async (string key, [FromBody] object body, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                await userConfigService.SetAsync(new UserConfigModel<object> {
                    Key = key,
                    Value = body,
                }, cancellationToken);
            });

            routeGroup.MapDelete("/{key}", async (string key, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                await userConfigService.ResetAsync(key, null, cancellationToken);
            });

            return endpoints;
        }

    }
}
