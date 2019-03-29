using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class PurchaseOrderServices
    {
        private readonly PurchaseOrderRepository _purchaseOrderRepository;
        public PurchaseOrderServices()
        {
            _purchaseOrderRepository = new PurchaseOrderRepository();
        }

        public long AddPurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            return _purchaseOrderRepository.AddPurchaseOrder(purchaseOrder);
        }

        public long AddPurchaseOrder(PurchaseOrderObject purchaseOrder, out string purchaseOrderNumber)
        {
            return _purchaseOrderRepository.AddPurchaseOrder(purchaseOrder, out purchaseOrderNumber);
        }

        public long DeleteOrderItem(long orderItemId)
        {
            return _purchaseOrderRepository.DeleteOrderItem(orderItemId);
        }

        public long DeletePurchaseOrderItemDelivery(long purchaseOrderItemDeliveryId)
        {
            return _purchaseOrderRepository.DeletePurchaseOrderItemDelivery(purchaseOrderItemDeliveryId);
        }

        public long UpdatePurchaseOrder(PurchaseOrderObject purchaseOrder)
        {
            return _purchaseOrderRepository.UpdatePurchaseOrder(purchaseOrder);
        }
        public PurchaseOrderSelectable GetSelectables()
        {
            return _purchaseOrderRepository.GetSelectables();
        }
        public PurchaseOrderObject GetPurchaseOrder(long purchaseOrderId)
        {
            return _purchaseOrderRepository.GetPurchaseOrder(purchaseOrderId);
        }

        public long ProcessPurchaseOrderInvoice(InvoiceJson invoice)
        {
            return _purchaseOrderRepository.ProcessPurchaseOrderInvoice(invoice);
        }
        public long ProcessPurchaseOrderDeliveries(POrderInfo delivery)
        {
            return _purchaseOrderRepository.ProcessPurchaseOrderDeliveries(delivery);
        }

        //public long EditPurchaseOrderDelivery(PurchaseOrderObject deliveryObject)
        //{
        //    return _purchaseOrderRepository.EditPurchaseOrderDelivery(deliveryObject);
        //}
        //public long AddPurchaseOrderDelivery(PurchaseOrderObject deliveryObject)
        //{
        //    return _purchaseOrderRepository.AddPurchaseOrderDelivery(deliveryObject);
        //}

        public PurchaseOrderObject GetPurchaseOrderDetails(long purchaseOrderId)
        {
            return _purchaseOrderRepository.GetPurchaseOrderDetails(purchaseOrderId);
        }

        public List<PurchaseOrderObject> GetPurchaseOrderObjects(int? itemsPerPage, int? pageNumber, out int count)
        {
            return _purchaseOrderRepository.GetPurchaseOrders(itemsPerPage, pageNumber, out count);
        }
        public List<PurchaseOrderObject> GetPurchaseOrdersByOutlet(int? itemsPerPage, int? pageNumber, out int count, int outletId)
        {
            return _purchaseOrderRepository.GetPurchaseOrdersByOutlet(itemsPerPage, pageNumber, out count, outletId);
        }

        public List<PurchaseOrderObject> GetPurchaseOrdersByEmployee(int? itemsPerPage, int? pageNumber, out int count, long employeeId)
        {
            return _purchaseOrderRepository.GetPurchaseOrdersByEmployee(itemsPerPage, pageNumber, out count, employeeId);
        }

        public List<PurchaseOrderObject> SearchPurchaseOrders(string searchCriteria)
        {
            return _purchaseOrderRepository.SearchPurchaseOrders(searchCriteria);
        }

        public List<PurchaseOrderObject> SearchOutletPurchaseOrder(string searchCriteria, int outletId)
        {
            return _purchaseOrderRepository.SearchOutletPurchaseOrder(searchCriteria, outletId);
        }

        public List<PurchaseOrderObject> SearchEmployeePurchaseOrder(string searchCriteria, long employeeId)
        {
            return _purchaseOrderRepository.SearchEmployeePurchaseOrder(searchCriteria, employeeId);
        }

    }

}

