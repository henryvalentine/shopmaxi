
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class ImageViewObject
    {
        public int ImageViewId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<StockUploadObject> StockUploadObjects { get; set; }
    }
}
