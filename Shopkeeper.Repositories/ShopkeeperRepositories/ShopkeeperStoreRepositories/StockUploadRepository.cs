using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
    public class StockUploadRepository
    {
       private readonly IShopkeeperRepository<StockUpload> _repository;
       private readonly UnitOfWork _uoWork;
       private readonly ShopKeeperStoreEntities _db;
       public StockUploadRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            _db = new ShopKeeperStoreEntities(connectionString);
            _uoWork = new UnitOfWork(_db);
           _repository = new ShopkeeperRepository<StockUpload>(_uoWork);
		}
       
        public long AddStockUpload(StockUploadObject stockUpload)
        {
            try
            {
                if (stockUpload == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.StoreItemStockId == stockUpload.StoreItemStockId && stockUpload.ImageViewId == m.ImageViewId);
                if (duplicates > 0)
                {
                    return -3;
                }
                var stockUploadEntity = ModelCrossMapper.Map<StockUploadObject, StockUpload>(stockUpload);
                if (stockUploadEntity == null || stockUploadEntity.StoreItemStockId < 1)
                {
                    return -2;
                }
                var returnStatus = _repository.Add(stockUploadEntity);
                _uoWork.SaveChanges();
                return returnStatus.StockUploadId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateStockUpload(StockUploadObject stockUpload)
        {
            try
            {
                if (stockUpload == null)
                {
                    return -2;
                }
                using (var db = _db)
                {
                    var duplicates = db.StockUploads.Count(m => m.StoreItemStockId == stockUpload.StoreItemStockId && stockUpload.ImageViewId == m.ImageViewId && stockUpload.StockUploadId != m.StockUploadId);
                    if (duplicates > 0)
                    {
                        return -3;
                    }
                    var items = db.StockUploads.Where(s => s.StockUploadId == stockUpload.StockUploadId).ToList();
                    if (!items.Any())
                    {
                        return -2;
                    }

                    var item = items[0];
                    item.ImageViewId = stockUpload.ImageViewId;
                    item.ImagePath = stockUpload.ImagePath;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();

                    return 5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteStockUpload(long stockUploadId)
        {
            try
            {
                var returnStatus = _repository.Remove(stockUploadId);
                _uoWork.SaveChanges();
                return returnStatus.StockUploadId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

       public bool DeleteStockUploadByStorItemId(long storeItemStockId, out List<string> filePathList)
        {
            try
            {
                var returnStatus = _repository.RemoveWithResultList(m => m.StoreItemStockId == storeItemStockId);
                _uoWork.SaveChanges();
                var output = new List<string>();
                returnStatus.ForEach(x => output.Add(x.ImagePath));
                filePathList = output;
                return returnStatus.Any();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                filePathList = new List<string>();
                return false;
            }
        }
        public StockUploadObject GetStockUpload(long stockUploadId)
        {
            try
            {
                var myItem = _repository.GetById(stockUploadId);
                if (myItem == null || myItem.StockUploadId < 1)
                {
                    return new StockUploadObject();
                }
                var stockUploadObject = ModelCrossMapper.Map<StockUpload, StockUploadObject>(myItem);
                if (stockUploadObject == null || stockUploadObject.StockUploadId < 1)
                {
                    return new StockUploadObject();
                }
                return stockUploadObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StockUploadObject();
            }
        }

        public List<StockUploadObject> GetStockUploadsByStockItem(long storeItemStockId)
        {
            try
            {
                var stockUploadEntityList = _repository.GetAll(m => m.StoreItemStockId == storeItemStockId, "ImageView").ToList();
                
                if (!stockUploadEntityList.Any())
                {
                    return new List<StockUploadObject>();
                }
                var stockUploadObjList = new List<StockUploadObject>();
                stockUploadEntityList.ForEach(m =>
                {
                    var stockUploadObject = ModelCrossMapper.Map<StockUpload, StockUploadObject>(m);
                    if (stockUploadObject != null && stockUploadObject.StockUploadId > 0)
                    {
                        stockUploadObject.ViewName = m.ImageView.Name;;
                        stockUploadObjList.Add(stockUploadObject);
                    }
                });

                return stockUploadObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StockUploadObject>();
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

        public int GetObjectCount(Expression<Func<StockUpload, bool>> predicate)
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
        public List<StockUploadObject> GetStockUploads()
        {
            try
            {
                var stockUploadEntityList = _repository.GetAll().ToList();
                if (!stockUploadEntityList.Any())
                {
                    return new List<StockUploadObject>();
                }
                var stockUploadObjList = new List<StockUploadObject>();
                stockUploadEntityList.ForEach(m =>
                {
                    var stockUploadObject = ModelCrossMapper.Map<StockUpload, StockUploadObject>(m);
                    if (stockUploadObject != null && stockUploadObject.StockUploadId > 0)
                    {
                        stockUploadObjList.Add(stockUploadObject);
                    }
                });
                return stockUploadObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
