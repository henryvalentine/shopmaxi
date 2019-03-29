using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class SaleServices
    {
        private readonly SaleRepository _saleRepository;
        public SaleServices()
        {
            _saleRepository = new SaleRepository();
        }

        public long AddSale(SaleObject sale, out string invoiceRef)
        {
            return _saleRepository.AddSale(sale, out invoiceRef);
        }

        public long RefundSale(RefundNoteObject refundNote, out string refundNoteNumber)
        {
            return _saleRepository.RefundSale(refundNote, out refundNoteNumber);
        }

        public List<StoreCountryObject> GetCountries()
        {
            return _saleRepository.GetCountries();
        }
        public List<StoreStateObject> GetStates(long countryId)
        {
            return _saleRepository.GetStates(countryId);
        }

        public int UpdateSale(SaleObject sale)
        {
            try
            {
                return _saleRepository.UpdateSale(sale);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSale(long saleAccountId)
        {
            return _saleRepository.DeleteSale(saleAccountId);
        }

        public List<CustomerObject> GetCustomers()
        {
            return _saleRepository.GetCustomers();
        }

        public List<CustomerObject> GetCustomers(int pageNumber, int itemsPerPage)
        {
            return _saleRepository.GetCustomers();
        }

        public StateReportDefault GetCustomersAndSuppliers()
        {
            return _saleRepository.GetCustomersAndSuppliers();
        }

        public SaleObject GetSale(long saleAccountId)
        {
            return _saleRepository.GetSale(saleAccountId);
        }

        public SaleObject GetInvoice(long saleId)
        {
            return _saleRepository.GetInvoice(saleId);
        }

        public SaleObject GetInvoiceForRevoke(string invoiceNumber)
        {
            return _saleRepository.GetInvoiceForRevoke(invoiceNumber);
        }

        public List<RefundNoteObject> GetRefundedSaleNotes(long saleId)
        {
            return _saleRepository.GetRefundedSaleNotes(saleId);
        }

        public long UpdateSalePayment(SaleObject sale, UserProfileObject userProfile)
        {
            return _saleRepository.UpdateSalePayment(sale, userProfile);
        }

        public int GetObjectCount()
        {
            try
            {
                return _saleRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<SaleObject> GetSales()
        {
            try
            {
                var objList = _saleRepository.GetSales();
                if (objList == null || !objList.Any())
                {
                    return new List<SaleObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSaleObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _saleRepository.GetSaleObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SaleObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> GetSalesByOutlet(long outletId, int? itemsPerPage, int? pageNumber)
        {
            try
            {
                return _saleRepository.GetSalesByOutlet(outletId, itemsPerPage, pageNumber);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }

        public List<SaleObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _saleRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<SaleObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SaleObject>();
            }
        }
    }

}
