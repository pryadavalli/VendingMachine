using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace InventorySpace
{
   public class Stock
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string Price { get; set; }
        public string Count { get; set; }
    }
}
