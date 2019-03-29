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
	public class PaymentTypeServices
	{
		private readonly PaymentTypeRepository  _paymentTypeRepository;
        public PaymentTypeServices()
		{
            _paymentTypeRepository = new PaymentTypeRepository();
		}

        public long AddPaymentType(PaymentTypeObject paymentTypeAccount)
		{
			try
			{
                return _paymentTypeRepository.AddPaymentType(paymentTypeAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        
        public int UpdatePaymentType(PaymentTypeObject paymentType)
		{
			try
			{
                return _paymentTypeRepository.UpdatePaymentType(paymentType);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeletePaymentType(long paymentTypeAccountId)
		{
			try
			{
                return _paymentTypeRepository.DeletePaymentType(paymentTypeAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public PaymentTypeObject GetPaymentType(long paymentTypeAccountId)
		{
			try
			{
                return _paymentTypeRepository.GetPaymentType(paymentTypeAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PaymentTypeObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _paymentTypeRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
       
        public List<PaymentTypeObject> GetPaymentTypes()
		{
			try
			{
                var objList = _paymentTypeRepository.GetPaymentTypes();
                if (objList == null || !objList.Any())
			    {
                    return new List<PaymentTypeObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentTypeObject>();
			}
		}

        public List<PaymentTypeObject> GetPaymentTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _paymentTypeRepository.GetPaymentTypeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<PaymentTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentTypeObject>();
            }
        }

        public List<PaymentTypeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _paymentTypeRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<PaymentTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentTypeObject>();
            }
        }
	}

}
