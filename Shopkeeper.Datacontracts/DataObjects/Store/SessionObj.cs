
using System.Collections.Generic;
using System.Web;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public class SessionObj
    {
        public HttpPostedFileBase FileObj { get; set; }
        public int FileId { get; set; }
        public int ImageViewId { get; set; }

        public string ViewName { get; set; }
    }
}
