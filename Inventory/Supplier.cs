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
