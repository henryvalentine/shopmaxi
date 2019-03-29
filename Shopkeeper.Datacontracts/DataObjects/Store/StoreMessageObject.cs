

using ImportPermitPortal.DataObjects;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreMessageObject
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int MessageTemplateId { get; set; }
        public int Status { get; set; }
        public System.DateTime DateSent { get; set; }
        public string MessageBody { get; set; }

        public virtual StoreMessageTemplateObject StoreMessageTemplateObject { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
    }
}
