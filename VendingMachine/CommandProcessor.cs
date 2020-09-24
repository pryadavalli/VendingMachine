using Inventory.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VendingMachineSpace;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;

namespace Command.CommandProcessor
{
    public interface ICommandProcessor
    {
            bool ExecuteCommand(string commandParams, Action<string,string> func);
          bool AddCommand(string newCommand, string description);
          Dictionary<string,string> help();
        string BillDispenser(float balance);

    }
    public class CommandProcessor : ICommandProcessor
    {
        public Dictionary<string,string> commandList = new Dictionary<string,string>();
        public IVendingMachine _vendingMachine;
        public CommandProcessor(IVendingMachine vendingMachine)
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
        /// This function can go for Command design pattern
        /// </summary>
        /// <param name="commandParams"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool ExecuteCommand(string commandParams, Action<string,string> callback)
        {
            var temp = commandParams.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (temp?.Count > 0 && commandList.ContainsKey(temp[0]))
            {
                if (temp[0].Equals("inv"))
                {
                    callback("inv","Lists the inventory");
                }
                else
                if (temp[0].Equals("cls"))
                {
                    callback("cls","clearing the screen");
                }
                else
                if (temp.Count == 4 && temp[0].Equals("order"))
                {
                    string amount = temp[1];
                    string itemid = temp[2];
                    float amountFloat = 0.0F;
                    bool bAmount = float.TryParse(temp[1], out amountFloat);
                 
                    if(amountFloat <=0)
                    {
                        callback("error", "The payment cannot be zero or negative");
                        return false;
                    }
                    if (!bAmount)
                    {
                        callback("error", "The required amount is invalid, valid NUMBER should be entered");
                        return false;
                    }
                     
                    if(amountFloat > Convert.ToDouble(MaxLimits.AMOUNT))
                    {
                        callback("error", "Entered payment is beyond the max limit " + Convert.ToDouble(MaxLimits.AMOUNT).ToString());
                        return false;
                    }

                    int itemInt = 0;
                    bool bItemInt = int.TryParse(temp[2], out itemInt);
                    if (!bItemInt)
                    {
                        callback("error", "The required ItemId is invalid, valid NUMBER should be entered");
                        return false;
                    }
                    if (itemInt <= 0)
                    {
                        callback("error", "The item number cannot be zero or negative");
                        return false;
                    }
                    if (itemInt > Convert.ToInt32(MaxLimits.ITEMID))
                    {
                        callback("error", "Entered ItemID is beyond the  accepted range " + Convert.ToInt32(MaxLimits.ITEMID).ToString());
                        return false;
                    }

                    int requiredQuantity = 0;
                    bool bQ = int.TryParse(temp[3], out requiredQuantity);

                    if (requiredQuantity <= 0)
                    {
                        callback("error", "The item number cannot be zero or negative");
                        return false;
                    }
                    if (!bQ)
                    {
                        callback("error", "The required quantity is invalid. valid NUMBER should be entered");
                        return false;
                    }

                    if (requiredQuantity > Convert.ToInt32(MaxLimits.QUANTITY))
                    {
                        callback("error", "Entered Quantity is beyond the  accepted range "+ Convert.ToInt32(MaxLimits.QUANTITY).ToString());
                        return false;
                    }


                    var items =    _vendingMachine.GetProduct(itemid);
                    
                    if(items == null)
                    {
                        callback("error", "Machine unable to dispatch items, Item not found, please choose correct item id");
                    }

                    if (Convert.ToInt32(items?.Count) >= requiredQuantity)
                    {
                       
                        float priceofItem = 0.00F;
                        bool bfloatprice = float.TryParse(items.Price,out priceofItem);
                        if (!bfloatprice)
                        {
                            callback("error", "The required quantity is invalid. valid NUMBER should be entered");
                            return false;
                        }

                        var cost = priceofItem * requiredQuantity;
                        if (cost > amountFloat)
                        {
                            callback("error", "Order cannot be processed,  payment is insufficient - " + items.ItemName + " Total payment required for -" + requiredQuantity + " items is $ " + cost.ToString());
                        }
                        else
                        {
                           string responseStr;
                           bool bSuccess =  _vendingMachine.DispatchItem(itemid, requiredQuantity, out responseStr);

                            if(!string.IsNullOrEmpty(responseStr))
                            {
                                callback("error", responseStr);
                            }

                            if (bSuccess)
                            {
                                callback("order", "Order is successful - " + items.ItemName + " Payment is completed  - " + requiredQuantity + " items - $ " + cost.ToString());
                                float balance = amountFloat - cost;

                                if (balance > 0.00F)
                                {
                                    callback("order", "Please collect the below dispensed cash $" + balance.ToString());
                                    string dispMoney = BillDispenser(balance);
                                    callback("order", dispMoney);
                                }
                            }
                            else
                            {
                                callback("error", "cannot dispatch the item as the item is not found");
                            }
                        }
                    }
                    else
                    {
                        callback( "error", "Order Failed!, cannot be processed because " + Convert.ToInt32(items.Count) + " items left in the Machine");
                        return false;
                    }
                }
                else
                if (temp[0].Equals("?"))
                {
                    callback("help", "Get Help");
                }
                else
                {
                    callback("error", commandParams + " => Please provide required parameters, for help type ? + <enter>");
                }
            }
            else
            {
                callback("error", commandParams + " =>Invalid command, please see the help by typing ? and press enter");
            }
            return true;
        }

        public string BillDispenser(float balance)
        {
            var tempFraction = Math.Round((float)(balance * 100));
            int result = (int)(tempFraction % 100);
            int no_of_quarts = (result - (int)result % 25) / 25;
           // Console.WriteLine("No of quarts =" + no_of_quarts);
            int no_of_dimes = ((int)result - 25 * no_of_quarts) / 10;
          // Console.WriteLine("No of dimes =" + no_of_dimes);

            int no_ofnickels = ((int)result - 25 * no_of_quarts - 10 * no_of_dimes - (int)(result % 5)) / 5;
           // Console.WriteLine("No of nickels =" + no_ofnickels);

            int no_ofcents = ((int)result - 25 * no_of_quarts - 10 * no_of_dimes - 5 * no_ofnickels);
           // Console.WriteLine("No of cents =" + no_ofcents);

            string str = no_of_quarts + " Quarts - " + no_of_dimes + " Dimes -" + no_ofnickels + " Nickels - " + no_ofcents + " Cents";

            return str;
        }

      
    }
}
