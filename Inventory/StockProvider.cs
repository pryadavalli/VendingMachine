using Inventory.Enums;
using InventorySpace;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySupplier
{
    public interface IStockProvider
    {
        Task<List<Stock>> LoadStock();
        Task<List<Stock>> CreateSampleDataJson();
        Task<List<Stock>> CreateSampleDataMemory();
        void SetStockProvider(ProviderType decideprovider);
    }
    public class StockProvider : IStockProvider
    {
        private static ProviderType providerType;
        List<Stock> inventories = new List<Stock>();

        public StockProvider()
        {

        }
        public StockProvider(ProviderType decideprovider)
        {
            providerType = decideprovider;
        }
        public void SetStockProvider(ProviderType decideprovider)
        {
            providerType = decideprovider;
        }

        public async Task<List<Stock>> LoadStock()
        {
            if (providerType == ProviderType.JSON)
            {
                inventories = await CreateSampleDataJson();
            }

            if (providerType == ProviderType.InMemory)
            {
                inventories = await CreateSampleDataMemory();
            }

            return inventories;
        }

        public async Task<List<Stock>> CreateSampleDataJson()
        {
            Task loadTask = Task.Run(() =>
            {
                string str = Directory.GetCurrentDirectory();
                bool b = File.Exists("inventory.json");

                if (File.Exists("inventory.json"))
                {
                    using (StreamReader r = new StreamReader("inventory.json"))
                    {
                        string json = r.ReadToEnd();
                       var localInventories = JsonConvert.DeserializeObject<List<Stock>>(json);

                        CreateConsolidatedList(localInventories);
                    }
                }
            });
            await loadTask;
            return inventories;
        }

        private void CreateConsolidatedList(List<Stock> localInventories)
        {
            var items = localInventories?.GroupBy(x => x.ItemID)
                            .Select(x => new
                            {
                                count = x.Count(),
                                Key = x.Key
                            });

            foreach (var item in items)
            {
                var temp = localInventories.Where(x => x.ItemID == item.Key).Select(x => x).FirstOrDefault();
                temp.Count = item.count.ToString();
                inventories.Add(temp);
            }

           // return inventories;
        }
        public async Task<List<Stock>> CreateSampleDataMemory()
        {
            List<Stock> localInventories = new List<Stock>();
            Task loadTask = Task.Run(() =>
            {
                Stock coke = new Stock()
                {
                    ItemID = "1",
                    ItemName = "Coke",
                    Price = "1.25"
                };
                Stock MnM = new Stock()
                {
                    ItemID = "2",
                    ItemName = "M&M's",
                    Price = "1.89"
                };
                Stock Water = new Stock()
                {
                    ItemID = "3",
                    ItemName = "Water",
                    Price = ".89"
                };
                Stock Snickers = new Stock()
                {
                    ItemID = "4",
                    ItemName = "Snickers",
                    Price = "2.05"
                };
                //create coke box

                for (int i = 0; i < 10; i++)
                {
                    localInventories.Add(coke);
                }
                for (int i = 0; i < 15; i++)
                {
                    localInventories.Add(MnM);
                }
                for (int i = 0; i < 5; i++)
                {
                    localInventories.Add(Water);
                }
                for (int i = 0; i < 7; i++)
                {
                    localInventories.Add(Snickers);
                }
            });
            await loadTask;
            CreateConsolidatedList(localInventories);
            return inventories;
        }
    }

    
}
