using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
    public class SalePaymentServices
    {
        private readonly SalePaymentRepository _salePaymentRepository;
        public SalePaymentServices()
        {
            _salePaymentRepository = new SalePaymentRepository();
        }

        public long AddSalePayment(SalePaymentObject salePayment)
        {
            return _salePaymentRepository.AddSalePayment(salePayment);
        }

        public long RefundSalePayment(SalePaymentObject salePayment, bool saleRevoked)
        {
            return _salePaymentRepository.RefundSalePayment(salePayment, saleRevoked);
        }

        public long ProcessCustomerInvoice(CustomerInvoiceObject customerInvoice)
        {
            return _salePaymentRepository.ProcessCustomerInvoice(customerInvoice);
        }

        public long BalanceCustomerInvoice(CustomerInvoiceObject customerInvoice)
        {
            return _salePaymentRepository.BalanceCustomerInvoice(customerInvoice);
        }

        public int GetObjectCount()
        {
            try
            {
                return _salePaymentRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSalePayment(SalePaymentObject city)
        {
            try
            {
                return _salePaymentRepository.UpdateSalePayment(city);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSalePayment(long cityAccountId)
        {
            try
            {
                return _salePaymentRepository.DeleteSalePayment(cityAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SalePaymentObject GetSalePayment(long cityAccountId)
        {
            try
            {
                return _salePaymentRepository.GetSalePayment(cityAccountId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SalePaymentObject();
            }
        }

        public List<SalePaymentObject> GetCities()
        {
            try
            {
                var objList = _salePaymentRepository.GetCities();
                if (objList == null || !objList.Any())
                {
                    return new List<SalePaymentObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalePaymentObject>();
            }
        }

        public List<SalePaymentObject> GetSalePaymentObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _salePaymentRepository.GetSalePaymentObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<SalePaymentObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SalePaymentObject>();
            }
        }

        //public List<SalePaymentObject> Search(string searchCriteria)
        //{
        //    try
        //    {
        //        var objList = _cityRepository.Search(searchCriteria);
        //        if (objList == null || !objList.Any())
        //        {
        //            return new List<SalePaymentObject>();
        //        }
        //        return objList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<SalePaymentObject>();
        //    }
        //}
    }

}
