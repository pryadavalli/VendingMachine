using Command.CommandProcessor;
using InventorySupplier;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using VendingMachineSpace;

namespace VendingMachineProgram
{
    class Program
    {
        static  void Main(string[] args)
        {
            var services = ConfigurationServices();
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<App>().Run();

            System.Console.WriteLine("Application is shutting down....");
        }

        private static IServiceCollection ConfigurationServices()
        {
            IServiceCollection services = new ServiceCollection();
            var config = LoadConfiguration();

            services.AddSingleton(config);
            services.AddTransient<App>();
            services.AddLogging(config => config.AddSerilog());
            services.AddTransient<IStockProvider, StockProvider>();
            services.AddTransient<ISupplier, Supplier>();
            services.AddTransient<IVendingMachine, VendingMachine>();
            services.AddTransient<ICommandProcessor, CommandProcessor>();
            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                            
            return builder.Build();
        }
        
    }
}
