

using Microsoft.Data.SqlClient;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;
using MyGuitarShop.Data.Ado.Repository;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MyGuitarShop.Api
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                AddLogging(builder);
                AddServices(builder);

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                if (builder.Environment.IsDevelopment())
                {
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                }

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                ConfigureApplication(app);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                Console.WriteLine(ex.Message);
            }
        }

        private static void AddLogging(WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddFilter(category: "Microsoft", LogLevel.Information)
                    .AddFilter(category: "MicroSoft.AspNetCore.HttpLogging", LogLevel.Information)
                    .AddConsole();
            });

            builder.Services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode;
            });
        }

        private static void ConfigureApplication(WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString(name: "MyGuitarShop")
                ?? throw new InvalidOperationException("MyGuitarShop connection string not found.");

            builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));

            builder.Services.AddScoped<IRepository<ProductDto>, ProductRepository>();

            builder.Services.AddControllers();
        }
    }
}
