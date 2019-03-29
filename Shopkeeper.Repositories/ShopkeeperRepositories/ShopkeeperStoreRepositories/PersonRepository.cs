using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class UserProfileRepository
    {
       private readonly IShopkeeperRepository<UserProfile> _repository;
       private readonly UnitOfWork _uoWork;

       public UserProfileRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<UserProfile>(_uoWork);
		}


       public long AddUserProfile(UserProfileObject person)
       {
           try
           {
               if (person == null)
               {
                   return -2;
               }
               var duplicates = _repository.Count(m => m.LastName.Trim().ToLower() == person.LastName.Trim().ToLower() && m.OtherNames.Trim().ToLower() == person.OtherNames.Trim().ToLower());
               if (duplicates > 0)
               {
                   return -3;
               }

               var personEntity = ModelCrossMapper.Map<UserProfileObject, UserProfile>(person);

               if (personEntity == null)
               {
                   return -2;
               }
               var returnStatus = _repository.Add(personEntity);
               _uoWork.SaveChanges();
               return returnStatus.Id;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return 0;
           }
       }

       public int UpdateUserProfile(UserProfileObject person)
       {
           try
           {
               if (person == null)
               {
                   return -2;
               }
               var duplicates = _repository.Count(m => m.LastName.Trim().ToLower() == person.LastName.Trim().ToLower() && m.OtherNames.Trim().ToLower() == person.OtherNames.Trim().ToLower() && (m.Id != person.Id));
               if (duplicates > 0)
               {
                   return -3;
               }
               var personEntity = ModelCrossMapper.Map<UserProfileObject, UserProfile>(person);
               if (personEntity == null)
               {
                   return -2;
               }
               _repository.Update(personEntity);
               _uoWork.SaveChanges();
               return 5;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return -2;
           }
       }

       public bool DeleteUserProfile(long personId)
       {
           try
           {
               var returnStatus = _repository.Remove(personId);
               _uoWork.SaveChanges();
               return returnStatus.Id > 0;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return false;
           }
       }

       public UserProfileObject GetUserProfile(long personId)
       {
           try
           {
               var myItems = _repository.GetAll(m => m.Id == personId, "Salutation").ToList();
               if (!myItems.Any())
               {
                   return new UserProfileObject();
               }
               var myItem = myItems[0];
               var personObject = ModelCrossMapper.Map<UserProfile, UserProfileObject>(myItem);
               if (personObject == null || personObject.Id < 1)
               {
                   return new UserProfileObject();
               }
               if (personObject.Birthday != null)
               {
                   personObject.BirthdayStr = ((DateTime)personObject.Birthday).ToString("dd/MM/yyyy");
               }
               //personObject.Salutation = myItem.Salutation.Name;
               return personObject;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new UserProfileObject();
           }
       }

       public List<UserProfileObject> GetUserProfileObjects(int? itemsPerPage, int? pageNumber)
       {
           try
           {
               List<UserProfile> personEntityList;
               if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
               {
                   var tpageNumber = (int)pageNumber;
                   var tsize = (int)itemsPerPage;
                   personEntityList = _repository.GetWithPaging(m => m.Id, tpageNumber, tsize, "Salutation").ToList();
               }

               else
               {
                   personEntityList = _repository.GetAll("Salutation").ToList();
               }

               if (!personEntityList.Any())
               {
                   return new List<UserProfileObject>();
               }
               var personObjList = new List<UserProfileObject>();
               personEntityList.ForEach(m =>
               {
                   var personObject = ModelCrossMapper.Map<UserProfile, UserProfileObject>(m);
                   if (personObject != null && personObject.Id > 0)
                   {
                       if (personObject.Birthday != null)
                       {
                           //personObject.Salutation = m.Salutation.Name;
                           personObject.BirthdayStr = ((DateTime)personObject.Birthday).ToString("dd/MM/yyyy");
                       }
                       personObjList.Add(personObject);
                   }
               });

               return personObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<UserProfileObject>();
           }
       }

       public List<UserProfileObject> Search(string searchCriteria)
       {
           try
           {
               var personEntityList = _repository.GetAll(m => m.LastName.ToLower().Contains(searchCriteria.ToLower()) && m.OtherNames.Contains(searchCriteria), "Salutation").ToList();

               if (!personEntityList.Any())
               {
                   return new List<UserProfileObject>();
               }
               var personObjList = new List<UserProfileObject>();
               personEntityList.ForEach(m =>
               {
                   var personObject = ModelCrossMapper.Map<UserProfile, UserProfileObject>(m);
                   if (personObject != null && personObject.Id > 0)
                   {
                       if (personObject.Birthday != null)
                       {
                           //personObject.Salutation = m.Salutation.Name;
                           personObject.BirthdayStr = ((DateTime)personObject.Birthday).ToString("dd/MM/yyyy");
                       }
                       personObjList.Add(personObject);
                   }
               });
               return personObjList;
           }

           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new List<UserProfileObject>();
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

       public int GetObjectCount(Expression<Func<UserProfile, bool>> predicate)
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

       public List<UserProfileObject> GetUserProfiles()
       {
           try
           {
               var personEntityList = _repository.GetAll().ToList();
               if (!personEntityList.Any())
               {
                   return new List<UserProfileObject>();
               }
               var personObjList = new List<UserProfileObject>();
               personEntityList.ForEach(m =>
               {
                   var personObject = ModelCrossMapper.Map<UserProfile, UserProfileObject>(m);
                   if (personObject != null && personObject.Id > 0)
                   {
                       personObject.LastName = personObject.LastName + " " + personObject.OtherNames;
                       personObjList.Add(personObject);
                   }
               });
               return personObjList;
           }
           catch (Exception ex)
           {
               ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return null;
           }
       }
       
    }
}
