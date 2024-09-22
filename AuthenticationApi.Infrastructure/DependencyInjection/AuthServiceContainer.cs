using AuthenticationApi.Application.Interface;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.SharedLibrary.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class AuthServiceContainer
    {
        public static IServiceCollection AddInfrasuctureService(this IServiceCollection services, IConfiguration _config)
        {
            //add jwt authentication scheme and database
            ServiceContainer.AddServices<AuthenticationDbContext>(services, _config, _config["MySerilog:FileName"]!);
            //create dependency injection
            services.AddScoped<IUser, UserRepository>();

            return services;
        }
        //middelware

        public static IApplicationBuilder UserInfrastucturePolicy(this IApplicationBuilder app)
        {
            //Listen to only API gateway request and block unthorize request
            ServiceContainer.UsePolicies(app);
            return app;
        }
    }
}