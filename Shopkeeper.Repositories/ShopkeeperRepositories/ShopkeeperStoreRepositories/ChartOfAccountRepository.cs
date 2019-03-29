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
    public class ChartOfAccountRepository
    {
       private readonly IShopkeeperRepository<ChartOfAccount> _repository;
       private readonly UnitOfWork _uoWork;

       public ChartOfAccountRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<ChartOfAccount>(_uoWork);
		}
       
        public long AddChartOfAccount(ChartOfAccountObject chartOfAccount)
        {
            try
            {
                if (chartOfAccount == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.AccountType.Trim().ToLower() == chartOfAccount.AccountType.Trim().ToLower() && m.AccountCode.Trim().ToLower() == chartOfAccount.AccountCode.Trim().ToLower() && chartOfAccount.AccountGroupId == m.AccountGroupId);
                if (duplicates > 0)
                {
                    return -3;
                }
                var chartOfAccountEntity = ModelCrossMapper.Map<ChartOfAccountObject, ChartOfAccount>(chartOfAccount);
                if (chartOfAccountEntity == null || string.IsNullOrEmpty(chartOfAccountEntity.AccountType))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(chartOfAccountEntity);
                _uoWork.SaveChanges();
                return returnStatus.ChartOfAccountId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateChartOfAccount(ChartOfAccountObject chartOfAccount)
        {
            try
            {
                if (chartOfAccount == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.AccountType.Trim().ToLower() == chartOfAccount.AccountType.Trim().ToLower() && m.AccountCode.Trim().ToLower() == chartOfAccount.AccountCode.Trim().ToLower() && chartOfAccount.AccountGroupId == m.AccountGroupId && (m.ChartOfAccountId != chartOfAccount.ChartOfAccountId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var chartOfAccountEntity = ModelCrossMapper.Map<ChartOfAccountObject, ChartOfAccount>(chartOfAccount);
                if (chartOfAccountEntity == null || chartOfAccountEntity.ChartOfAccountId < 1)
                {
                    return -2;
                }
                _repository.Update(chartOfAccountEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteChartOfAccount(long chartOfAccountId)
        {
            try
            {
                var returnStatus = _repository.Remove(chartOfAccountId);
                _uoWork.SaveChanges();
                return returnStatus.ChartOfAccountId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public ChartOfAccountObject GetChartOfAccount(long chartOfAccountId)
        {
            try
            {
                var myItem = _repository.GetById(chartOfAccountId);
                if (myItem == null || myItem.ChartOfAccountId < 1)
                {
                    return new ChartOfAccountObject();
                }
                var chartOfAccountObject = ModelCrossMapper.Map<ChartOfAccount, ChartOfAccountObject>(myItem);
                if (chartOfAccountObject == null || chartOfAccountObject.ChartOfAccountId < 1)
                {
                    return new ChartOfAccountObject();
                }
                return chartOfAccountObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ChartOfAccountObject();
            }
        }

        public List<ChartOfAccountObject> GetChartOfAccountObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<ChartOfAccount> chartOfAccountEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    chartOfAccountEntityList = _repository.GetWithPaging(m => m.ChartOfAccountId, tpageNumber, tsize, "AccountGroup").ToList();
                }

                else
                {
                    chartOfAccountEntityList = _repository.GetAll("AccountGroup").ToList();
                }

                if (!chartOfAccountEntityList.Any())
                {
                    return new List<ChartOfAccountObject>();
                }
                var chartOfAccountObjList = new List<ChartOfAccountObject>();
                chartOfAccountEntityList.ForEach(m =>
                {
                    var chartOfAccountObject = ModelCrossMapper.Map<ChartOfAccount, ChartOfAccountObject>(m);
                    if (chartOfAccountObject != null && chartOfAccountObject.ChartOfAccountId > 0)
                    {
                        chartOfAccountObject.AccountGroupName = m.AccountGroup.Name;
                        chartOfAccountObjList.Add(chartOfAccountObject);
                    }
                });

                return chartOfAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChartOfAccountObject>();
            }
        }

        public List<ChartOfAccountObject> Search(string searchCriteria)
        {
            try
            {
               var chartOfAccountEntityList = _repository.GetAll(m => m.AccountType.ToLower().Contains(searchCriteria.ToLower()), "AccountGroup").ToList();

                if (!chartOfAccountEntityList.Any())
                {
                    return new List<ChartOfAccountObject>();
                }
                var chartOfAccountObjList = new List<ChartOfAccountObject>();
                chartOfAccountEntityList.ForEach(m =>
                {
                    var chartOfAccountObject = ModelCrossMapper.Map<ChartOfAccount, ChartOfAccountObject>(m);
                    if (chartOfAccountObject != null && chartOfAccountObject.ChartOfAccountId > 0)
                    {
                        chartOfAccountObject.AccountGroupName = m.AccountGroup.Name;
                        chartOfAccountObjList.Add(chartOfAccountObject);
                    }
                });
                return chartOfAccountObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ChartOfAccountObject>();
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

        public int GetObjectCount(Expression<Func<ChartOfAccount, bool>> predicate)
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

        public List<ChartOfAccountObject> GetChartsOfAccount()
        {
            try
            {
                var chartOfAccountEntityList = _repository.GetAll().ToList();
                if (!chartOfAccountEntityList.Any())
                {
                    return new List<ChartOfAccountObject>();
                }
                var chartOfAccountObjList = new List<ChartOfAccountObject>();
                chartOfAccountEntityList.ForEach(m =>
                {
                    var chartOfAccountObject = ModelCrossMapper.Map<ChartOfAccount, ChartOfAccountObject>(m);
                    if (chartOfAccountObject != null && chartOfAccountObject.ChartOfAccountId > 0)
                    {
                        chartOfAccountObjList.Add(chartOfAccountObject);
                    }
                });
                return chartOfAccountObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
