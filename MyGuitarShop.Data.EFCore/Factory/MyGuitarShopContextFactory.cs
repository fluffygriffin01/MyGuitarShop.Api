using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyGuitarShop.Data.EFCore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Data.EFCore.Factory
{
    internal class MyGuitarShopContextFactory : IDesignTimeDbContextFactory<MyGuitarShopContext>
    {
        public MyGuitarShopContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("MyGuitarShopMigrations");

            var optionsBuilder = new DbContextOptionsBuilder<MyGuitarShopContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new MyGuitarShopContext(optionsBuilder.Options);
        }
    }
}
