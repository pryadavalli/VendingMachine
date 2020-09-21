using InventorySpace;
using InventorySupplier;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestVendingMachine
{
    public class SupplierUnitTest
    {
        private IStockProvider isource;
        private ISupplier isupplier;
        public SupplierUnitTest()
        {
            //Arrange
            isource = new StockProvider(Inventory.Enums.ProviderType.InMemory);
          
        }
        [Fact(DisplayName = " Create Fake object and check against inventory creation")]
        // This requires some improvements.
        public void Test1()
        {
            //arrange
            List<Stock> tempStock = new List<Stock>();

            tempStock.Add(new Stock
            {
                ItemID = "1",
                ItemName = "test",
                Price = "3.4"
            });
            var stockProviderMock = new Mock<IStockProvider>();
            stockProviderMock.Setup(x => x.LoadStock()).Returns(Task.FromResult(tempStock));
            ISupplier sup = new Supplier(stockProviderMock.Object);

            //act
            var vmitems = sup.CreateInventory();
            var stocklist = sup.GetInventory();

            //assert
            Assert.True(stocklist.Count == 4);
        }

        [Fact]
        public async Task VMShouldbeLoadedwithproducts()
        {
            //arrange
            isupplier = new Supplier(isource);
            await isupplier.CreateInventory();

            //act
            var vmitems = isupplier.GetInventory();
            //assert
            Assert.True(vmitems?.Count > 0);
            
        }
    
        [Fact]
        public void Test2()
        {
            var mock = new Mock<ISupplier>();
            mock.Setup(x => x.CreateInventory()).Returns(Task.FromResult(It.IsAny<int>()));
            var temp = mock.Object.CreateInventory();
            Assert.NotNull(temp);
        }
    }
}
