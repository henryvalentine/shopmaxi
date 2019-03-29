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
    public class StorePaymentGatewayRepository
    {
       private readonly IShopkeeperRepository<StorePaymentGateway> _repository;
       private readonly UnitOfWork _uoWork;

       public StorePaymentGatewayRepository()
       {
           var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
           var storeSetting = new SessionHelpers().GetStoreInfo();
           if (storeSetting != null && storeSetting.StoreId > 0)
           {
               connectionString = storeSetting.EntityConnectionString;
           }
           var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StorePaymentGateway>(_uoWork);
	   }
       
        public long AddStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
        {
            try
            {
                if (storePaymentGateway == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.GatewayName.Trim().ToLower() == storePaymentGateway.GatewayName.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var storePaymentGatewayEntity = ModelCrossMapper.Map<StorePaymentGatewayObject, StorePaymentGateway>(storePaymentGateway);
                if (storePaymentGatewayEntity == null || string.IsNullOrEmpty(storePaymentGatewayEntity.GatewayName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storePaymentGatewayEntity);
                _uoWork.SaveChanges();
                return returnStatus.StorePaymentGatewayId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStorePaymentGateway(StorePaymentGatewayObject storePaymentGateway)
        {
            try
            {
                if (storePaymentGateway == null)
                {
                    return -2;
                }
                
                if (_repository.Count(m => m.GatewayName.Trim().ToLower() == storePaymentGateway.GatewayName.Trim().ToLower() && (m.StorePaymentGatewayId != storePaymentGateway.StorePaymentGatewayId)) > 0)
                {
                    return -3;
                }
                
                var storePaymentGatewayEntity = ModelCrossMapper.Map<StorePaymentGatewayObject, StorePaymentGateway>(storePaymentGateway);
                if (storePaymentGatewayEntity == null || storePaymentGatewayEntity.StorePaymentGatewayId < 1)
                {
                    return -2;
                }
                _repository.Update(storePaymentGatewayEntity);
                _uoWork.SaveChanges();
                return 5;
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
                var returnStatus = _repository.Remove(storePaymentGatewayId);
                _uoWork.SaveChanges();
                return returnStatus.StorePaymentGatewayId > 0;
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
                var myItem = _repository.GetById(storePaymentGatewayId);
                if (myItem == null || myItem.StorePaymentGatewayId < 1)
                {
                    return new StorePaymentGatewayObject();
                }
                var storePaymentGatewayObject = ModelCrossMapper.Map<StorePaymentGateway, StorePaymentGatewayObject>(myItem);
                if (storePaymentGatewayObject == null || storePaymentGatewayObject.StorePaymentGatewayId < 1)
                {
                    return new StorePaymentGatewayObject();
                }
                return storePaymentGatewayObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StorePaymentGatewayObject();
            }
        }

        public List<StorePaymentGatewayObject> GetStorePaymentGatewayObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StorePaymentGateway> storePaymentGatewayEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storePaymentGatewayEntityList = _repository.GetWithPaging(m => m.StorePaymentGatewayId, tpageNumber, tsize).ToList();
                    }

                    else
                    {
                        storePaymentGatewayEntityList = _repository.GetAll().ToList();
                    }

                    if (!storePaymentGatewayEntityList.Any())
                    {
                        return new List<StorePaymentGatewayObject>();
                    }
                    var storePaymentGatewayObjList = new List<StorePaymentGatewayObject>();
                    storePaymentGatewayEntityList.ForEach(m =>
                    {
                        var storePaymentGatewayObject = ModelCrossMapper.Map<StorePaymentGateway, StorePaymentGatewayObject>(m);
                        if (storePaymentGatewayObject != null && storePaymentGatewayObject.StorePaymentGatewayId > 0)
                        {
                            storePaymentGatewayObjList.Add(storePaymentGatewayObject);
                        }
                    });

                return storePaymentGatewayObjList;
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
                var storePaymentGatewayEntityList = _repository.GetAll(m => m.GatewayName.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storePaymentGatewayEntityList.Any())
                {
                    return new List<StorePaymentGatewayObject>();
                }
                var storePaymentGatewayObjList = new List<StorePaymentGatewayObject>();
                storePaymentGatewayEntityList.ForEach(m =>
                {
                    var storePaymentGatewayObject = ModelCrossMapper.Map<StorePaymentGateway, StorePaymentGatewayObject>(m);
                    if (storePaymentGatewayObject != null && storePaymentGatewayObject.StorePaymentGatewayId > 0)
                    {
                        storePaymentGatewayObjList.Add(storePaymentGatewayObject);
                    }
                });
                return storePaymentGatewayObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StorePaymentGatewayObject>();
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

        public List<StorePaymentGatewayObject> GetStorePaymentGateways()
        {
            try
            {
                var storePaymentGatewayEntityList = _repository.GetAll().ToList();
                if (!storePaymentGatewayEntityList.Any())
                {
                    return new List<StorePaymentGatewayObject>();
                }
                var storePaymentGatewayObjList = new List<StorePaymentGatewayObject>();
                storePaymentGatewayEntityList.ForEach(m =>
                {
                    var storePaymentGatewayObject = ModelCrossMapper.Map<StorePaymentGateway, StorePaymentGatewayObject>(m);
                    if (storePaymentGatewayObject != null && storePaymentGatewayObject.StorePaymentGatewayId > 0)
                    {
                        storePaymentGatewayObjList.Add(storePaymentGatewayObject);
                    }
                });
                return storePaymentGatewayObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
