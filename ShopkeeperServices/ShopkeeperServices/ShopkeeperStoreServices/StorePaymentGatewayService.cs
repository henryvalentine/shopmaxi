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
	public class StorePaymentGatewayServices
	{
		private readonly StorePaymentGatewayRepository  _storePaymentGatewayRepository;
        public StorePaymentGatewayServices()
		{
            _storePaymentGatewayRepository = new StorePaymentGatewayRepository();
		}

        public long AddStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
		{
			try
			{
                return _storePaymentGatewayRepository.AddStorePaymentGateway(storePaymentGateway);
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
                return _storePaymentGatewayRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
		{
			try
			{
                return _storePaymentGatewayRepository.UpdateStorePaymentGateway(storePaymentGateway);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteStorePaymentGateway(long storePaymentGatewayId)
		{
			try
			{
                return _storePaymentGatewayRepository.DeleteStorePaymentGateway(storePaymentGatewayId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public StorePaymentGatewayObject GetStorePaymentGateway(long storePaymentGatewayId)
		{
			try
			{
                return _storePaymentGatewayRepository.GetStorePaymentGateway(storePaymentGatewayId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StorePaymentGatewayObject();
			}
		}

        public List<StorePaymentGatewayObject> GetStorePaymentGateways()
		{
			try
			{
                var objList = _storePaymentGatewayRepository.GetStorePaymentGateways();
                if (objList == null || !objList.Any())
			    {
                    return new List<StorePaymentGatewayObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentGatewayObject>();
			}
		}

        public List<StorePaymentGatewayObject> GetStorePaymentGatewayObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _storePaymentGatewayRepository.GetStorePaymentGatewayObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<StorePaymentGatewayObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentGatewayObject>();
            }
        }

        public List<StorePaymentGatewayObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _storePaymentGatewayRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StorePaymentGatewayObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentGatewayObject>();
            }
        }
        
	  }

}
