using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class PaymentMethodServices
	{
		private readonly PaymentMethodRepository  _paymentMethodRepository;
        public PaymentMethodServices()
		{
            _paymentMethodRepository = new PaymentMethodRepository();
		}
        public long AddPaymentMethod(PaymentMethodObject paymentMethod)
		{
			try
			{
                return _paymentMethodRepository.AddPaymentMethod(paymentMethod);
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
        public int UpdatePaymentMethod(PaymentMethodObject paymentMethod)
		{
			try
			{
                return _paymentMethodRepository.UpdatePaymentMethod(paymentMethod);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeletePaymentMethod(long paymentMethodId)
		{
			try
			{
                return _paymentMethodRepository.DeletePaymentMethod(paymentMethodId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public PaymentMethodObject GetPaymentMethod(long paymentMethodId)
		{
			try
			{
                return _paymentMethodRepository.GetPaymentMethod(paymentMethodId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PaymentMethodObject();
			}
		}
        public List<PaymentMethodObject> GetPaymentMethods()
		{
			try
			{
                var objList = _paymentMethodRepository.GetPaymentMethods();
                if (objList == null || !objList.Any())
			    {
                    return new List<PaymentMethodObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentMethodObject>();
			}
		}
        public List<PaymentMethodObject> GetPaymentMethodObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _paymentMethodRepository.GetPaymentMethodObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<PaymentMethodObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentMethodObject>();
            }
        }
        
	  }

}
