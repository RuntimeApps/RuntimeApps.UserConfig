using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RuntimeApps.Authentication.Controller;
using RuntimeApps.Authentication.EF.Extensions;
using RuntimeApps.Authentication.Extensions;
using RuntimeApps.Authentication.Model;
using RuntimeApps.UserConfig.AspNet;
using RuntimeApps.UserConfig.EntityFrameworkCore;
using RuntimeApps.UserConfig.AspNetSample;
using RuntimeApps.Authentication.Interface;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("UserConfig_AspNetSample"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddRuntimeAppsAuthentication<IdentityUser, IdentityRole, string>()
    .AddEfStores<ApplicationDbContext, IdentityUser, IdentityRole, string>()
    .UseJwt(JwtBearerDefaults.AuthenticationScheme, option => {
        SymmetricSecurityKey signingKey = new(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!));
        option.RequireHttpsMetadata = false;
        option.SaveToken = true;
        option.RefreshOnIssuerKeyNotFound = false;
        option.RefreshInterval = TimeSpan.FromMinutes(int.Parse(builder.Configuration["Jwt:ExpireInMinute"]!));
        option.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
        };
    })
    .AddValidators();

builder.Services.AddAuthorization();

builder.Services.AddAddUserConfigServicesWithEFCore<ApplicationDbContext>(option => {
    builder.Configuration.GetSection("UserConfigOption").Bind(option);
});

builder.Services.AddAutoMapper(conf => {
    conf.AddProfile<IdentityUserMapper<IdentityUser, IdentityUserDto, string>>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "User Config Sample",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
});

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
    using(var scope = app.Services.CreateScope()) {
        var services = scope.ServiceProvider;
        try {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            if(!context.Users.Any()) {
                var userManager = services.GetRequiredService<IUserManager<IdentityUser>>();
                var user = new IdentityUser("Admin");
                var addUserResult = await userManager.CreateAsync(user, "@Admin123");
                if(!addUserResult.Succeeded)
                    throw new Exception($"Add user error: {addUserResult}");

                var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if(!addRoleResult.Succeeded)
                    throw new Exception($"Add role error: {addRoleResult}");
            }
        } catch(Exception ex) {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api")
    .WithTags("Authentication APIs")
    .MapLoginApi<IdentityUser, IdentityUserDto, string>()
    .MapRegisterApi<IdentityUser, IdentityUserDto, string>();

app.MapGroup("api/account")
    .WithTags("Account APIs")
    .MapAccountApi<IdentityUser, IdentityRole, string>();

app.MapGroup("api/account/setting")
    .WithTags("User settings")
    .MapUserConfigApi();

app.MapGroup("api/account/setting")
    .WithTags("User Admin settings")
    .RequireAuthorization(config => config.RequireRole("Admin"))
    .MapUserConfigAdminApi();

app.Run();
