﻿using SSMO.Data.Models;
using SSMO.Models.Documents.Packing_List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Documents
{
    public interface IDocumentService
    {
       // public int GetLastNumInvoice();
        public string GetLastNumOrder();
       
        public ICollection<int> GetInvoices();

        public ICollection<int> GetPackingList();

        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber);

    }
}
