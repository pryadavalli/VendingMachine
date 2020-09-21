using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Inventory.Enums
{
    public enum ProviderType
    {
        JSON,
        Database,
        InMemory
    }
   public enum ECommandList
    {
        [Description("Inv")]
        INV,
        [Description("Order")]
        ORDER
    }

    public enum MaxLimits
    {
        AMOUNT = 100,
        QUANTITY = 20,
        ITEMID = 15
    }
}
