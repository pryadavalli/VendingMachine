using InventorySpace;
using InventorySupplier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace VendingMachineSpace
{
    public interface IVendingMachine
    {
        void CreateVendingMachine(Action<string> callbackforSystemReady);
        List<Stock> GetAllQuantities();
        Stock GetProduct(string item);
       bool DispatchItem(string item, int quantity);
    }
    public class VendingMachine:IVendingMachine
    {
        static List<Stock> stockList = new List<Stock>();
        ISupplier isupplier;
        private   object lockobject = new object();
        public VendingMachine(ISupplier _supplier)
        {
            isupplier = _supplier;// new Supplier(isource);
        }
        public   void CreateVendingMachine(Action<string> callbackforSystemReady)
        {
            Task<int> inventoryCreationTask = isupplier.CreateInventory();
            inventoryCreationTask.Wait();
            stockList = isupplier.GetInventory();
            callbackforSystemReady("ready");
        }
        public   List<Stock> GetAllQuantities()
        {
            lock (lockobject)
            {
                stockList = isupplier.GetInventory();
                return stockList;
            }
        }

        public   Stock GetProduct(string item)
        {
            lock (lockobject)
            {
                return stockList.Where(x => x.ItemID == item).FirstOrDefault();
            }
        }

        public   bool DispatchItem(string item,int quantity)
        {
           
            lock (lockobject)
            {
                var test = stockList.Where(x => x.ItemID == item).ToList().FirstOrDefault();
                test.Count = (Convert.ToInt32(test.Count) - quantity).ToString();

                if (test != null) return true;
                else return false;
             }
        }
    }
}
