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
	public class DeliveryMethodServices
	{
		private readonly DeliveryMethodRepository  _deliveryMethodAccountRepository;
        public DeliveryMethodServices()
		{
            _deliveryMethodAccountRepository = new DeliveryMethodRepository();
		}

        public int AddDeliveryMethod(DeliveryMethodObject deliveryMethodAccount)
		{
			try
			{
                return _deliveryMethodAccountRepository.AddDeliveryMethod(deliveryMethodAccount);
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
                return _deliveryMethodAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateDeliveryMethod(DeliveryMethodObject deliveryMethodAccount)
		{
			try
			{
                return _deliveryMethodAccountRepository.UpdateDeliveryMethod(deliveryMethodAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteDeliveryMethod(long deliveryMethodAccountId)
		{
			try
			{
                return _deliveryMethodAccountRepository.DeleteDeliveryMethod(deliveryMethodAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public DeliveryMethodObject GetDeliveryMethod(long deliveryMethodAccountId)
		{
			try
			{
                return _deliveryMethodAccountRepository.GetDeliveryMethod(deliveryMethodAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new DeliveryMethodObject();
			}
		}

        public List<DeliveryMethodObject> GetDeliveryMethods()
		{
			try
			{
                var objList = _deliveryMethodAccountRepository.GetDeliveryMethods();
                if (objList == null || !objList.Any())
			    {
                    return new List<DeliveryMethodObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DeliveryMethodObject>();
			}
		}

        public List<DeliveryMethodObject> GetDeliveryMethodObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _deliveryMethodAccountRepository.GetDeliveryMethodObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<DeliveryMethodObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DeliveryMethodObject>();
            }
        }

        public List<DeliveryMethodObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _deliveryMethodAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<DeliveryMethodObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DeliveryMethodObject>();
            }
        }
	}

}
