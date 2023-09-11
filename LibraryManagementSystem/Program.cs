using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Presentation;
using Serilog;
using Newtonsoft.Json;

namespace LibraryManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services
                .AddApplication()
                .AddInfrastructure()
                .AddPresentation();

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(Program).Assembly.FullName)
                )) ;
            builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

            builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}