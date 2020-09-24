using Inventory.Enums;
using InventorySpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VendingMachineSpace;

namespace Command.CommandProcessor
{
    class InvCommand : ICommand
    {
        public bool ExecuteCommand(Action<string, string> func)
        {
            func("inv", "Show the inventory");
            return true;
        }
    }

    class HelpCommand : ICommand
    {
        public bool ExecuteCommand(Action<string, string> func)
        {
            func("help", "Get Help");
            return true;
        }
    }
}
