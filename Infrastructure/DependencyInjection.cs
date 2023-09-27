using Application;
using Application.Interfaces;
using Application.Interfaces.Configuration;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IPatronRepository, PatronRepository>();
            services.AddScoped<ICheckoutRepository, CheckoutRepository>();

            services.AddSingleton<ILibraryManagementConfiguration, LibraryManagementConfiguration>();

            return services;
        }
    }
}
