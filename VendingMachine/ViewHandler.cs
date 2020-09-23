using Command.CommandProcessor;
using Inventory.Enums;
using InventorySupplier;
using System;
using System.Linq;
using VendingMachineSpace;

namespace VendingMachineConsole
{
    public class ViewHandler
    {
       public   IStockProvider isource; 
       public   ISupplier isupplier; 
       public   IVendingMachine ivMachine; 
       public   ICommandProcessor icommandProcessor; 
       public   bool IsVendingMachineReady = false;
       
        public   ViewHandler(ISupplier supplier,
            IVendingMachine vendingMachine,
            IStockProvider stockProvider, ICommandProcessor commandProcessor, ProviderType providerType)
        {
            IsVendingMachineReady = false;
            isupplier = supplier;
            isource = stockProvider;
            ivMachine = vendingMachine;
            icommandProcessor = commandProcessor;
            isource.SetStockProvider(providerType);
        }
        public   void CreateVendingmachineThread()
        {
            ivMachine.CreateVendingMachine(machineReadyCallback);
        }
        public   void machineReadyCallback(string str)
        {
            IsVendingMachineReady = true;
            // DisplayDashboard();
            Console.WriteLine();
            Console.WriteLine("Vending Machine is ready to process your orders");
            Console.Write("...VM$ ");
        }
        public   void CommandResultCallBack(string code, string description)
        {
            switch (code)
            {
                case "inv": DisplayDashboard(); break;
                case "order": OrderResponse(description); break;
                case "cls": Console.Clear(); break;
                case "error": ShowError(description); break;
                case "help": Help(description); break;

                default:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(description);
                    Console.ResetColor();
                    break;
            }
        }

        private   void ShowError(string str)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
            Console.ResetColor();
          
        }

        private   void DisplayDashboard()
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Blue;
            foreach (var item in ivMachine.GetAllQuantities())
            {
                Console.WriteLine(item.ItemID + "  " + item.ItemName + "(" + item.Count + ")" + " - " + "   :$ " + item.Price);
            }
            Console.ResetColor();
        }


        private   void OrderResponse(string response)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Status:" + response);
            Console.ResetColor();
        }
        private   void Help(string response)
        {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            var commands = icommandProcessor.help().ToList();

            foreach (var item in commands)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Status:" + response);
            Console.ResetColor();
        }
    }
}
