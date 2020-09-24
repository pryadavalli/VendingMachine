using Inventory.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VendingMachineSpace;

namespace Command.CommandProcessor
{
    public class OrderCommand : ICommand
    {
        public string ItemID { get; set; }
        public string Payment { get; set; }
        public string Quantity { get; set; }
        public string CommandError { get; set; }

        private IVendingMachine _vendingMachine;

        public OrderCommand()
        {

        }
        public OrderCommand(string commandParams, IVendingMachine vendingMachine)
        {
            var temp = commandParams.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (temp != null && temp.Count == 4)
            {
                this.Payment = temp[1];
                this.ItemID = temp[2];
                this.Quantity = temp[3];
                this._vendingMachine = vendingMachine;
            }
            else
            {
                CommandError = "Invalid command parameters";
            }


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

            string str = no_of_quarts + " Quarts - " + no_of_dimes + " Dimes - " + no_ofnickels + " Nickels - " + no_ofcents + " Cents";

            return str;
        }
        public bool ExecuteCommand(Action<string, string> callback)
        {
            if (!string.IsNullOrEmpty(CommandError))
            {
                callback("error", CommandError);
                return false;
            }
            float amountFloat = 0.0F;
            bool bAmount = float.TryParse(Payment, out amountFloat);

            if (amountFloat <= 0)
            {
                callback("error", "The payment cannot be zero or negative");
                return false;
            }
            if (!bAmount)
            {
                callback("error", "The required amount is invalid, valid NUMBER should be entered");
                return false;
            }

            if (amountFloat > Convert.ToDouble(MaxLimits.AMOUNT))
            {
                callback("error", "Entered payment is beyond the max limit " + Convert.ToDouble(MaxLimits.AMOUNT).ToString());
                return false;
            }

            int itemInt = 0;
            bool bItemInt = int.TryParse(ItemID, out itemInt);
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
            bool bQ = int.TryParse(Quantity, out requiredQuantity);

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
                callback("error", "Entered Quantity is beyond the  accepted range " + Convert.ToInt32(MaxLimits.QUANTITY).ToString());
                return false;
            }


            var items = _vendingMachine.GetProduct(ItemID);

            if (items == null)
            {
                callback("error", "Machine unable to dispatch items, Item not found, please choose correct item id");
            }

            if (Convert.ToInt32(items?.Count) >= requiredQuantity)
            {

                float priceofItem = 0.00F;
                bool bfloatprice = float.TryParse(items.Price, out priceofItem);
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
                    bool bSuccess = _vendingMachine.DispatchItem(ItemID, requiredQuantity, out responseStr);

                    if (!string.IsNullOrEmpty(responseStr))
                    {
                        callback("error", responseStr);
                    }

                    if (bSuccess)
                    {
                        callback("order", "Order is successful - " + items.ItemName + " Payment is completed  - " + requiredQuantity + " items - $ " + cost.ToString());
                        float balance = amountFloat - cost;

                        if (balance > 0.00F)
                        {
                            callback("order", "Please collect the below dispensed coins for total amount $" + balance.ToString());
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
                callback("error", "Order Failed!, cannot be processed because " + Convert.ToInt32(items.Count) + " items left in the Machine");
                return false;
            }
            return true;
        }
    }
}
