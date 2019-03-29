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
    public class UnitOfMeasurementRepository
    {
        private readonly IShopkeeperRepository<UnitsOfMeasurement> _repository;
       private readonly UnitOfWork _uoWork;

       public UnitOfMeasurementRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<UnitsOfMeasurement>(_uoWork);
		}
       
        public long AddUnitsOfMeasurement(UnitsOfMeasurementObject unitsOfMeasurement)
        {
            try
            {
                if (unitsOfMeasurement == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.UoMCode.Trim().ToLower() == unitsOfMeasurement.UoMCode.Trim().ToLower());
                if (duplicates > 0)
                {
                    return -3;
                }
                var unitsOfMeasurementEntity = ModelCrossMapper.Map<UnitsOfMeasurementObject, UnitsOfMeasurement>(unitsOfMeasurement);
                if (unitsOfMeasurementEntity == null || string.IsNullOrEmpty(unitsOfMeasurementEntity.UoMCode))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(unitsOfMeasurementEntity);
                _uoWork.SaveChanges();
                return returnStatus.UnitOfMeasurementId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateUnitsOfMeasurement(UnitsOfMeasurementObject unitsOfMeasurement)
        {
            try
            {
                if (unitsOfMeasurement == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.UoMCode.Trim().ToLower() == unitsOfMeasurement.UoMCode.Trim().ToLower() && (m.UnitOfMeasurementId != unitsOfMeasurement.UnitOfMeasurementId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var unitsOfMeasurementEntity = ModelCrossMapper.Map<UnitsOfMeasurementObject, UnitsOfMeasurement>(unitsOfMeasurement);
                if (unitsOfMeasurementEntity == null || unitsOfMeasurementEntity.UnitOfMeasurementId < 1)
                {
                    return -2;
                }
                _repository.Update(unitsOfMeasurementEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteUnitsOfMeasurement(long unitsOfMeasurementId)
        {
            try
            {
                _repository.Remove(unitsOfMeasurementId);
                _uoWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public UnitsOfMeasurementObject GetUnitOfMeasurement(long unitOfMeasurementId)
        {
            try
            {
                var myItem = _repository.GetById(unitOfMeasurementId);
                if (myItem == null || myItem.UnitOfMeasurementId < 1)
                {
                    return new UnitsOfMeasurementObject();
                }
                var unitsOfMeasurementObject = ModelCrossMapper.Map<UnitsOfMeasurement, UnitsOfMeasurementObject>(myItem);
                if (unitsOfMeasurementObject == null || unitsOfMeasurementObject.UnitOfMeasurementId < 1)
                {
                    return new UnitsOfMeasurementObject();
                }
                return unitsOfMeasurementObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new UnitsOfMeasurementObject();
            }
        }

        public List<UnitsOfMeasurementObject> GetUnitsOfMeasurementObject(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<UnitsOfMeasurement> unitsOfMeasurementEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    unitsOfMeasurementEntityList = _repository.GetWithPaging(m => m.UnitOfMeasurementId, tpageNumber, tsize).ToList();
                }

                else
                {
                    unitsOfMeasurementEntityList = _repository.GetAll().ToList();
                }

                if (!unitsOfMeasurementEntityList.Any())
                {
                    return new List<UnitsOfMeasurementObject>();
                }
                var unitsOfMeasurementObjList = new List<UnitsOfMeasurementObject>();
                unitsOfMeasurementEntityList.ForEach(m =>
                {
                    var unitsOfMeasurementObject = ModelCrossMapper.Map<UnitsOfMeasurement, UnitsOfMeasurementObject>(m);
                    if (unitsOfMeasurementObject != null && unitsOfMeasurementObject.UnitOfMeasurementId > 0)
                    {
                        unitsOfMeasurementObjList.Add(unitsOfMeasurementObject);
                    }
                });

                return unitsOfMeasurementObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UnitsOfMeasurementObject>();
            }
        }

        public List<UnitsOfMeasurementObject> Search(string searchCriteria)
        {
            try
            {
                var unitsOfMeasurementEntityList = _repository.GetAll(m => m.UoMCode.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!unitsOfMeasurementEntityList.Any())
                {
                    return new List<UnitsOfMeasurementObject>();
                }
                var unitsOfMeasurementObjList = new List<UnitsOfMeasurementObject>();
                unitsOfMeasurementEntityList.ForEach(m =>
                {
                    var unitsOfMeasurementObject = ModelCrossMapper.Map<UnitsOfMeasurement, UnitsOfMeasurementObject>(m);
                    if (unitsOfMeasurementObject != null && unitsOfMeasurementObject.UnitOfMeasurementId > 0)
                    {
                        unitsOfMeasurementObjList.Add(unitsOfMeasurementObject);
                    }
                });
                return unitsOfMeasurementObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UnitsOfMeasurementObject>();
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

        public int GetObjectCount(Expression<Func<UnitsOfMeasurement, bool>> predicate)
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

        public List<UnitsOfMeasurementObject> GetUnitsofMeasurement()
        {
            try
            {
                var unitsOfMeasurementEntityList = _repository.GetAll().ToList();
                if (!unitsOfMeasurementEntityList.Any())
                {
                    return new List<UnitsOfMeasurementObject>();
                }
                var unitsOfMeasurementObjList = new List<UnitsOfMeasurementObject>();
                unitsOfMeasurementEntityList.ForEach(m =>
                {
                    var unitsOfMeasurementObject = ModelCrossMapper.Map<UnitsOfMeasurement, UnitsOfMeasurementObject>(m);
                    if (unitsOfMeasurementObject != null && unitsOfMeasurementObject.UnitOfMeasurementId > 0)
                    {
                        unitsOfMeasurementObjList.Add(unitsOfMeasurementObject);
                    }
                });
                return unitsOfMeasurementObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
