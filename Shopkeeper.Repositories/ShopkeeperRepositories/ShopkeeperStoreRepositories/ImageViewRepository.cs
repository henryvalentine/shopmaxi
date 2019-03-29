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
    public class ImageViewRepository
    {
        private readonly IShopkeeperRepository<ImageView> _repository;
       private readonly UnitOfWork _uoWork;

       public ImageViewRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<ImageView>(_uoWork);
		}
       
        public long AddImageView(ImageViewObject imageView)
        {
            try
            {
                if (imageView == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == imageView.Name.Trim().ToLower() && (m.ImageViewId != imageView.ImageViewId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var imageViewEntity = ModelCrossMapper.Map<ImageViewObject, ImageView>(imageView);
                if (imageViewEntity == null || string.IsNullOrEmpty(imageViewEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(imageViewEntity);
                _uoWork.SaveChanges();
                return returnStatus.ImageViewId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateImageView(ImageViewObject imageView)
        {
            try
            {
                if (imageView == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower() == imageView.Name.Trim().ToLower() && (m.ImageViewId != imageView.ImageViewId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var imageViewEntity = ModelCrossMapper.Map<ImageViewObject, ImageView>(imageView);
                if (imageViewEntity == null || imageViewEntity.ImageViewId < 1)
                {
                    return -2;
                }
                _repository.Update(imageViewEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteImageView(long imageViewId)
        {
            try
            {
                var returnStatus = _repository.Remove(imageViewId);
                _uoWork.SaveChanges();
                return returnStatus.ImageViewId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public ImageViewObject GetImageView(long imageViewId)
        {
            try
            {
                var myItem = _repository.GetById(imageViewId);
                if (myItem == null || myItem.ImageViewId < 1)
                {
                    return new ImageViewObject();
                }
                var imageViewObject = ModelCrossMapper.Map<ImageView, ImageViewObject>(myItem);
                if (imageViewObject == null || imageViewObject.ImageViewId < 1)
                {
                    return new ImageViewObject();
                }
                return imageViewObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ImageViewObject();
            }
        }

        public List<ImageViewObject> GetImageViewObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<ImageView> imageViewEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    imageViewEntityList = _repository.GetWithPaging(m => m.ImageViewId, tpageNumber, tsize).ToList();
                }

                else
                {
                    imageViewEntityList = _repository.GetAll().ToList();
                }

                if (!imageViewEntityList.Any())
                {
                    return new List<ImageViewObject>();
                }
                var imageViewObjList = new List<ImageViewObject>();
                imageViewEntityList.ForEach(m =>
                {
                    var imageViewObject = ModelCrossMapper.Map<ImageView, ImageViewObject>(m);
                    if (imageViewObject != null && imageViewObject.ImageViewId > 0)
                    {
                        imageViewObjList.Add(imageViewObject);
                    }
                });

                return imageViewObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ImageViewObject>();
            }
        }

        public List<ImageViewObject> Search(string searchCriteria)
        {
            try
            {
               var imageViewEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!imageViewEntityList.Any())
                {
                    return new List<ImageViewObject>();
                }
                var imageViewObjList = new List<ImageViewObject>();
                imageViewEntityList.ForEach(m =>
                {
                    var imageViewObject = ModelCrossMapper.Map<ImageView, ImageViewObject>(m);
                    if (imageViewObject != null && imageViewObject.ImageViewId > 0)
                    {
                        imageViewObjList.Add(imageViewObject);
                    }
                });
                return imageViewObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ImageViewObject>();
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

        public int GetObjectCount(Expression<Func<ImageView, bool>> predicate)
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

        public List<ImageViewObject> GetImageViews()
        {
            try
            {
                var imageViewEntityList = _repository.GetAll().ToList();
                if (!imageViewEntityList.Any())
                {
                    return new List<ImageViewObject>();
                }
                var imageViewObjList = new List<ImageViewObject>();
                imageViewEntityList.ForEach(m =>
                {
                    var imageViewObject = ModelCrossMapper.Map<ImageView, ImageViewObject>(m);
                    if (imageViewObject != null && imageViewObject.ImageViewId > 0)
                    {
                        imageViewObjList.Add(imageViewObject);
                    }
                });
                return imageViewObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
