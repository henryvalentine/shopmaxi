using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class PaymentGatewayRepository
    {
       private readonly IShopkeeperRepository<PaymentGateway> _repository;
       private readonly UnitOfWork _uoWork;

       public PaymentGatewayRepository()
       {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
            _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<PaymentGateway>(_uoWork);
	   }
       
        public long AddPaymentGateway(PaymentGatewayObject paymentGateway)
        {
            try
            {
                if (paymentGateway == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.GatewayName.Trim().ToLower() == paymentGateway.GatewayName.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var paymentGatewayEntity = ModelCrossMapper.Map<PaymentGatewayObject, PaymentGateway>(paymentGateway);
                if (paymentGatewayEntity == null || string.IsNullOrEmpty(paymentGatewayEntity.GatewayName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(paymentGatewayEntity);
                _uoWork.SaveChanges();
                return returnStatus.PaymentGatewayId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdatePaymentGateway(PaymentGatewayObject paymentGateway)
        {
            try
            {
                if (paymentGateway == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.GatewayName.Trim().ToLower() == paymentGateway.GatewayName.Trim().ToLower() && (m.PaymentGatewayId != paymentGateway.PaymentGatewayId)) > 0)
                {
                    return -3;
                }
                
                var paymentGatewayEntity = ModelCrossMapper.Map<PaymentGatewayObject, PaymentGateway>(paymentGateway);
                if (paymentGatewayEntity == null || paymentGatewayEntity.PaymentGatewayId < 1)
                {
                    return -2;
                }
                _repository.Update(paymentGatewayEntity);
                _uoWork.SaveChanges();
                return 5;
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
                var returnStatus = _repository.Remove(paymentGatewayId);
                _uoWork.SaveChanges();
                return returnStatus.PaymentGatewayId > 0;
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
                var myItem = _repository.GetById(paymentGatewayId);
                if (myItem == null || myItem.PaymentGatewayId < 1)
                {
                    return new PaymentGatewayObject();
                }
                var paymentGatewayObject = ModelCrossMapper.Map<PaymentGateway, PaymentGatewayObject>(myItem);
                if (paymentGatewayObject == null || paymentGatewayObject.PaymentGatewayId < 1)
                {
                    return new PaymentGatewayObject();
                }
                return paymentGatewayObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PaymentGatewayObject();
            }
        }

        public List<PaymentGatewayObject> GetPaymentGatewayObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<PaymentGateway> paymentGatewayEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        paymentGatewayEntityList = _repository.GetWithPaging(m => m.PaymentGatewayId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        paymentGatewayEntityList = _repository.GetAll().ToList();
                    }

                    if (!paymentGatewayEntityList.Any())
                    {
                        return new List<PaymentGatewayObject>();
                    }
                    var paymentGatewayObjList = new List<PaymentGatewayObject>();
                    paymentGatewayEntityList.ForEach(m =>
                    {
                        var paymentGatewayObject = ModelCrossMapper.Map<PaymentGateway, PaymentGatewayObject>(m);
                        if (paymentGatewayObject != null && paymentGatewayObject.PaymentGatewayId > 0)
                        {
                            paymentGatewayObjList.Add(paymentGatewayObject);
                        }
                    });

                return paymentGatewayObjList;
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
                var paymentGatewayEntityList = _repository.GetAll(m => m.GatewayName.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!paymentGatewayEntityList.Any())
                {
                    return new List<PaymentGatewayObject>();
                }
                var paymentGatewayObjList = new List<PaymentGatewayObject>();
                paymentGatewayEntityList.ForEach(m =>
                {
                    var paymentGatewayObject = ModelCrossMapper.Map<PaymentGateway, PaymentGatewayObject>(m);
                    if (paymentGatewayObject != null && paymentGatewayObject.PaymentGatewayId > 0)
                    {
                        paymentGatewayObjList.Add(paymentGatewayObject);
                    }
                });
                return paymentGatewayObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentGatewayObject>();
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _repository.Count();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<PaymentGatewayObject> GetPaymentGateways()
        {
            try
            {
                var paymentGatewayEntityList = _repository.GetAll().ToList();
                if (!paymentGatewayEntityList.Any())
                {
                    return new List<PaymentGatewayObject>();
                }
                var paymentGatewayObjList = new List<PaymentGatewayObject>();
                paymentGatewayEntityList.ForEach(m =>
                {
                    var paymentGatewayObject = ModelCrossMapper.Map<PaymentGateway, PaymentGatewayObject>(m);
                    if (paymentGatewayObject != null && paymentGatewayObject.PaymentGatewayId > 0)
                    {
                        paymentGatewayObjList.Add(paymentGatewayObject);
                    }
                });
                return paymentGatewayObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
