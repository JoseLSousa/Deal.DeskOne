using Deal.DeskOne.Application.Abstractions;
using Deal.DeskOne.Application.Abstractions.Mediator;
using Deal.DeskOne.Application.Commands.Request.CreateRequest;
using Deal.DeskOne.Domain.Abstractions;
using Deal.DeskOne.Domain.Abstractions.Repositories;
using Deal.DeskOne.Infrastructure.Data;
using Deal.DeskOne.Infrastructure.Persistence;
using Deal.DeskOne.Infrastructure.Persistence.Repositories;
using Deal.DeskOne.Infrastructure.Queries;
using Deal.DeskOne.Infrastructure.Security;
using Deal.DeskOne.Infrastructure.Services.Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deal.DeskOne.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("Postgres"));
            });

            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            services.Scan(scan => scan
                .FromAssemblies(
                    typeof(CreateRequestCommandHandler).Assembly,
                    typeof(GetRequestsQueryHandler).Assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Authentication:Authority"];
                    options.Audience = configuration["Authentication:Audience"];
                    options.RequireHttpsMetadata = false;
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.RequestReadCreate,
                    policy => policy.RequireRole("User", "Manager"));

                options.AddPolicy(AuthorizationPolicies.RequestApproveReject,
                    policy => policy.RequireRole("Manager"));
            });

            services.AddTransient<IClaimsTransformation, RealmRolesClaimsTransformation>();


            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
