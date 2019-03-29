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
    public class StoreDepartmentRepository
    {
       private readonly IShopkeeperRepository<StoreDepartment> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreDepartmentRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreDepartment>(_uoWork);
		}
       
        public long AddStoreDepartment(StoreDepartmentObject storeDepartment)
        {
            try
            {
                if (storeDepartment == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == storeDepartment.Name.Trim().ToLower());
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeDepartmentEntity = ModelCrossMapper.Map<StoreDepartmentObject, StoreDepartment>(storeDepartment);
                if (storeDepartmentEntity == null || string.IsNullOrEmpty(storeDepartmentEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeDepartmentEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreDepartmentId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreDepartment(StoreDepartmentObject storeDepartment)
        {
            try
            {
                if (storeDepartment == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == storeDepartment.Name.Trim().ToLower() && (m.StoreDepartmentId != storeDepartment.StoreDepartmentId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var storeDepartmentEntity = ModelCrossMapper.Map<StoreDepartmentObject, StoreDepartment>(storeDepartment);
                if (storeDepartmentEntity == null || storeDepartmentEntity.StoreDepartmentId < 1)
                {
                    return -2;
                }
                _repository.Update(storeDepartmentEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreDepartment(long storeDepartmentId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeDepartmentId);
                _uoWork.SaveChanges();
                return returnStatus.StoreDepartmentId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreDepartmentObject GetStoreDepartment(long storeDepartmentId)
        {
            try
            {
                var myItem = _repository.GetById(storeDepartmentId);
                if (myItem == null || myItem.StoreDepartmentId < 1)
                {
                    return new StoreDepartmentObject();
                }
                var storeDepartmentObject = ModelCrossMapper.Map<StoreDepartment, StoreDepartmentObject>(myItem);
                if (storeDepartmentObject == null || storeDepartmentObject.StoreDepartmentId < 1)
                {
                    return new StoreDepartmentObject();
                }
                return storeDepartmentObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreDepartmentObject();
            }
        }

        public List<StoreDepartmentObject> GetStoreDepartmentObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreDepartment> storeDepartmentEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeDepartmentEntityList = _repository.GetWithPaging(m => m.StoreDepartmentId, tpageNumber, tsize).ToList();
                }

                else
                {
                    storeDepartmentEntityList = _repository.GetAll().ToList();
                }

                if (!storeDepartmentEntityList.Any())
                {
                    return new List<StoreDepartmentObject>();
                }
                var storeDepartmentObjList = new List<StoreDepartmentObject>();
                storeDepartmentEntityList.ForEach(m =>
                {
                    var storeDepartmentObject = ModelCrossMapper.Map<StoreDepartment, StoreDepartmentObject>(m);
                    if (storeDepartmentObject != null && storeDepartmentObject.StoreDepartmentId > 0)
                    {
                        storeDepartmentObjList.Add(storeDepartmentObject);
                    }
                });

                return storeDepartmentObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreDepartmentObject>();
            }
        }

        public List<StoreDepartmentObject> Search(string searchCriteria)
        {
            try
            {
               var storeDepartmentEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!storeDepartmentEntityList.Any())
                {
                    return new List<StoreDepartmentObject>();
                }
                var storeDepartmentObjList = new List<StoreDepartmentObject>();
                storeDepartmentEntityList.ForEach(m =>
                {
                    var storeDepartmentObject = ModelCrossMapper.Map<StoreDepartment, StoreDepartmentObject>(m);
                    if (storeDepartmentObject != null && storeDepartmentObject.StoreDepartmentId > 0)
                    {
                        storeDepartmentObjList.Add(storeDepartmentObject);
                    }
                });
                return storeDepartmentObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreDepartmentObject>();
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

        public int GetObjectCount(Expression<Func<StoreDepartment, bool>> predicate)
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

        public List<StoreDepartmentObject> GetStoreDepartments()
        {
            try
            {
                var storeDepartmentEntityList = _repository.GetAll().ToList();
                if (!storeDepartmentEntityList.Any())
                {
                    return new List<StoreDepartmentObject>();
                }
                var storeDepartmentObjList = new List<StoreDepartmentObject>();
                storeDepartmentEntityList.ForEach(m =>
                {
                    var storeDepartmentObject = ModelCrossMapper.Map<StoreDepartment, StoreDepartmentObject>(m);
                    if (storeDepartmentObject != null && storeDepartmentObject.StoreDepartmentId > 0)
                    {
                        storeDepartmentObjList.Add(storeDepartmentObject);
                    }
                });
                return storeDepartmentObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
