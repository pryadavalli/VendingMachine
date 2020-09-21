using Inventory.Enums;
using InventorySpace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                if (File.Exists("inventory.json"))
                {
                    using (StreamReader r = new StreamReader("inventory.json"))
                    {
                        string json = r.ReadToEnd();
                        inventories = JsonConvert.DeserializeObject<List<Stock>>(json);
                    }
                }
            });
            await loadTask;
            return inventories;
        }
        public async Task<List<Stock>> CreateSampleDataMemory()
        {
            List<Stock> inventories = new List<Stock>();
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
                    inventories.Add(coke);
                }
                for (int i = 0; i < 15; i++)
                {
                    inventories.Add(MnM);
                }
                for (int i = 0; i < 5; i++)
                {
                    inventories.Add(Water);
                }
                for (int i = 0; i < 7; i++)
                {
                    inventories.Add(Snickers);
                }
            });
            await loadTask;
            return inventories;
        }
    }

    
}
