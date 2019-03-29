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
    public class StoreCurrencyRepository
    {
        private readonly IShopkeeperRepository<StoreCurrency> _repository;
       private readonly UnitOfWork _uoWork;

       public StoreCurrencyRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<StoreCurrency>(_uoWork);
		}
       
        public long AddStoreCurrency(StoreCurrencyObject currency)
        {
            try
            {
                if (currency == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(currency.Name.Trim().ToLower()) && currency.StoreCountryId.Equals(m.StoreCountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var currencyEntity = ModelCrossMapper.Map<StoreCurrencyObject, StoreCurrency>(currency);
                if (currencyEntity == null || string.IsNullOrEmpty(currencyEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(currencyEntity);
                _uoWork.SaveChanges();
                return returnStatus.StoreCurrencyId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStoreCurrency(StoreCurrencyObject currency)
        {
            try
            {
                if (currency == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(currency.Name.Trim().ToLower()) && (m.StoreCountryId == currency.StoreCountryId) && (m.StoreCurrencyId != currency.StoreCurrencyId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var currencyEntity = ModelCrossMapper.Map<StoreCurrencyObject, StoreCurrency>(currency);
                if (currencyEntity == null || currencyEntity.StoreCurrencyId < 1)
                {
                    return -2;
                }
                _repository.Update(currencyEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStoreCurrency(long currencyId)
        {
            try
            {
                var returnStatus = _repository.Remove(currencyId);
                _uoWork.SaveChanges();
                return returnStatus.StoreCurrencyId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public StoreCurrencyObject GetStoreCurrency(long currencyId)
        {
            try
            {
                var myItem = _repository.GetById(currencyId);
                if (myItem == null || myItem.StoreCurrencyId < 1)
                {
                    return new StoreCurrencyObject();
                }
                var currencyObject = ModelCrossMapper.Map<StoreCurrency, StoreCurrencyObject>(myItem);
                if (currencyObject == null || currencyObject.StoreCurrencyId < 1)
                {
                    return new StoreCurrencyObject();
                }
                return currencyObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCurrencyObject();
            }
        }

        public StoreCurrencyObject GetStoreCurrency()
        {
            try
            {
                var myItems = _repository.GetAll().ToList();
                if (!myItems.Any())
                {
                    return new StoreCurrencyObject();
                }
                var currencyObject = ModelCrossMapper.Map<StoreCurrency, StoreCurrencyObject>(myItems[0]);
                if (currencyObject == null || currencyObject.StoreCurrencyId < 1)
                {
                    return new StoreCurrencyObject();
                }
                return currencyObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreCurrencyObject();
            }
        }

        public List<StoreCurrencyObject> GetStoreCurrencyObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<StoreCurrency> currencyEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    currencyEntityList = _repository.GetWithPaging(m => m.StoreCurrencyId, tpageNumber, tsize, "StoreCountry").ToList();
                }

                else
                {
                    currencyEntityList = _repository.GetAll("StoreCountry").ToList();
                }

                if (!currencyEntityList.Any())
                {
                    return new List<StoreCurrencyObject>();
                }
                var currencyObjList = new List<StoreCurrencyObject>();
                currencyEntityList.ForEach(m =>
                {
                    var currencyObject = ModelCrossMapper.Map<StoreCurrency, StoreCurrencyObject>(m);
                    if (currencyObject != null && currencyObject.StoreCurrencyId > 0)
                    {
                        currencyObject.CountryName = m.StoreCountry.Name;
                        currencyObjList.Add(currencyObject);
                    }
                });

                return currencyObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCurrencyObject>();
            }
        }

        public List<StoreCurrencyObject> Search(string searchCriteria)
        {
            try
            {
               var currencyEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()) || m.Symbol.Contains(searchCriteria), "StoreCountry").ToList();

                if (!currencyEntityList.Any())
                {
                    return new List<StoreCurrencyObject>();
                }
                var currencyObjList = new List<StoreCurrencyObject>();
                currencyEntityList.ForEach(m =>
                {
                    var currencyObject = ModelCrossMapper.Map<StoreCurrency, StoreCurrencyObject>(m);
                    if (currencyObject != null && currencyObject.StoreCurrencyId > 0)
                    {
                        currencyObject.CountryName = m.StoreCountry.Name;
                        currencyObjList.Add(currencyObject);
                    }
                });
                return currencyObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreCurrencyObject>();
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

        public int GetObjectCount(Expression<Func<StoreCurrency, bool>> predicate)
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

        public List<StoreCurrencyObject> GetCurrencies()
        {
            try
            {
                var currencyEntityList = _repository.GetAll().ToList();
                if (!currencyEntityList.Any())
                {
                    return new List<StoreCurrencyObject>();
                }
                var currencyObjList = new List<StoreCurrencyObject>();
                currencyEntityList.ForEach(m =>
                {
                    var currencyObject = ModelCrossMapper.Map<StoreCurrency, StoreCurrencyObject>(m);
                    if (currencyObject != null && currencyObject.StoreCurrencyId > 0)
                    {
                        currencyObjList.Add(currencyObject);
                    }
                });
                return currencyObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
