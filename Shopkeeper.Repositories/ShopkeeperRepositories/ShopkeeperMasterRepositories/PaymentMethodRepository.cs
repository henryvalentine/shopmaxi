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
    public class PaymentMethodRepository
    {
       private readonly IShopkeeperRepository<PaymentMethod> _repository;
       private readonly UnitOfWork _uoWork;

       public PaymentMethodRepository()
       {
           var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
           var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<PaymentMethod>(_uoWork);
	   }
       public long AddPaymentMethod(PaymentMethodObject paymentMethod)
        {
            try
            {
                if (paymentMethod == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == paymentMethod.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var paymentMethodEntity = ModelCrossMapper.Map<PaymentMethodObject, PaymentMethod>(paymentMethod);
                if (paymentMethodEntity == null || string.IsNullOrEmpty(paymentMethodEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(paymentMethodEntity);
                _uoWork.SaveChanges();
                return returnStatus.PaymentMethodId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
       public Int32 UpdatePaymentMethod(PaymentMethodObject paymentMethod)
        {
            try
            {
                if (paymentMethod == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == paymentMethod.Name.Trim().ToLower() && (m.PaymentMethodId != paymentMethod.PaymentMethodId)) > 0)
                {
                    return -3;
                }
                
                var paymentMethodEntity = ModelCrossMapper.Map<PaymentMethodObject, PaymentMethod>(paymentMethod);
                if (paymentMethodEntity == null || paymentMethodEntity.PaymentMethodId < 1)
                {
                    return -2;
                }
                _repository.Update(paymentMethodEntity);
                _uoWork.SaveChanges();
                return 5;
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
                var returnStatus = _repository.Remove(paymentMethodId);
                _uoWork.SaveChanges();
                return returnStatus.PaymentMethodId > 0;
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
                var myItem = _repository.GetById(paymentMethodId);
                if (myItem == null || myItem.PaymentMethodId < 1)
                {
                    return new PaymentMethodObject();
                }
                var paymentMethodObject = ModelCrossMapper.Map<PaymentMethod, PaymentMethodObject>(myItem);
                if (paymentMethodObject == null || paymentMethodObject.PaymentMethodId < 1)
                {
                    return new PaymentMethodObject();
                }
                return paymentMethodObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PaymentMethodObject();
            }
        }
       public List<PaymentMethodObject> GetPaymentMethodObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<PaymentMethod> paymentMethodEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        paymentMethodEntityList = _repository.GetWithPaging(m => m.PaymentMethodId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        paymentMethodEntityList = _repository.GetAll().ToList();
                    }

                    if (!paymentMethodEntityList.Any())
                    {
                        return new List<PaymentMethodObject>();
                    }
                    var paymentMethodObjList = new List<PaymentMethodObject>();
                    paymentMethodEntityList.ForEach(m =>
                    {
                        var paymentMethodObject = ModelCrossMapper.Map<PaymentMethod, PaymentMethodObject>(m);
                        if (paymentMethodObject != null && paymentMethodObject.PaymentMethodId > 0)
                        {
                            paymentMethodObjList.Add(paymentMethodObject);
                        }
                    });

                return paymentMethodObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentMethodObject>();
            }
        }
       public List<PaymentMethodObject> Search(string searchCriteria)
        {
            try
            {
                var paymentMethodEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!paymentMethodEntityList.Any())
                {
                    return new List<PaymentMethodObject>();
                }
                var paymentMethodObjList = new List<PaymentMethodObject>();
                paymentMethodEntityList.ForEach(m =>
                {
                    var paymentMethodObject = ModelCrossMapper.Map<PaymentMethod, PaymentMethodObject>(m);
                    if (paymentMethodObject != null && paymentMethodObject.PaymentMethodId > 0)
                    {
                        paymentMethodObjList.Add(paymentMethodObject);
                    }
                });
                return paymentMethodObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentMethodObject>();
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
       public List<PaymentMethodObject> GetPaymentMethods()
        {
            try
            {
                var paymentMethodEntityList = _repository.GetAll().ToList();
                if (!paymentMethodEntityList.Any())
                {
                    return new List<PaymentMethodObject>();
                }
                var paymentMethodObjList = new List<PaymentMethodObject>();
                paymentMethodEntityList.ForEach(m =>
                {
                    var paymentMethodObject = ModelCrossMapper.Map<PaymentMethod, PaymentMethodObject>(m);
                    if (paymentMethodObject != null && paymentMethodObject.PaymentMethodId > 0)
                    {
                        paymentMethodObjList.Add(paymentMethodObject);
                    }
                });
                return paymentMethodObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
