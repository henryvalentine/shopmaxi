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
    public class ContactTagRepository
    {
        private readonly IShopkeeperRepository<StoreContactTag> _repository;
       private readonly UnitOfWork _uoWork;

       public ContactTagRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreContactTag>(_uoWork);
		}
       
        public long AddContactTag(StoreContactTagObject contactTag)
        {
            try
            {
                if (contactTag == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == contactTag.Name.Trim().ToLower() && (m.ContactTagId != contactTag.ContactTagId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var contactTagEntity = ModelCrossMapper.Map<StoreContactTagObject, StoreContactTag>(contactTag);
                if (contactTagEntity == null || string.IsNullOrEmpty(contactTagEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(contactTagEntity);
                _uoWork.SaveChanges();
                return returnStatus.ContactTagId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateContactTag(StoreContactTagObject contactTag)
        {
            try
            {
                if (contactTag == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == contactTag.Name.Trim().ToLower() && (m.ContactTagId != contactTag.ContactTagId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var contactTagEntity = ModelCrossMapper.Map<StoreContactTagObject, StoreContactTag>(contactTag);
                if (contactTagEntity == null || contactTagEntity.ContactTagId < 1)
                {
                    return -2;
                }
                _repository.Update(contactTagEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteContactTag(long contactTagId)
        {
            try
            {
                var returnStatus = _repository.Remove(contactTagId);
                _uoWork.SaveChanges();
                return returnStatus.ContactTagId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreContactTagObject GetContactTag(long contactTagId)
        {
            try
            {
                var myItem = _repository.GetById(contactTagId);
                if (myItem == null || myItem.ContactTagId < 1)
                {
                    return new StoreContactTagObject();
                }
                var contactTagObject = ModelCrossMapper.Map<StoreContactTag, StoreContactTagObject>(myItem);
                if (contactTagObject == null || contactTagObject.ContactTagId < 1)
                {
                    return new StoreContactTagObject();
                }
                return contactTagObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreContactTagObject();
            }
        }

        public List<StoreContactTagObject> GetContactTagObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreContactTag> contactTagEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    contactTagEntityList = _repository.GetWithPaging(m => m.ContactTagId, tpageNumber, tsize).ToList();
                }

                else
                {
                    contactTagEntityList = _repository.GetAll().ToList();
                }

                if (!contactTagEntityList.Any())
                {
                    return new List<StoreContactTagObject>();
                }
                var contactTagObjList = new List<StoreContactTagObject>();
                contactTagEntityList.ForEach(m =>
                {
                    var contactTagObject = ModelCrossMapper.Map<StoreContactTag, StoreContactTagObject>(m);
                    if (contactTagObject != null && contactTagObject.ContactTagId > 0)
                    {
                        contactTagObjList.Add(contactTagObject);
                    }
                });

                return contactTagObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactTagObject>();
            }
        }

        public List<StoreContactTagObject> Search(string searchCriteria)
        {
            try
            {
               var contactTagEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!contactTagEntityList.Any())
                {
                    return new List<StoreContactTagObject>();
                }
                var contactTagObjList = new List<StoreContactTagObject>();
                contactTagEntityList.ForEach(m =>
                {
                    var contactTagObject = ModelCrossMapper.Map<StoreContactTag, StoreContactTagObject>(m);
                    if (contactTagObject != null && contactTagObject.ContactTagId > 0)
                    {
                        contactTagObjList.Add(contactTagObject);
                    }
                });
                return contactTagObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactTagObject>();
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

        public int GetObjectCount(Expression<Func<StoreContactTag, bool>> predicate)
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

        public List<StoreContactTagObject> GetContactTags()
        {
            try
            {
                var contactTagEntityList = _repository.GetAll().ToList();
                if (!contactTagEntityList.Any())
                {
                    return new List<StoreContactTagObject>();
                }
                var contactTagObjList = new List<StoreContactTagObject>();
                contactTagEntityList.ForEach(m =>
                {
                    var contactTagObject = ModelCrossMapper.Map<StoreContactTag, StoreContactTagObject>(m);
                    if (contactTagObject != null && contactTagObject.ContactTagId > 0)
                    {
                        contactTagObjList.Add(contactTagObject);
                    }
                });
                return contactTagObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
