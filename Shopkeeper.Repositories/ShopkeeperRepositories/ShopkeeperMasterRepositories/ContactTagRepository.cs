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
    public class ContactTagRepository
    {
        private readonly IShopkeeperRepository<ContactTag> _repository;
       private readonly UnitOfWork _uoWork;

       public ContactTagRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<ContactTag>(_uoWork);
		}
       
        public long AddContactTag(ContactTagObject contactTag)
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
                var contactTagEntity = ModelCrossMapper.Map<ContactTagObject, ContactTag>(contactTag);
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

        public int UpdateContactTag(ContactTagObject contactTag)
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
                var contactTagEntity = ModelCrossMapper.Map<ContactTagObject, ContactTag>(contactTag);
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

        public ContactTagObject GetContactTag(long contactTagId)
        {
            try
            {
                var myItem = _repository.GetById(contactTagId);
                if (myItem == null || myItem.ContactTagId < 1)
                {
                    return new ContactTagObject();
                }
                var contactTagObject = ModelCrossMapper.Map<ContactTag, ContactTagObject>(myItem);
                if (contactTagObject == null || contactTagObject.ContactTagId < 1)
                {
                    return new ContactTagObject();
                }
                return contactTagObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ContactTagObject();
            }
        }

        public List<ContactTagObject> GetContactTagObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<ContactTag> contactTagEntityList;
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
                    return new List<ContactTagObject>();
                }
                var contactTagObjList = new List<ContactTagObject>();
                contactTagEntityList.ForEach(m =>
                {
                    var contactTagObject = ModelCrossMapper.Map<ContactTag, ContactTagObject>(m);
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
                return new List<ContactTagObject>();
            }
        }

        public List<ContactTagObject> Search(string searchCriteria)
        {
            try
            {
               var contactTagEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!contactTagEntityList.Any())
                {
                    return new List<ContactTagObject>();
                }
                var contactTagObjList = new List<ContactTagObject>();
                contactTagEntityList.ForEach(m =>
                {
                    var contactTagObject = ModelCrossMapper.Map<ContactTag, ContactTagObject>(m);
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
                return new List<ContactTagObject>();
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

        public int GetObjectCount(Expression<Func<ContactTag, bool>> predicate)
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

        public List<ContactTagObject> GetContactTags()
        {
            try
            {
                var contactTagEntityList = _repository.GetAll().ToList();
                if (!contactTagEntityList.Any())
                {
                    return new List<ContactTagObject>();
                }
                var contactTagObjList = new List<ContactTagObject>();
                contactTagEntityList.ForEach(m =>
                {
                    var contactTagObject = ModelCrossMapper.Map<ContactTag, ContactTagObject>(m);
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
