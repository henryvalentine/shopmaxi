using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class StateRepository
    {
        private readonly IShopkeeperRepository<State> _repository;
       private readonly UnitOfWork _uoWork;

       public StateRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<State>(_uoWork);
		}
       
        public long AddState(StateObject state)
        {
            try
            {
                if (state == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == state.Name.Trim().ToLower() && state.CountryId.Equals(m.CountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var stateEntity = ModelCrossMapper.Map<StateObject, State>(state);
                if (stateEntity == null || string.IsNullOrEmpty(stateEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(stateEntity);
                _uoWork.SaveChanges();
                return returnStatus.StateId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateState(StateObject state)
        {
            try
            {
                if (state == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == state.Name.Trim().ToLower() && state.CountryId.Equals(m.CountryId) && (m.StateId != state.StateId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var stateEntity = ModelCrossMapper.Map<StateObject, State>(state);
                if (stateEntity == null || stateEntity.StateId < 1)
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

        public bool DeleteState(long stateId)
        {
            try
            {
                var returnStatus = _repository.Remove(stateId);
                _uoWork.SaveChanges();
                return returnStatus.StateId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StateObject GetState(long stateId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StateId == stateId, "Country");
                if (myItem == null || myItem.StateId < 1)
                {
                    return new StateObject();
                }
                var stateObject = ModelCrossMapper.Map<State, StateObject>(myItem);
                if (stateObject == null || stateObject.StateId < 1)
                {
                    return new StateObject();
                }
                stateObject.CountryName = myItem.Country.Name;
                return stateObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StateObject();
            }
        }

        public List<StateObject> GetStateObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<State> stateEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    stateEntityList = _repository.GetWithPaging(m => m.CountryId, tpageNumber, tsize, "Country").ToList();
                }

                else
                {
                    stateEntityList = _repository.GetAll().ToList();
                }

                if (!stateEntityList.Any())
                {
                    return new List<StateObject>();
                }
                var stateObjList = new List<StateObject>();
                stateEntityList.ForEach(m =>
                {
                    var stateObject = ModelCrossMapper.Map<State, StateObject>(m);
                    if (stateObject != null && stateObject.StateId > 0)
                    {
                        stateObject.CountryName = m.Country.Name;
                        stateObjList.Add(stateObject);
                    }
                });

                return stateObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StateObject>();
            }
        }

        public List<StateObject> Search(string searchCriteria)
        {
            try
            {
               var stateEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()),"Country").ToList();

                if (!stateEntityList.Any())
                {
                    return new List<StateObject>();
                }
                var stateObjList = new List<StateObject>();
                stateEntityList.ForEach(m =>
                {
                    var stateObject = ModelCrossMapper.Map<State, StateObject>(m);
                    if (stateObject != null && stateObject.StateId > 0)
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
                return new List<StateObject>();
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

        public int GetObjectCount(Expression<Func<State, bool>> predicate)
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

        public List<StateObject> GetStates()
        {
            try
            {
                var stateEntityList = _repository.GetAll().ToList();
                if (!stateEntityList.Any())
                {
                    return new List<StateObject>();
                }
                var stateObjList = new List<StateObject>();
                stateEntityList.ForEach(m =>
                {
                    var stateObject = ModelCrossMapper.Map<State, StateObject>(m);
                    if (stateObject != null && stateObject.StateId > 0)
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
