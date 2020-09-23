using InventorySpace;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventorySupplier
{
    public interface ISupplier
    {
        Task<int> CreateInventory();
        List<Stock> GetInventory();
    }
   public  class Supplier : ISupplier
    {
        static  List<Stock> vendingMachineItems = new List<Stock>();
        private   int usingResource = 0;
        IStockProvider _provider;
        public Supplier(IStockProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Count the total values while loading to Vending machine
        /// feed the count to the system
        /// </summary>
        /// <returns></returns>
        //private List<Stock> CountStock()
        //{
        //    List<Stock> vendingMachine = new List<Stock>();
        //    var items = inventories?.GroupBy(x => x.ItemID)
        //           .Select(x => new
        //           {
        //               count = x.Count(),
        //               Key = x.Key
        //           });

        //    foreach (var item in inventories?.Distinct())
        //    {
        //        var test = items.Where(x => x.Key == item.ItemID).ToList().FirstOrDefault();
        //        item.Count = test.count.ToString();
        //        vendingMachine.Add(item);
        //    }

        //    return vendingMachine;
        //}

        public async Task<int> CreateInventory()
        {
            vendingMachineItems.Clear();
            if (0 == Interlocked.Exchange(ref usingResource, 1))
            {
                vendingMachineItems = await _provider.LoadStock();
                Interlocked.Exchange(ref usingResource, 0);
                return vendingMachineItems.Count();
            }
            else
            {
                return 0;
            }
        }
        public List<Stock> GetInventory()
        {
            return vendingMachineItems;
        }
    }
     
}
