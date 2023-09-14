using Application;
using Application.Interfaces.Configuration;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();

            services.AddSingleton<ILibraryManagementConfiguration, LibraryManagementConfiguration>();

            return services;
        }
    }
}
