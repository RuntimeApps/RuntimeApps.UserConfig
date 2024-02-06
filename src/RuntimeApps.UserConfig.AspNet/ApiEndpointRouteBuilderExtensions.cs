using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RuntimeApps.UserConfig.Exceptions;

namespace RuntimeApps.UserConfig.AspNet {
    public static class ApiEndpointRouteBuilderExtensions {

        public static IEndpointRouteBuilder MapUserConfigApi(this IEndpointRouteBuilder endpoints, bool throwKnownException = false) {
            var routeGroup = endpoints.MapGroup("").RequireAuthorization();

            routeGroup.MapGet("/{key}", async Task<Results<Ok<object>, ValidationProblem>> (string key, HttpContext httpContext, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                try {
                    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var result = await userConfigService.GetAsync<object>(key, userId, cancellationToken);
                    return TypedResults.Ok(result);
                }
                catch(InvalidConfigKeyException keyException) {
                    if(throwKnownException)
                        throw;
                    return KeyNotFoundProblem(keyException.Key);
                }
                catch(InvalidValueModelException) {
                    if(throwKnownException)
                        throw;
                    return InvalidValueroblem();
                }
            });

            routeGroup.MapGet("/{key}/default", async Task<Results<Ok<object>, ValidationProblem>> (string key, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                try{
                    var result = await userConfigService.GetAsync<object>(key, null, cancellationToken);
                    return TypedResults.Ok(result);
                } catch(InvalidConfigKeyException keyException) {
                    if(throwKnownException)
                        throw; 
                    return KeyNotFoundProblem(keyException.Key);
                } catch(InvalidValueModelException) {
                    if(throwKnownException)
                        throw; 
                    return InvalidValueroblem();
                }
            });

            routeGroup.MapPost("/{key}", async Task<Results<Ok, ValidationProblem>> (string key, [FromBody] object body, HttpContext httpContext, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                try {
                    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await userConfigService.SetAsync(new UserConfigModel<object> {
                        Key = key,
                        Value = body,
                        UserId = userId
                    }, cancellationToken);
                    return TypedResults.Ok();
                } catch(InvalidConfigKeyException keyException) {
                    if(throwKnownException)
                        throw; 
                    return KeyNotFoundProblem(keyException.Key);
                } catch(InvalidValueModelException) {
                    if(throwKnownException)
                        throw; 
                    return InvalidValueroblem();
                }
            });

            routeGroup.MapDelete("/{key}", async Task<Results<Ok, ValidationProblem>> (string key, HttpContext httpContext, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                try{
                    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await userConfigService.ResetAsync(key, userId!, cancellationToken);
                    return TypedResults.Ok();
                } catch(InvalidConfigKeyException keyException) {
                    if(throwKnownException)
                        throw; 
                    return KeyNotFoundProblem(keyException.Key);
                } catch(InvalidValueModelException) {
                    if(throwKnownException)
                        throw; 
                    return InvalidValueroblem();
                }
            });

            return endpoints;
        }

        public static IEndpointRouteBuilder MapUserConfigAdminApi(this IEndpointRouteBuilder endpoints, bool throwKnownException = false) {
            var routeGroup = endpoints.MapGroup("").RequireAuthorization();

            routeGroup.MapPost("/{key}/default", async Task<Results<Ok, ValidationProblem>> (string key, [FromBody] object body, IUserConfigService userConfigService, CancellationToken cancellationToken) => {
                try{
                    await userConfigService.SetAsync(new UserConfigModel<object> {
                        Key = key,
                        Value = body,
                    }, cancellationToken);
                    return TypedResults.Ok();
                } catch(InvalidConfigKeyException keyException) {
                    if(throwKnownException)
                        throw; 
                    return KeyNotFoundProblem(keyException.Key);
                } catch(InvalidValueModelException) {
                    if(throwKnownException)
                        throw; 
                    return InvalidValueroblem();
                }
            });

            return endpoints;
        }

        private static ValidationProblem KeyNotFoundProblem(string? key) =>
            TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { "InValidKey", new string[] { $"Key {key} is invalid or access is denied!" } }
        });

        private static ValidationProblem InvalidValueroblem() =>
            TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { "InValidValue", new string[] { $"Value is invalid! Please send correct value." } }
        });
    }
}
