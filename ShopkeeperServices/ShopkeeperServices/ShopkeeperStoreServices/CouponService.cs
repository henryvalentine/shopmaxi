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
	public class CouponServices
	{
		private readonly CouponRepository  _couponRepository;
        public CouponServices()
		{
            _couponRepository = new CouponRepository();
		}

        public long AddCoupon(CouponObject coupon)
		{
			try
			{
                return _couponRepository.AddCoupon(coupon);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        
        public int UpdateCoupon(CouponObject coupon)
		{
			try
			{
                return _couponRepository.UpdateCoupon(coupon);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteCoupon(long couponId)
		{
			try
			{
                return _couponRepository.DeleteCoupon(couponId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public CouponObject GetCoupon(long couponId)
		{
			try
			{
                return _couponRepository.GetCoupon(couponId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CouponObject();
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _couponRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public List<CouponObject> GetCountries()
		{
			try
			{
                var objList = _couponRepository.GetCountries();
                if (objList == null || !objList.Any())
			    {
                    return new List<CouponObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CouponObject>();
			}
		}

        public List<CouponObject> GetCouponObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _couponRepository.GetCouponObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<CouponObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CouponObject>();
            }
        }

        public List<CouponObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _couponRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<CouponObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CouponObject>();
            }
        }
	}

}
