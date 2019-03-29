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
    public class StoreStateRepository
    {
        private readonly IShopkeeperRepository<StoreState> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreStateRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreState>(_uoWork);
		}

       public long AddStoreState(StoreStateObject state)
       {
           try
           {
               if (state == null)
               {
                   return -2;
               }
               var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == state.Name.Trim().ToLower() && state.StoreCountryId.Equals(m.StoreCountryId));
               if (duplicates > 0)
               {
                   return -3;
               }
               var stateEntity = ModelCrossMapper.Map<StoreStateObject, StoreState>(state);
               if (stateEntity == null || string.IsNullOrEmpty(stateEntity.Name))
               {
                   return -2;
               }
               var returnStatus = _repository.Add(stateEntity);
               _uoWork.SaveChanges();
               return returnStatus.StoreStateId;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public int UpdateStoreState(StoreStateObject state)
       {
           try
           {
               if (state == null)
               {
                   return -2;
               }
               var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == state.Name.Trim().ToLower() && state.StoreCountryId.Equals(m.StoreCountryId) && (m.StoreStateId != state.StoreStateId));
               if (duplicates > 0)
               {
                   return -3;
               }
               var stateEntity = ModelCrossMapper.Map<StoreStateObject, StoreState>(state);
               if (stateEntity == null || stateEntity.StoreStateId < 1)
               {
                   return -2;
               }
               _repository.Update(stateEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteStoreState(long stateId)
       {
           try
           {
               var returnStatus = _repository.Remove(stateId);
               _uoWork.SaveChanges();
               return returnStatus.StoreStateId > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public StoreStateObject GetStoreState(long stateId)
       {
           try
           {
               var myItem = _repository.Get(m => m.StoreStateId == stateId, "StoreCountry");
               if (myItem == null || myItem.StoreStateId < 1)
               {
                   return new StoreStateObject();
               }
               var stateObject = ModelCrossMapper.Map<StoreState, StoreStateObject>(myItem);
               if (stateObject == null || stateObject.StoreStateId < 1)
               {
                   return new StoreStateObject();
               }
               stateObject.CountryName = myItem.StoreCountry.Name;
               return stateObject;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new StoreStateObject();
           }
       }

       public List<StoreStateObject> GetStoreStateObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               List<StoreState> stateEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   stateEntityList = _repository.GetWithPaging(m => m.StoreCountryId, tpageNumber, tsize, "StoreCountry").ToList();
               }

               else
               {
                   stateEntityList = _repository.GetAll().ToList();
               }

               if (!stateEntityList.Any())
               {
                   return new List<StoreStateObject>();
               }
               var stateObjList = new List<StoreStateObject>();
               stateEntityList.ForEach(m =>
               {
                   var stateObject = ModelCrossMapper.Map<StoreState, StoreStateObject>(m);
                   if (stateObject != null && stateObject.StoreStateId > 0)
                   {
                       stateObject.CountryName = m.StoreCountry.Name;
                       stateObjList.Add(stateObject);
                   }
               });

               return stateObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreStateObject>();
           }
       }

       public List<StoreStateObject> Search(string searchCriteria)
       {
           try
           {
               var stateEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()), "StoreCountry").ToList();

               if (!stateEntityList.Any())
               {
                   return new List<StoreStateObject>();
               }
               var stateObjList = new List<StoreStateObject>();
               stateEntityList.ForEach(m =>
               {
                   var stateObject = ModelCrossMapper.Map<StoreState, StoreStateObject>(m);
                   if (stateObject != null && stateObject.StoreStateId > 0)
                   {
                       stateObject.CountryName = m.Name;
                       stateObjList.Add(stateObject);
                   }
               });
               return stateObjList;
           }

           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<StoreStateObject>();
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

       public int GetObjectCount(Expression<Func<StoreState, bool>> predicate)
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

       public List<StoreStateObject> GetStoreStates()
       {
           try
           {
               var stateEntityList = _repository.GetAll().ToList();
               if (!stateEntityList.Any())
               {
                   return new List<StoreStateObject>();
               }
               var stateObjList = new List<StoreStateObject>();
               stateEntityList.ForEach(m =>
               {
                   var stateObject = ModelCrossMapper.Map<StoreState, StoreStateObject>(m);
                   if (stateObject != null && stateObject.StoreStateId > 0)
                   {
                       stateObjList.Add(stateObject);
                   }
               });
               return stateObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
    }
}
