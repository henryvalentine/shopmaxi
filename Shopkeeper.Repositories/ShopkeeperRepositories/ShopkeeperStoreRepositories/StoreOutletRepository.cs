using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreOutletRepository
    {
       private readonly IShopkeeperRepository<StoreOutlet> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreOutletRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
            _repository = new ShopkeeperRepository<StoreOutlet>(_uoWork);
		}
       
        public long AddStoreOutlet(StoreOutletObject storeOutlet)
        {
            try
            {
                if (storeOutlet == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.OutletName.Trim().ToLower() == storeOutlet.OutletName.Trim().ToLower()) > 0)
                {
                    return -3;
                }
                var storeOutletEntity = ModelCrossMapper.Map<StoreOutletObject, StoreOutlet>(storeOutlet);
                if (storeOutletEntity == null || storeOutletEntity.StoreAddressId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeOutletEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreOutletId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreOutlet(StoreOutletObject storeOutlet)
        {
            try
            {
                if (storeOutlet == null)
                {
                    return -2;
                }
                if (_repository.Count(m => m.OutletName.Trim().ToLower() == storeOutlet.OutletName.Trim().ToLower() && m.StoreOutletId != storeOutlet.StoreOutletId) > 0)
                {
                    return -3;
                }
                var storeOutletEntity = ModelCrossMapper.Map<StoreOutletObject, StoreOutlet>(storeOutlet);
                if (storeOutletEntity == null || storeOutletEntity.StoreOutletId < 1)
                {
                    return -2;
                }
                _repository.Update(storeOutletEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreOutlet(long storeOutletId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeOutletId);
                _uoWork.SaveChanges();
                return returnStatus.StoreOutletId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreOutletObject GetStoreOutlet(long storeOutletId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreOutletId == storeOutletId, "StoreAddress");
                if (myItem == null || myItem.StoreOutletId < 1)
                {
                    return new StoreOutletObject();
                }
                var storeOutletObject = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(myItem);
                if (storeOutletObject == null || storeOutletObject.StoreOutletId < 1)
                {
                    return new StoreOutletObject();
                }
                storeOutletObject.Address = myItem.StoreAddress.StreetNo;
                var city = new StoreCityRepository().GetStoreCity(myItem.StoreAddress.StoreCityId);
                if (city != null && city.StoreCityId > 0)
                {
                    storeOutletObject.CityName = city.Name;
                    storeOutletObject.StoreCityId = city.StoreCityId;
                }
                return storeOutletObject;
            } 
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreOutletObject();
            }
        }

        public List<StoreOutletObject> GetStoreOutletObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreOutlet> storeOutletEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeOutletEntityList = _repository.GetWithPaging(m => m.StoreOutletId, tpageNumber, tsize, "StoreAddress").ToList();
                    }

                    else
                    {
                        storeOutletEntityList = _repository.GetAll("StoreAddress").ToList();
                    }

                    if (!storeOutletEntityList.Any())
                    {
                        return new List<StoreOutletObject>();
                    }
                    var storeOutletObjList = new List<StoreOutletObject>();
                    storeOutletEntityList.ForEach(m =>
                    {
                        var storeOutletObject = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(m);
                        if (storeOutletObject != null && storeOutletObject.StoreOutletId > 0)
                        {
                            storeOutletObjList.Add(storeOutletObject);
                        }
                    });

                return storeOutletObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreOutletObject>();
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

        public List<StoreOutletObject> GetStoreOutlets()
        {
            try
            {
                var storeOutletEntityList = _repository.GetAll().ToList();
                if (!storeOutletEntityList.Any())
                {
                    return new List<StoreOutletObject>();
                }
                var storeOutletObjList = new List<StoreOutletObject>();
                storeOutletEntityList.ForEach(m =>
                {
                    var storeOutletObject = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(m);
                    if (storeOutletObject != null && storeOutletObject.StoreOutletId > 0)
                    {
                        storeOutletObjList.Add(storeOutletObject);
                    }
                });
                return storeOutletObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        public List<StoreOutletObject> Search(string searchCriteria)
        {
            try
            {
                var storeOutletEntityList = _repository.GetAll(m => m.OutletName.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storeOutletEntityList.Any())
                {
                    return new List<StoreOutletObject>();
                }

                var storeItemObjList = new List<StoreOutletObject>();
                storeOutletEntityList.ForEach(m =>
                {
                    var storeOutletObject = ModelCrossMapper.Map<StoreOutlet, StoreOutletObject>(m);
                    if (storeOutletObject != null && storeOutletObject.StoreOutletId > 0)
                    {
                       storeItemObjList.Add(storeOutletObject);
                    }
                });

                return storeItemObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreOutletObject>();
            }
        }

        public List<StoreOutlet> SearchByItemStock(string searchCriteria)
        {
            try
            {
                return _repository.GetAll(m => m.OutletName.ToLower().Contains(searchCriteria.ToLower()), "StoreItemStocks").ToList();
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreOutlet>();
            }
        }
    }
}
