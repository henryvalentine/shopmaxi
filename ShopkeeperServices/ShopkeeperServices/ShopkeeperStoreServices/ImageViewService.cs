using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class ImageViewServices
	{
        private readonly ImageViewRepository _imageViewRepository;
        public ImageViewServices()
		{
            _imageViewRepository = new ImageViewRepository();
		}

        public long AddImageView(ImageViewObject imageViewAccount)
		{
			try
			{
                return _imageViewRepository.AddImageView(imageViewAccount);
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
                return _imageViewRepository.UpdateImageView(imageView);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteImageView(long imageViewAccountId)
		{
			try
			{
                return _imageViewRepository.DeleteImageView(imageViewAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public ImageViewObject GetImageView(long imageViewAccountId)
		{
			try
			{
                return _imageViewRepository.GetImageView(imageViewAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new ImageViewObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _imageViewRepository.GetObjectCount();
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
                var objList = _imageViewRepository.GetImageViews();
                if (objList == null || !objList.Any())
			    {
                    return new List<ImageViewObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ImageViewObject>();
			}
		}

        public List<ImageViewObject> GetImageViewObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _imageViewRepository.GetImageViewObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<ImageViewObject>();
                }
                return objList;
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
                var objList = _imageViewRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<ImageViewObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ImageViewObject>();
            }
        }
	}

}
