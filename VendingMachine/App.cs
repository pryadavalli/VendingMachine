using Command.CommandProcessor;
using Inventory.Enums;
using InventorySupplier;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;
using System.Threading;
using VendingMachineSpace;
using VendingMachineViewHandler;

namespace VendingMachineProgram
{
    public static class LoggerEx
    {
        static Logger loggerConfiguration;

         static LoggerEx()
        {
           var _config =  Program.LoadConfiguration();
            var logDirectory = _config.GetValue<string>("Runtime:LogOutputDirectory");

            loggerConfiguration = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(logDirectory)
                        .CreateLogger();
        }
        public static void LogInformation(string str)
        {
            loggerConfiguration.Information(str);
        }
      
    }
    public class App
    {
        private readonly IConfiguration _config;
        ViewHandler _viewHandler;
        public App(IConfiguration config, ISupplier supplier,
            IVendingMachine vendingMachine,  
            IStockProvider stockProvider, ICommandProcessor commandProcessor)
            
        {
            _config = config;
            var dataProvider = _config.GetValue<ProviderType>("Runtime:DataProvider");
            _viewHandler = new ViewHandler(supplier,vendingMachine, stockProvider,commandProcessor, dataProvider);
        }

        public void Run()
        {
            var logDirectory = _config.GetValue<string>("Runtime:LogOutputDirectory");

            var log = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(logDirectory)
                        .CreateLogger();

            log.Information("serilog logger information");

            CommandPrompt();
        }

        public void CommandPrompt()
        {
            try
            {
                Thread tstart = new Thread(_viewHandler.CreateVendingmachineThread);
                tstart.Start();
                Console.WriteLine("Welcome to Food Court");
                Console.WriteLine("PLACE THE ORDER \n");
                string cmdInput = string.Empty;
                while (cmdInput != "exit")
                {
                    Console.Write("...VM$ ");
                    cmdInput = Console.ReadLine().ToLower();
                    if (cmdInput != "exit" && cmdInput != string.Empty)
                    {
                        if (_viewHandler.IsVendingMachineReady)
                        {
                            _viewHandler.icommandProcessor.ExecuteCommand(cmdInput, _viewHandler.CommandResultCallBack);
                        }
                        else
                        {
                              Console.WriteLine("VM is getting loaded please try again....");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application got into error, please contact Admin " + ex.Message);
            }
        }
    }
}
