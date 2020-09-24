using InventorySpace;
using InventorySupplier;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Command.CommandProcessor;
using System.Linq;
using VendingMachineSpace;
using System.Threading;
using VendingMachineProgram;
using System.Diagnostics;

namespace XUnitTestVendingMachine
{
    public class CommandProcessUnitTest
    {
        public Dictionary<string, string> commandList = new Dictionary<string, string>();
        ICommandProcessor icommandProcessor;
        static IStockProvider isource = new StockProvider(Inventory.Enums.ProviderType.InMemory);
        static ISupplier isupplier = new Supplier(isource);
        IVendingMachine vendingMachine = new VendingMachine(isupplier);

        public CommandProcessUnitTest()
        {
            icommandProcessor = new CommandProcessor(vendingMachine);
            commandList.Add("inv", "inv");
            commandList.Add("cls", "clear");
            commandList.Add("?", "help");
            commandList.Add("order", "order");

            vendingMachine.CreateVendingMachine(machineReadyCallback);
        }

        private void machineReadyCallback(string str)
        {


        }
        [Fact(DisplayName = "Check all commands exist")]
        public void Test1()
        {
            var commandcount = icommandProcessor.help().Count;

            Assert.True(commandcount == 4);
        }

        [Fact]
        public void Test2()
        {
            var actualcommands = icommandProcessor.help();
            bool bexists = actualcommands.All(x => commandList.ContainsKey(x.Key));
            Assert.True(bexists);
        }
        [Fact]
        public void Test3()
        {
            string cmdparam = "inv";
            bool bValue = icommandProcessor.ExecuteCommand(cmdparam, CommandResultCallBack);


            Assert.True(bValue);
        }
        private void CommandResultCallBack(string code, string description)
        {
            string cmd = "inv";
            string desc = "Lists the inventory";

            Assert.True(commandList.ContainsKey(code));
            Assert.True(cmd.Equals(code) && desc.Equals(description));
        }


        [Fact]
        public void Test4()
        {
            string cmdparam = "order 12 1 3";

            var mockCmd = new Mock<IVendingMachine>();
            Stock sample = new Stock()
            {
                ItemID = "1",
                ItemName = "Coke",
                Price = "1.25",
                Count = "10"
            };

            mockCmd.Setup(x => x.CreateVendingMachine(machineReadyCallback));
            mockCmd.Setup(x => x.GetProduct("1")).Returns(sample);

            ICommandProcessor icommandProcessorLocal = new CommandProcessor(mockCmd.Object);
            bool bValue = icommandProcessorLocal.ExecuteCommand(cmdparam, CommandResultCallBackDummy);
            Assert.True(bValue);
        }
        private void CommandResultCallBackDummy(string code, string description)
        {


        }

        private void CommandResultCallBackParallelPRocess(string code, string description)
        {
            LoggerEx.LogInformation(description);
            Assert.Equal("order", code);
        }
        [Fact]
        public void ParallelProcess()
        {
            string cmdInput = "order 17 1 5";

            Parallel.Invoke(() => { icommandProcessor.ExecuteCommand(cmdInput, CommandResultCallBackParallelPRocess); },
                () => { icommandProcessor.ExecuteCommand(cmdInput, CommandResultCallBackParallelPRocess); }
                );
        }

        [Fact]
        public void CurrencyCal()
        {
            float balance = 1.06F;
            var tempFraction = Math.Round((float)(balance * 100));
            int result = (int)(tempFraction % 100);
            Debug.WriteLine(result);
            int no_of_quarts = (result - (int)result % 25) / 25;
            Debug.WriteLine("No of quarts =" + no_of_quarts);

            int no_of_dimes = ((int)result - 25 * no_of_quarts) / 10;
            Debug.WriteLine("No of dimes =" + no_of_dimes);

            int no_ofnickels = ((int)result - 25 * no_of_quarts - 10 * no_of_dimes - (int)(result % 5)) / 5;
            Debug.WriteLine("No of nickels =" + no_ofnickels);

            int no_ofcents = ((int)result - 25 * no_of_quarts - 10 * no_of_dimes - 5 * no_ofnickels);
            Debug.WriteLine("No of cents =" + no_ofcents);
        }

        [Fact]
        public void BillDispenserTest()
        {
            int quart = 1;
            int dime = 1;
            int nickel = 1;
            int cent = 2;
            float finput =(float)(quart * 25 + dime * 10 + nickel * 5 + cent * 1)/100;
            var result =  icommandProcessor.BillDispenser(finput);
            string expected = quart + " Quarts - " + dime + " Dimes - " + nickel + " Nickels - " + cent + " Cents";
            Assert.Equal(expected, result);
        }
    }
}
