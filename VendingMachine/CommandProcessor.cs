using Inventory.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VendingMachineSpace;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Command.CommandProcessor
{
    public interface ICommandFactory
    {
          bool ExecuteCommand(string commandParams, Action<string,string> func);
          bool AddCommand(string newCommand, string description);
          Dictionary<string,string> help();
    }
    public class CommandFactory : ICommandFactory
    {
        public Dictionary<string,string> commandList = new Dictionary<string,string>();
        public IVendingMachine _vendingMachine;
        public CommandFactory(IVendingMachine vendingMachine)
        {
            _vendingMachine = vendingMachine;
            // create a command list
            commandList.Add("inv","This command lists all the available inventory");
            commandList.Add("order","This command take 3 arguments as => amount, itemid, no.of quanitities");
            commandList.Add("cls","clears the screen");
            commandList.Add("?", "Get Help!");
        }
        public bool AddCommand(string newCommand,string description)
        {
            commandList.Add(newCommand, description);
            return true;
        }
        public Dictionary<string, string> help()
        {
            return commandList;
        }
        /// <summary>
        /// </summary>
        /// <param name="commandParams"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool ExecuteCommand(string commandParams, Action<string,string> callback)
        {
            ICommand command;
            var temp = commandParams.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (temp?.Count > 0 && commandList.ContainsKey(temp[0]))
            {
                if (temp[0].Equals("inv"))
                {
                    command = new InvCommand();
                    command.ExecuteCommand(callback); 
                }
                
                if (temp[0].Equals("cls"))
                {
                    callback("cls","clearing the screen");
                }
                
                if (temp[0].Equals("order"))
                {
                    command = new OrderCommand(commandParams, _vendingMachine);
                    command.ExecuteCommand(callback);
                }
                 
                if (temp[0].Equals("?"))
                {
                    command = new HelpCommand();
                    command.ExecuteCommand(callback);
                }
            }
            else
            {
                callback("error", commandParams + " =>Invalid command, please see the help by typing ? and press enter");
            }
            return true;
        }
    }
}
