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
    public class CurrencyRepository
    {
       private readonly IShopkeeperRepository<Currency> _repository;
       private readonly UnitOfWork _uoWork;

       public CurrencyRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<Currency>(_uoWork);
		}
       
        public long AddCurrency(CurrencyObject currency)
        {
            try
            {
                if (currency == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(currency.Name.Trim().ToLower()) && currency.CountryId.Equals(m.CountryId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var currencyEntity = ModelCrossMapper.Map<CurrencyObject, Currency>(currency);
                if (currencyEntity == null || string.IsNullOrEmpty(currencyEntity.Name))
                {
                    return -2;
                }
                _repository.Add(currencyEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateCurrency(CurrencyObject currency)
        {
            try
            {
                if (currency == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(currency.Name.Trim().ToLower()) && (m.CountryId == currency.CountryId) && (m.CurrencyId != currency.CurrencyId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var currencyEntity = ModelCrossMapper.Map<CurrencyObject, Currency>(currency);
                if (currencyEntity == null || currencyEntity.CurrencyId < 1)
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

        public bool DeleteCurrency(long currencyId)
        {
            try
            {
                var returnStatus = _repository.Remove(currencyId);
                _uoWork.SaveChanges();
                return returnStatus.CurrencyId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public CurrencyObject GetCurrency(long currencyId)
        {
            try
            {
                var myItem = _repository.GetById(currencyId);
                if (myItem == null || myItem.CurrencyId < 1)
                {
                    return new CurrencyObject();
                }
                var currencyObject = ModelCrossMapper.Map<Currency, CurrencyObject>(myItem);
                if (currencyObject == null || currencyObject.CurrencyId < 1)
                {
                    return new CurrencyObject();
                }
                return currencyObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CurrencyObject();
            }
        }

        public List<CurrencyObject> GetCurrencyObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<Currency> currencyEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    currencyEntityList = _repository.GetWithPaging(m => m.CurrencyId, tpageNumber, tsize, "Country").ToList();
                }

                else
                {
                    currencyEntityList = _repository.GetAll("Country").ToList();
                }

                if (!currencyEntityList.Any())
                {
                    return new List<CurrencyObject>();
                }
                var currencyObjList = new List<CurrencyObject>();
                currencyEntityList.ForEach(m =>
                {
                    var currencyObject = ModelCrossMapper.Map<Currency, CurrencyObject>(m);
                    if (currencyObject != null && currencyObject.CurrencyId > 0)
                    {
                        currencyObject.CountryName = m.Country.Name;
                        currencyObjList.Add(currencyObject);
                    }
                });

                return currencyObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CurrencyObject>();
            }
        }

        public List<CurrencyObject> Search(string searchCriteria)
        {
            try
            {
               var currencyEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()) || m.Symbol.Contains(searchCriteria), "Country").ToList();

                if (!currencyEntityList.Any())
                {
                    return new List<CurrencyObject>();
                }
                var currencyObjList = new List<CurrencyObject>();
                currencyEntityList.ForEach(m =>
                {
                    var currencyObject = ModelCrossMapper.Map<Currency, CurrencyObject>(m);
                    if (currencyObject != null && currencyObject.CurrencyId > 0)
                    {
                        currencyObject.CountryName = m.Country.Name;
                        currencyObjList.Add(currencyObject);
                    }
                });
                return currencyObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CurrencyObject>();
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

        public int GetObjectCount(Expression<Func<Currency, bool>> predicate)
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

        public List<CurrencyObject> GetCurrencies()
        {
            try
            {
                var currencyEntityList = _repository.GetAll().ToList();
                if (!currencyEntityList.Any())
                {
                    return new List<CurrencyObject>();
                }
                var currencyObjList = new List<CurrencyObject>();
                currencyEntityList.ForEach(m =>
                {
                    var currencyObject = ModelCrossMapper.Map<Currency, CurrencyObject>(m);
                    if (currencyObject != null && currencyObject.CurrencyId > 0)
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
