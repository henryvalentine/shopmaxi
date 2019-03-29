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
	public class StorePaymentMethodServices
	{
		private readonly StorePaymentMethodRepository  _paymentMethodRepository;
        public StorePaymentMethodServices()
		{
            _paymentMethodRepository = new StorePaymentMethodRepository();
		}
        public long AddStorePaymentMethod(StorePaymentMethodObject paymentMethod)
		{
			try
			{
                return _paymentMethodRepository.AddStorePaymentMethod(paymentMethod);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        public int GetObjectCount()
        {
            try
            {
                return _paymentMethodRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateStorePaymentMethod(StorePaymentMethodObject paymentMethod)
		{
			try
			{
                return _paymentMethodRepository.UpdateStorePaymentMethod(paymentMethod);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteStorePaymentMethod(long paymentMethodId)
		{
			try
			{
                return _paymentMethodRepository.DeleteStorePaymentMethod(paymentMethodId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public StorePaymentMethodObject GetStorePaymentMethod(long paymentMethodId)
		{
			try
			{
                return _paymentMethodRepository.GetStorePaymentMethod(paymentMethodId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StorePaymentMethodObject();
			}
		}
        public List<StorePaymentMethodObject> GetStorePaymentMethods()
		{
			try
			{
                var objList = _paymentMethodRepository.GetStorePaymentMethods();
                if (objList == null || !objList.Any())
			    {
                    return new List<StorePaymentMethodObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentMethodObject>();
			}
		}
        public List<StorePaymentMethodObject> GetStorePaymentMethodObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _paymentMethodRepository.GetStorePaymentMethodObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StorePaymentMethodObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentMethodObject>();
            }
        }
        
	  }

}
