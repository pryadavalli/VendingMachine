using InventorySpace;
using InventorySupplier;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestVendingMachine
{
    public class StockProviderUnitTest
    {
        private IStockProvider isource;
        public   StockProviderUnitTest()
        {
            //Arrange
        }
        [Fact(DisplayName = "CheckForDIFortheProvider")]
        public  void Test1()
        {
            //assert
            Assert.True(isource == null);
        }

        [Fact(DisplayName = "MakeSureStockProviderGetsData")]
        public async Task Test2()
        {
            //arrange
            isource = new StockProvider(Inventory.Enums.ProviderType.InMemory);

            //act
            List<Stock> inventry = await isource.LoadStock();

            //assert
            Assert.True(inventry?.Count > 1);
        }
        [Fact(DisplayName = "WhenStockProviderCouldnotProvideDataFromDatabase")]
        public async Task Test3()
        {
            //arrange
            isource = new StockProvider(Inventory.Enums.ProviderType.Database);
            //act
            List<Stock> inventry = await isource.LoadStock();
            //assert
            Assert.True(inventry?.Count == 0);
        }
        [Fact(DisplayName = "This checks for In Memory Data stock load and real data stock load")]
        public async Task Test4()
        {
            //arrange
            isource = new StockProvider(Inventory.Enums.ProviderType.InMemory);
            var mock = new Mock<IStockProvider>();
            List<Stock> tempStock = new List<Stock>();
            mock.Setup(x => x.CreateSampleDataMemory()).Returns(Task.FromResult(tempStock));

            //act
            List<Stock> inventry = await isource.LoadStock();
            List<Stock> mockinventry  = await mock.Object.LoadStock();
            //assert
            Assert.True(inventry != null && mockinventry == null);
        }
       
    }
}
