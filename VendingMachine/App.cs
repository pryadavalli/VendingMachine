using Command.CommandProcessor;
using Inventory.Enums;
using InventorySupplier;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using VendingMachineConsole;
using VendingMachineSpace;

namespace VendingMachineProgram
{
    public class App
    {
        private readonly IConfiguration _config;
        ViewHandler _viewHandler;
        public App(IConfiguration config, ISupplier supplier,
            IVendingMachine vendingMachine,  
            IStockProvider stockProvider, ICommandProcessor commandProcessor)
            
        {
            _config = config;
             _viewHandler = new ViewHandler(supplier,vendingMachine, stockProvider,commandProcessor);
        }

        public void Run()
        {
            var logDirectory = _config.GetValue<string>("Runtime:LogOutputDirectory");

            var log = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(logDirectory)
                        .CreateLogger();
          
         //   log.Information("serilog logger information");

            CommandPrompt(log);
        }

        public void CommandPrompt(Logger logger)
        {
            try
            {
                Console.WriteLine(Convert.ToDouble(MaxLimits.AMOUNT));
                
                Thread tstart = new Thread(_viewHandler.CreateVendingmachineThread);
                tstart.Start();
                Console.WriteLine("Welcome to Food Court");
                Console.WriteLine("PLACE THE ORDER \n");
               // Console.WriteLine("Input text should be: order <amount> <item> <quantity> \n");
                //
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
                logger.Error(ex.StackTrace.ToString());
                Console.WriteLine("Application got into error, please contact Admin " + ex.Message);
            }
        }
    }
}
