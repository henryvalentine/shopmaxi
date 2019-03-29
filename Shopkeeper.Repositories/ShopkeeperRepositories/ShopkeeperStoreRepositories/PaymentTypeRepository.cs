using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class PaymentTypeRepository
    {
        private readonly IShopkeeperRepository<PaymentType> _repository;
       private readonly UnitOfWork _uoWork;

       public PaymentTypeRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<PaymentType>(_uoWork);
		}
       
        public long AddPaymentType(PaymentTypeObject paymentType)
        {
            try
            {
                if (paymentType == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == paymentType.Name.Trim().ToLower() && (m.PaymentTypeId != paymentType.PaymentTypeId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var paymentTypeEntity = ModelCrossMapper.Map<PaymentTypeObject, PaymentType>(paymentType);
                if (paymentTypeEntity == null || string.IsNullOrEmpty(paymentTypeEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(paymentTypeEntity);
                _uoWork.SaveChanges();
                return returnStatus.PaymentTypeId;
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
                if (paymentType == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == paymentType.Name.Trim().ToLower() && (m.PaymentTypeId != paymentType.PaymentTypeId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var paymentTypeEntity = ModelCrossMapper.Map<PaymentTypeObject, PaymentType>(paymentType);
                if (paymentTypeEntity == null || paymentTypeEntity.PaymentTypeId < 1)
                {
                    return -2;
                }
                _repository.Update(paymentTypeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeletePaymentType(long paymentTypeId)
        {
            try
            {
                var returnStatus = _repository.Remove(paymentTypeId);
                _uoWork.SaveChanges();
                return returnStatus.PaymentTypeId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public PaymentTypeObject GetPaymentType(long paymentTypeId)
        {
            try
            {
                var myItem = _repository.GetById(paymentTypeId);
                if (myItem == null || myItem.PaymentTypeId < 1)
                {
                    return new PaymentTypeObject();
                }
                var paymentTypeObject = ModelCrossMapper.Map<PaymentType, PaymentTypeObject>(myItem);
                if (paymentTypeObject == null || paymentTypeObject.PaymentTypeId < 1)
                {
                    return new PaymentTypeObject();
                }
                return paymentTypeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PaymentTypeObject();
            }
        }

        public List<PaymentTypeObject> GetPaymentTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<PaymentType> paymentTypeEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    paymentTypeEntityList = _repository.GetWithPaging(m => m.PaymentTypeId, tpageNumber, tsize).ToList();
                }

                else
                {
                    paymentTypeEntityList = _repository.GetAll().ToList();
                }

                if (!paymentTypeEntityList.Any())
                {
                    return new List<PaymentTypeObject>();
                }
                var paymentTypeObjList = new List<PaymentTypeObject>();
                paymentTypeEntityList.ForEach(m =>
                {
                    var paymentTypeObject = ModelCrossMapper.Map<PaymentType, PaymentTypeObject>(m);
                    if (paymentTypeObject != null && paymentTypeObject.PaymentTypeId > 0)
                    {
                        paymentTypeObjList.Add(paymentTypeObject);
                    }
                });

                return paymentTypeObjList;
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
                var paymentTypeEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();
                if (!paymentTypeEntityList.Any())
                {
                    return new List<PaymentTypeObject>();
                }
                var paymentTypeObjList = new List<PaymentTypeObject>();
                paymentTypeEntityList.ForEach(m =>
                {
                    var paymentTypeObject = ModelCrossMapper.Map<PaymentType, PaymentTypeObject>(m);
                    if (paymentTypeObject != null && paymentTypeObject.PaymentTypeId > 0)
                    {
                        paymentTypeObjList.Add(paymentTypeObject);
                    }
                });
                return paymentTypeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PaymentTypeObject>();
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

        public int GetObjectCount(Expression<Func<PaymentType, bool>> predicate)
        {
            try
            {
                return _repository.Count(predicate);
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
                var paymentTypeEntityList = _repository.GetAll().ToList();
                if (!paymentTypeEntityList.Any())
                {
                    return new List<PaymentTypeObject>();
                }
                var paymentTypeObjList = new List<PaymentTypeObject>();
                paymentTypeEntityList.ForEach(m =>
                {
                    var paymentTypeObject = ModelCrossMapper.Map<PaymentType, PaymentTypeObject>(m);
                    if (paymentTypeObject != null && paymentTypeObject.PaymentTypeId > 0)
                    {
                        paymentTypeObjList.Add(paymentTypeObject);
                    }
                });
                return paymentTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
