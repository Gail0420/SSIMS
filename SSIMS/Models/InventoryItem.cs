﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIMS.Models
{
    public class InventoryItem 
    {
        public int InventoryItemID { get; set; }
        public int ItemID { get; set; }
        public int InStoreQty { get; set; }
        public int InTransitQty { get; set; }
        public int ReorderLvl { get; set; }
        public int ReorderQty { get; set; }
        public int StockCheck { get; set; }

        public Item Item { get; set; }

        public virtual ICollection<Tender> Tenders { get; set; }

        public InventoryItem()
        {
        }

        public InventoryItem( int inStoreQty, int inTransitQty, int reorderLvl, int reorderQty, int stockCheck)
        {
            InStoreQty = inStoreQty;
            InTransitQty = inTransitQty;
            ReorderLvl = reorderLvl;
            ReorderQty = reorderQty;
            StockCheck = stockCheck;
            Tenders = new List<Tender>();
        }
    }
}