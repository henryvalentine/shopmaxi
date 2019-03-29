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
    public class ShoppingCartRepository
    {
        private readonly IShopkeeperRepository<ShoppingCart> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _dbStoreEntities = new ShopKeeperStoreEntities();

       public ShoppingCartRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _dbStoreEntities = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_dbStoreEntities);
           _repository = new ShopkeeperRepository<ShoppingCart>(_uoWork);
		}
       
        public long AddShoppingCart(ShoppingCartObject shoppingCart)
        {
            try
            {
                if (shoppingCart == null)
                {
                    return -2;
                }
                
                var shoppingCartEntity = ModelCrossMapper.Map<ShoppingCartObject, ShoppingCart>(shoppingCart);
                if (shoppingCartEntity == null || shoppingCartEntity.CustomerId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(shoppingCartEntity);
                _uoWork.SaveChanges();
                return returnStatus.ShoppingCartId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateShoppingCart(ShoppingCartObject shoppingCart)
        {
            try
            {
                if (shoppingCart == null)
                {
                    return -2;
                }
                var shoppingCartEntity = ModelCrossMapper.Map<ShoppingCartObject, ShoppingCart>(shoppingCart);
                if (shoppingCartEntity == null || shoppingCartEntity.ShoppingCartId < 1)
                {
                    return -2;
                }
                _repository.Update(shoppingCartEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteShoppingCart(long shoppingCartId)
        {
            try
            {
                var returnStatus = _repository.Remove(shoppingCartId);
                _uoWork.SaveChanges();
                return returnStatus.ShoppingCartId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public ShoppingCartObject GetShoppingCart(long shoppingCartId)
        {
            try
            {
                using (var db = _dbStoreEntities)
                {
                    var item = from sh in db.ShoppingCarts
                        where sh.ShoppingCartId == shoppingCartId
                        join cs in db.Customers on sh.CustomerId equals cs.CustomerId
                               join ps in db.UserProfiles on cs.UserId equals ps.Id
                        join si in db.ShopingCartItems on sh.ShoppingCartId equals si.ShopingCartId
                        join st in db.StoreItemStocks on si.ShopingCartItemId equals st.StoreItemStockId
                        join sti in db.StoreItems on st.StoreItemId equals sti.StoreItemId
                        join sp in db.StoreItemTypes on sti.StoreItemTypeId equals sp.StoreItemTypeId
                        join sb in db.StoreItemBrands on sti.StoreItemBrandId equals sb.StoreItemBrandId
                        join sc in db.StoreItemCategories on sti.StoreItemCategoryId equals sc.StoreItemCategoryId
                        join um in db.UnitsOfMeasurements on si.UoMId equals um.UnitOfMeasurementId
                        select new{sh,cs,ps,si,st,sti,sp,sb,sc,um};
                   
                }
                var myItem = _repository.Get(m => m.ShoppingCartId == shoppingCartId, "Customer, ShopingCartItems");
                if (myItem == null || myItem.ShoppingCartId < 1)
                {
                    return new ShoppingCartObject();
                }
                var shoppingCartObject = ModelCrossMapper.Map<ShoppingCart, ShoppingCartObject>(myItem);
                if (shoppingCartObject == null || shoppingCartObject.ShoppingCartId < 1)
                {
                    return new ShoppingCartObject();
                }
                
                return shoppingCartObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ShoppingCartObject();
            }
        }

        public List<ShoppingCartObject> GetShoppingCartObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<ShoppingCart> shoppingCartEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    shoppingCartEntityList = _repository.GetWithPaging(m => m.ShoppingCartId, tpageNumber, tsize, "Customer, ShopingCartItems").ToList();
                }

                else
                {
                    shoppingCartEntityList = _repository.GetAll().ToList();
                }

                if (!shoppingCartEntityList.Any())
                {
                    return new List<ShoppingCartObject>();
                }
                var shoppingCartObjList = new List<ShoppingCartObject>();
                shoppingCartEntityList.ForEach(m =>
                {
                    var shoppingCartObject = ModelCrossMapper.Map<ShoppingCart, ShoppingCartObject>(m);
                    if (shoppingCartObject != null && shoppingCartObject.ShoppingCartId > 0)
                    {
                        shoppingCartObjList.Add(shoppingCartObject);
                    }
                });

                return shoppingCartObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ShoppingCartObject>();
            }
        }

        //public List<ShoppingCartObject> Search(string searchCriteria)
        //{
        //    try
        //    {
        //       var shoppingCartEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()),"Customer, ShopingCartItems").ToList();

        //        if (!shoppingCartEntityList.Any())
        //        {
        //            return new List<ShoppingCartObject>();
        //        }
        //        var shoppingCartObjList = new List<ShoppingCartObject>();
        //        shoppingCartEntityList.ForEach(m =>
        //        {
        //            var shoppingCartObject = ModelCrossMapper.Map<ShoppingCart, ShoppingCartObject>(m);
        //            if (shoppingCartObject != null && shoppingCartObject.ShoppingCartId > 0)
        //            {
        //                shoppingCartObjList.Add(shoppingCartObject);
        //            }
        //        });
        //        return shoppingCartObjList;
        //    }

        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<ShoppingCartObject>();
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

        public int GetObjectCount(Expression<Func<ShoppingCart, bool>> predicate)
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

        public List<ShoppingCartObject> GetShoppingCarts()
        {
            try
            {
                var shoppingCartEntityList = _repository.GetAll().ToList();
                if (!shoppingCartEntityList.Any())
                {
                    return new List<ShoppingCartObject>();
                }
                var shoppingCartObjList = new List<ShoppingCartObject>();
                shoppingCartEntityList.ForEach(m =>
                {
                    var shoppingCartObject = ModelCrossMapper.Map<ShoppingCart, ShoppingCartObject>(m);
                    if (shoppingCartObject != null && shoppingCartObject.ShoppingCartId > 0)
                    {
                        shoppingCartObjList.Add(shoppingCartObject);
                    }
                });
                return shoppingCartObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
