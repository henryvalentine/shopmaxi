
using System.Collections.Generic;
using System.Web;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StockUploadObject
    {
        public string StoreItemName { get; set; }
        public HttpPostedFileBase FileObj { get; set; }
        public int FileId { get; set; }

        public int TempId { get; set; }
        public string ViewName { get; set; }
    }
}

