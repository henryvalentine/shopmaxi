using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using ImportPermitPortal.DataObjects.Helpers;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class AccountGroupRepository
    {
        private readonly IShopkeeperRepository<AccountGroup> _repository;
        private readonly UnitOfWork _uoWork;

       public AccountGroupRepository()
        {
           var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<AccountGroup>(_uoWork);
		}
       
        public long AddAccountGroup(AccountGroupObject accountGroup)
        {
            try
            {
                if (accountGroup == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == accountGroup.Name.Trim().ToLower());
                if (duplicates > 0)
                {
                    return -3;
                }
                var accountGroupEntity = ModelCrossMapper.Map<AccountGroupObject, AccountGroup>(accountGroup);
                if (accountGroupEntity == null || string.IsNullOrEmpty(accountGroupEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(accountGroupEntity);
                _uoWork.SaveChanges();
                return returnStatus.AccountGroupId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateAccountGroup(AccountGroupObject accountGroup)
        {
            try
            {
                if (accountGroup == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == accountGroup.Name.Trim().ToLower() && (m.AccountGroupId != accountGroup.AccountGroupId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var accountGroupEntity = ModelCrossMapper.Map<AccountGroupObject, AccountGroup>(accountGroup);
                if (accountGroupEntity == null || accountGroupEntity.AccountGroupId < 1)
                {
                    return -2;
                }
                _repository.Update(accountGroupEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteAccountGroup(long accountGroupId)
        {
            try
            {
                _repository.Remove(accountGroupId);
                _uoWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public AccountGroupObject GetAccountGroup(long accountGroupId)
        {
            try
            {
                var myItem = _repository.GetById(accountGroupId);
                if (myItem == null || myItem.AccountGroupId < 1)
                {
                    return new AccountGroupObject();
                }
                var accountGroupObject = ModelCrossMapper.Map<AccountGroup, AccountGroupObject>(myItem);
                if (accountGroupObject == null || accountGroupObject.AccountGroupId < 1)
                {
                    return new AccountGroupObject();
                }
                return accountGroupObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new AccountGroupObject();
            }
        }

        public List<AccountGroupObject> GetAccountGroupObject(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<AccountGroup> accountGroupEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    accountGroupEntityList = _repository.GetWithPaging(m => m.AccountGroupId, tpageNumber, tsize).ToList();
                }

                else
                {
                    accountGroupEntityList = _repository.GetAll().ToList();
                }

                if (!accountGroupEntityList.Any())
                {
                    return new List<AccountGroupObject>();
                }
                var accountGroupObjList = new List<AccountGroupObject>();
                accountGroupEntityList.ForEach(m =>
                {
                    var accountGroupObject = ModelCrossMapper.Map<AccountGroup, AccountGroupObject>(m);
                    if (accountGroupObject != null && accountGroupObject.AccountGroupId > 0)
                    {
                        accountGroupObjList.Add(accountGroupObject);
                    }
                });

                return accountGroupObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AccountGroupObject>();
            }
        }

        public List<AccountGroupObject> Search(string searchCriteria)
        {
            try
            {
                var accountGroupEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!accountGroupEntityList.Any())
                {
                    return new List<AccountGroupObject>();
                }
                var accountGroupObjList = new List<AccountGroupObject>();
                accountGroupEntityList.ForEach(m =>
                {
                    var accountGroupObject = ModelCrossMapper.Map<AccountGroup, AccountGroupObject>(m);
                    if (accountGroupObject != null && accountGroupObject.AccountGroupId > 0)
                    {
                        accountGroupObjList.Add(accountGroupObject);
                    }
                });
                return accountGroupObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<AccountGroupObject>();
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

        public int GetObjectCount(Expression<Func<AccountGroup, bool>> predicate)
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

        public List<AccountGroupObject> GetAccountGroups()
        {
            try
            {
                var accountGroupEntityList = _repository.GetAll().ToList();
                if (!accountGroupEntityList.Any())
                {
                    return new List<AccountGroupObject>();
                }
                var accountGroupObjList = new List<AccountGroupObject>();
                accountGroupEntityList.ForEach(m =>
                {
                    var accountGroupObject = ModelCrossMapper.Map<AccountGroup, AccountGroupObject>(m);
                    if (accountGroupObject != null && accountGroupObject.AccountGroupId > 0)
                    {
                        accountGroupObjList.Add(accountGroupObject);
                    }
                });
                return accountGroupObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
