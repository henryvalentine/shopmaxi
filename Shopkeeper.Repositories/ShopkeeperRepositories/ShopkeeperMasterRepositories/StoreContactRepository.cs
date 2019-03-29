using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class StoreContactRepository
    {
       private readonly IShopkeeperRepository<StoreContact> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreContactRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<StoreContact>(_uoWork);
		}
       
        public long AddStoreContact(StoreContactObject storeContact)
        {
            try
            {
                if (storeContact == null)
                {
                    return -2;
                }
               
                var storeContactEntity = ModelCrossMapper.Map<StoreContactObject, StoreContact>(storeContact);
                if (storeContactEntity == null || storeContactEntity.StoreContactId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(storeContactEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreContactId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Int32 UpdateStoreContact(StoreContactObject storeContact)
        {
            try
            {
                if (storeContact == null)
                {
                    return -2;
                }
                var storeContactEntity = ModelCrossMapper.Map<StoreContactObject, StoreContact>(storeContact);
                if (storeContactEntity == null || storeContactEntity.StoreContactId < 1)
                {
                    return -2;
                }
                _repository.Update(storeContactEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreContact(long storeContactId)
        {
            try
            {
                var returnStatus = _repository.Remove(storeContactId);
                _uoWork.SaveChanges();
                return returnStatus.StoreContactId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreContactObject GetStoreContact(long storeContactId)
        {
            try
            {
                var myItem = _repository.Get(m => m.StoreContactId == storeContactId, "Person, ContactTag");
                if (myItem == null || myItem.StoreContactId < 1)
                {
                    return new StoreContactObject();
                }
                var storeContactObject = ModelCrossMapper.Map<StoreContact, StoreContactObject>(myItem);
                if (storeContactObject == null || storeContactObject.StoreContactId < 1)
                {
                    return new StoreContactObject();
                }
                var personObj = ModelCrossMapper.Map<Person, PersonObject>(myItem.Person);
                var contactObj = ModelCrossMapper.Map<ContactTag, ContactTagObject>(myItem.ContactTag);
                if (personObj != null && personObj.PersonId > 0 && contactObj != null && contactObj.ContactTagId > 0)
                {
                    storeContactObject.PersonObject = personObj;
                    storeContactObject.ContactTagObject = contactObj;
                    return storeContactObject;
                }
                
                return new StoreContactObject();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreContactObject();
            }
        }

        public List<StoreContactObject> GetStoreContactObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                    List<StoreContact> storeContactEntityList;
                    if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                    {
                        var tpageNumber = (int)pageNumber;
                        var tsize = (int)itemsPerPage;
                        storeContactEntityList = _repository.GetWithPaging(m => m.StoreContactId, tpageNumber, tsize, "PersonObject, ContactTagObject").ToList();
                    }

                    else
                    {
                        storeContactEntityList = _repository.GetAll("PersonObject, ContactTagObject").ToList();
                    }

                    if (!storeContactEntityList.Any())
                    {
                        return new List<StoreContactObject>();
                    }
                    var storeContactObjList = new List<StoreContactObject>();
                    storeContactEntityList.ForEach(m =>
                    {
                        var storeContactObject = ModelCrossMapper.Map<StoreContact, StoreContactObject>(m);
                        if (storeContactObject != null && storeContactObject.StoreContactId > 0)
                        {
                            var personObj = ModelCrossMapper.Map<Person, PersonObject>(m.Person);
                            var contactObj = ModelCrossMapper.Map<ContactTag, ContactTagObject>(m.ContactTag);
                            if (personObj != null && personObj.PersonId > 0 && contactObj != null && contactObj.ContactTagId > 0)
                            {
                                storeContactObject.PersonObject = new PersonObject();
                                storeContactObject.PersonObject = personObj;
                                storeContactObject.ContactTagObject = new ContactTagObject();
                                storeContactObject.ContactTagObject = contactObj;
                                storeContactObjList.Add(storeContactObject);
                            }
                            else
                            {
                                storeContactObjList.Add(storeContactObject);
                            }
                        }
                    });

                return storeContactObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactObject>();
            }
        }

        public List<StoreContactObject> GetStoreContactsByDepartment(int? itemsPerPage, int? pageNumber, int departmentId)
        {
            try
            {
                List<StoreContact> storeContactEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    storeContactEntityList = _repository.GetWithPaging(m => m.StoreContactId == departmentId, m => m.StoreContactId, tpageNumber, tsize, "PersonObject, ContactTagObject").ToList();
                }

                else
                {
                    storeContactEntityList = _repository.GetAll(m => m.StoreContactId == departmentId, "PersonObject, ContactTagObject").ToList();
                }

                if (!storeContactEntityList.Any())
                {
                    return new List<StoreContactObject>();
                }
                var storeContactObjList = new List<StoreContactObject>();
                storeContactEntityList.ForEach(m =>
                {
                    var storeContactObject = ModelCrossMapper.Map<StoreContact, StoreContactObject>(m);
                    if (storeContactObject != null && storeContactObject.StoreContactId > 0)
                    {
                        var personObj = ModelCrossMapper.Map<Person, PersonObject>(m.Person);
                        var contactObj = ModelCrossMapper.Map<ContactTag, ContactTagObject>(m.ContactTag);
                        if (personObj != null && personObj.PersonId > 0 && contactObj != null && contactObj.ContactTagId > 0)
                        {
                            storeContactObject.PersonObject = new PersonObject();
                            storeContactObject.PersonObject = personObj;
                            storeContactObject.ContactTagObject = new ContactTagObject();
                            storeContactObject.ContactTagObject = contactObj;
                            storeContactObjList.Add(storeContactObject);
                        }
                        else
                        {
                            storeContactObjList.Add(storeContactObject);
                        }
                    }
                });

                return storeContactObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreContactObject>();
            }
        }

        //public List<StoreContactObject> GetStoreContactsByStore(int? itemsPerPage, int? pageNumber, long storeId)
        //{
        //    try
        //    {
        //        List<StoreContact> storeContactEntityList;
        //        if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
        //        {
        //            var tpageNumber = (int)pageNumber;
        //            var tsize = (int)itemsPerPage;
        //            storeContactEntityList = _repository.GetWithPaging(m => m.id == storeId, m => m.StoreContactId, tpageNumber, tsize, "PersonObject, ContactTagObject").ToList();
        //        }

        //        else
        //        {
        //            storeContactEntityList = _repository.GetAll(m => m.CompanyId == storeId, "PersonObject, ContactTagObject").ToList();
        //        }

        //        if (!storeContactEntityList.Any())
        //        {
        //            return new List<StoreContactObject>();
        //        }
        //        var storeContactObjList = new List<StoreContactObject>();
        //        storeContactEntityList.ForEach(m =>
        //        {
        //            var storeContactObject = ModelCrossMapper.Map<StoreContact, StoreContactObject>(m);
        //            if (storeContactObject != null && storeContactObject.StoreContactId > 0)
        //            {
        //                var personObj = ModelCrossMapper.Map<Person, PersonObject>(m.Person);
        //                var contactObj = ModelCrossMapper.Map<ContactTag, ContactTagObject>(m.ContactTag);
        //                if (personObj != null && personObj.Id > 0 && contactObj != null && contactObj.ContactTagId > 0)
        //                {
        //                    storeContactObject.PersonObject = new PersonObject();
        //                    storeContactObject.PersonObject = personObj;
        //                    storeContactObject.ContactTagObject = new ContactTagObject();
        //                    storeContactObject.ContactTagObject = contactObj;
        //                    storeContactObjList.Add(storeContactObject);
        //                }
        //                else
        //                {
        //                    storeContactObjList.Add(storeContactObject);
        //                }
        //            }
        //        });

        //        return storeContactObjList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<StoreContactObject>();
        //    }
        //}
        
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

        public List<StoreContactObject> GetStoreContacts()
        {
            try
            {
                var storeContactEntityList = _repository.GetAll().ToList();
                if (!storeContactEntityList.Any())
                {
                    return new List<StoreContactObject>();
                }
                var storeContactObjList = new List<StoreContactObject>();
                storeContactEntityList.ForEach(m =>
                {
                    var storeContactObject = ModelCrossMapper.Map<StoreContact, StoreContactObject>(m);
                    if (storeContactObject != null && storeContactObject.StoreContactId > 0)
                    {
                        storeContactObjList.Add(storeContactObject);
                    }
                });
                return storeContactObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
