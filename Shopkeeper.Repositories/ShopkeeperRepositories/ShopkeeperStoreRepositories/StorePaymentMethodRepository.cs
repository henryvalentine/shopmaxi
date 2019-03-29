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
    public class StorePaymentMethodRepository
    {
       private readonly IShopkeeperRepository<StorePaymentMethod> _repository;
       private readonly UnitOfWork _uoWork;

       public StorePaymentMethodRepository()
       {
           var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
           var storeSetting = new SessionHelpers().GetStoreInfo();
           if (storeSetting != null && storeSetting.StoreId > 0)
           {
               connectionString = storeSetting.EntityConnectionString;
           }
           var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StorePaymentMethod>(_uoWork);
	   }
       public long AddStorePaymentMethod(StorePaymentMethodObject storePaymentMethod)
        {
            try
            {
                if (storePaymentMethod == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.Name.Trim().ToLower() == storePaymentMethod.Name.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var storePaymentMethodEntity = ModelCrossMapper.Map<StorePaymentMethodObject, StorePaymentMethod>(storePaymentMethod);
                if (storePaymentMethodEntity == null || string.IsNullOrEmpty(storePaymentMethodEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storePaymentMethodEntity);
                _uoWork.SaveChanges();
                return returnStatus.StorePaymentMethodId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
       public Int32 UpdateStorePaymentMethod(StorePaymentMethodObject storePaymentMethod)
        {
            try
            {
                if (storePaymentMethod == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.Name.Trim().ToLower() == storePaymentMethod.Name.Trim().ToLower() && (m.StorePaymentMethodId != storePaymentMethod.StorePaymentMethodId)) > 0)
                {
                    return -3;
                }
                
                var storePaymentMethodEntity = ModelCrossMapper.Map<StorePaymentMethodObject, StorePaymentMethod>(storePaymentMethod);
                if (storePaymentMethodEntity == null || storePaymentMethodEntity.StorePaymentMethodId < 1)
                {
                    return -2;
                }
                _repository.Update(storePaymentMethodEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }
       public bool DeleteStorePaymentMethod(long storePaymentMethodId)
        {
            try
            {
                var returnStatus = _repository.Remove(storePaymentMethodId);
                _uoWork.SaveChanges();
                return returnStatus.StorePaymentMethodId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
       public StorePaymentMethodObject GetStorePaymentMethod(long storePaymentMethodId)
        {
            try
            {
                var myItem = _repository.GetById(storePaymentMethodId);
                if (myItem == null || myItem.StorePaymentMethodId < 1)
                {
                    return new StorePaymentMethodObject();
                }
                var storePaymentMethodObject = ModelCrossMapper.Map<StorePaymentMethod, StorePaymentMethodObject>(myItem);
                if (storePaymentMethodObject == null || storePaymentMethodObject.StorePaymentMethodId < 1)
                {
                    return new StorePaymentMethodObject();
                }
                return storePaymentMethodObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StorePaymentMethodObject();
            }
        }
       public List<StorePaymentMethodObject> GetStorePaymentMethodObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StorePaymentMethod> storePaymentMethodEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storePaymentMethodEntityList = _repository.GetWithPaging(m => m.StorePaymentMethodId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        storePaymentMethodEntityList = _repository.GetAll().ToList();
                    }

                    if (!storePaymentMethodEntityList.Any())
                    {
                        return new List<StorePaymentMethodObject>();
                    }
                    var storePaymentMethodObjList = new List<StorePaymentMethodObject>();
                    storePaymentMethodEntityList.ForEach(m =>
                    {
                        var storePaymentMethodObject = ModelCrossMapper.Map<StorePaymentMethod, StorePaymentMethodObject>(m);
                        if (storePaymentMethodObject != null && storePaymentMethodObject.StorePaymentMethodId > 0)
                        {
                            storePaymentMethodObjList.Add(storePaymentMethodObject);
                        }
                    });

                return storePaymentMethodObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentMethodObject>();
            }
        }
       public List<StorePaymentMethodObject> Search(string searchCriteria)
        {
            try
            {
                var storePaymentMethodEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storePaymentMethodEntityList.Any())
                {
                    return new List<StorePaymentMethodObject>();
                }
                var storePaymentMethodObjList = new List<StorePaymentMethodObject>();
                storePaymentMethodEntityList.ForEach(m =>
                {
                    var storePaymentMethodObject = ModelCrossMapper.Map<StorePaymentMethod, StorePaymentMethodObject>(m);
                    if (storePaymentMethodObject != null && storePaymentMethodObject.StorePaymentMethodId > 0)
                    {
                        storePaymentMethodObjList.Add(storePaymentMethodObject);
                    }
                });
                return storePaymentMethodObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentMethodObject>();
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
       public List<StorePaymentMethodObject> GetStorePaymentMethods()
        {
            try
            {
                var storePaymentMethodEntityList = _repository.GetAll().ToList();
                if (!storePaymentMethodEntityList.Any())
                {
                    return new List<StorePaymentMethodObject>();
                }
                var storePaymentMethodObjList = new List<StorePaymentMethodObject>();
                storePaymentMethodEntityList.ForEach(m =>
                {
                    var storePaymentMethodObject = ModelCrossMapper.Map<StorePaymentMethod, StorePaymentMethodObject>(m);
                    if (storePaymentMethodObject != null && storePaymentMethodObject.StorePaymentMethodId > 0)
                    {
                        storePaymentMethodObjList.Add(storePaymentMethodObject);
                    }
                });
                return storePaymentMethodObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
