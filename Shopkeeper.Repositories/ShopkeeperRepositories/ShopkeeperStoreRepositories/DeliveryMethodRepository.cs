using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class DeliveryMethodRepository
    {
        private readonly IShopkeeperRepository<DeliveryMethod> _repository;
       private readonly UnitOfWork _uoWork;

       public DeliveryMethodRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<DeliveryMethod>(_uoWork);
		}
       
        public int AddDeliveryMethod(DeliveryMethodObject deliveryMethod)
        {
            try
            {
                if (deliveryMethod == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.MethodTitle.Trim().ToLower().Equals(deliveryMethod.MethodTitle.Trim().ToLower()));
                if (duplicates > 0)
                {
                    return -3;
                }
                var deliveryMethodEntity = ModelCrossMapper.Map<DeliveryMethodObject, DeliveryMethod>(deliveryMethod);
                if (deliveryMethodEntity == null || string.IsNullOrEmpty(deliveryMethodEntity.MethodTitle))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(deliveryMethodEntity);
                _uoWork.SaveChanges();
                return returnStatus.DeliveryMethodId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateDeliveryMethod(DeliveryMethodObject deliveryMethod)
        {
            try
            {
                if (deliveryMethod == null)
                {
                    return -2;
                }

                var duplicates = _repository.Count(m => m.MethodTitle.Trim().ToLower().Equals(deliveryMethod.MethodTitle.Trim().ToLower()) && (m.DeliveryMethodId != deliveryMethod.DeliveryMethodId));
                if (duplicates > 0)
                {
                    return -3;
                }

                var deliveryMethodEntity = ModelCrossMapper.Map<DeliveryMethodObject, DeliveryMethod>(deliveryMethod);
                if (deliveryMethodEntity == null || deliveryMethodEntity.DeliveryMethodId < 1)
                {
                    return -2;
                }
                _repository.Update(deliveryMethodEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteDeliveryMethod(long deliveryMethodId)
        {
            try
            {
                var returnStatus = _repository.Remove(deliveryMethodId);
                _uoWork.SaveChanges();
                return returnStatus.DeliveryMethodId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public DeliveryMethodObject GetDeliveryMethod(long deliveryMethodId)
        {
            try
            {
                var myItem = _repository.GetById(deliveryMethodId);
                if (myItem == null || myItem.DeliveryMethodId < 1)
                {
                    return new DeliveryMethodObject();
                }
                var deliveryMethodObject = ModelCrossMapper.Map<DeliveryMethod, DeliveryMethodObject>(myItem);
                if (deliveryMethodObject == null || deliveryMethodObject.DeliveryMethodId < 1)
                {
                    return new DeliveryMethodObject();
                }
                return deliveryMethodObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new DeliveryMethodObject();
            }
        }

        public List<DeliveryMethodObject> GetDeliveryMethodObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<DeliveryMethod> deliveryMethodEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        deliveryMethodEntityList = _repository.GetWithPaging(m => m.DeliveryMethodId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        deliveryMethodEntityList = _repository.GetAll().ToList();
                    }

                    if (!deliveryMethodEntityList.Any())
                    {
                        return new List<DeliveryMethodObject>();
                    }
                    var deliveryMethodObjList = new List<DeliveryMethodObject>();
                    deliveryMethodEntityList.ForEach(m =>
                    {
                        var deliveryMethodObject = ModelCrossMapper.Map<DeliveryMethod, DeliveryMethodObject>(m);
                        if (deliveryMethodObject != null && deliveryMethodObject.DeliveryMethodId > 0)
                        {
                            deliveryMethodObjList.Add(deliveryMethodObject);
                        }
                    });

                return deliveryMethodObjList;
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
                var deliveryMethodEntityList = _repository.GetAll(m => m.MethodTitle.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!deliveryMethodEntityList.Any())
                {
                    return new List<DeliveryMethodObject>();
                }
                var deliveryMethodObjList = new List<DeliveryMethodObject>();
                deliveryMethodEntityList.ForEach(m =>
                {
                    var deliveryMethodObject = ModelCrossMapper.Map<DeliveryMethod, DeliveryMethodObject>(m);
                    if (deliveryMethodObject != null && deliveryMethodObject.DeliveryMethodId > 0)
                    {
                        deliveryMethodObjList.Add(deliveryMethodObject);
                    }
                });
                return deliveryMethodObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DeliveryMethodObject>();
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

        public List<DeliveryMethodObject> GetDeliveryMethods()
        {
            try
            {
                var deliveryMethodEntityList = _repository.GetAll().ToList();
                if (!deliveryMethodEntityList.Any())
                {
                    return new List<DeliveryMethodObject>();
                }
                var deliveryMethodObjList = new List<DeliveryMethodObject>();
                deliveryMethodEntityList.ForEach(m =>
                {
                    var deliveryMethodObject = ModelCrossMapper.Map<DeliveryMethod, DeliveryMethodObject>(m);
                    if (deliveryMethodObject != null && deliveryMethodObject.DeliveryMethodId > 0)
                    {
                        deliveryMethodObjList.Add(deliveryMethodObject);
                    }
                });
                return deliveryMethodObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
