using System;
using System.Collections.Generic;
using System.Text;

namespace Command.CommandProcessor
{
    public interface ICommand
    {
        bool ExecuteCommand(Action<string, string> callback);
    }


}
