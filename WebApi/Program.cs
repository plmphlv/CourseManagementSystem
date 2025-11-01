using Application;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text.Json.Serialization;
using WebApi.Filters;
using WebApi.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(c => c.Filters.Add(new ApiExceptionFilterAttribute()))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        builder.Services.AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddHttpContextAccessor()
            .AddOpenApiDocument(configure =>
            {
                configure.Title = "CourseManagementSystem Web.API";
                configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });
                configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });


        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

        try
        {
            using IServiceScope scope = app.Services.CreateScope();

            IApplicationDbContext context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            await ApplicationDbContextSeed.SeedRoles(roleManager);

            if (app.Environment.IsDevelopment())
            {
                await ApplicationDbContextSeed.SeedDevelopmentData((ApplicationDbContext)context, userManager, builder.Configuration);
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        try
        {
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}