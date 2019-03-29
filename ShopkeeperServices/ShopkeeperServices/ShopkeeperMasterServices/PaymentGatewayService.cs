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
	public class PaymentGatewayServices
	{
		private readonly PaymentGatewayRepository  _paymentGatewayRepository;
        public PaymentGatewayServices()
		{
            _paymentGatewayRepository = new PaymentGatewayRepository();
		}

        public long AddPaymentGateway(PaymentGatewayObject paymentGateway)
		{
			try
			{
                return _paymentGatewayRepository.AddPaymentGateway(paymentGateway);
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
                return _paymentGatewayRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdatePaymentGateway(PaymentGatewayObject paymentGateway)
		{
			try
			{
                return _paymentGatewayRepository.UpdatePaymentGateway(paymentGateway);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeletePaymentGateway(long paymentGatewayId)
		{
			try
			{
                return _paymentGatewayRepository.DeletePaymentGateway(paymentGatewayId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public PaymentGatewayObject GetPaymentGateway(long paymentGatewayId)
		{
			try
			{
                return _paymentGatewayRepository.GetPaymentGateway(paymentGatewayId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PaymentGatewayObject();
			}
		}

        public List<PaymentGatewayObject> GetPaymentGateways()
		{
			try
			{
                var objList = _paymentGatewayRepository.GetPaymentGateways();
                if (objList == null || !objList.Any())
			    {
                    return new List<PaymentGatewayObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentGatewayObject>();
			}
		}

        public List<PaymentGatewayObject> GetPaymentGatewayObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _paymentGatewayRepository.GetPaymentGatewayObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<PaymentGatewayObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentGatewayObject>();
            }
        }

        public List<PaymentGatewayObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _paymentGatewayRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<PaymentGatewayObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentGatewayObject>();
            }
        }
        
	  }

}
