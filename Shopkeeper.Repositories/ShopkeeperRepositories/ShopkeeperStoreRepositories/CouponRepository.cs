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
    public class CouponRepository
    {
       private readonly IShopkeeperRepository<Coupon> _repository;
       private readonly UnitOfWork _uoWork;

       public CouponRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<Coupon>(_uoWork);
		}
       
        public long AddCoupon(CouponObject coupon)
        {
            try
            {
                if (coupon == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Title.Trim().ToLower() == coupon.Code.Trim().ToLower() && m.ValidFrom == coupon.ValidFrom && m.ValidTo == coupon.ValidTo);
                if (duplicates > 0)
                {
                    return -3;
                }
                var couponEntity = ModelCrossMapper.Map<CouponObject, Coupon>(coupon);
                if (couponEntity == null || string.IsNullOrEmpty(couponEntity.Title))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(couponEntity);
                _uoWork.SaveChanges();
                return returnStatus.CouponId;
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
                if (coupon == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Title.Trim().ToLower() == coupon.Code.Trim().ToLower() && m.ValidFrom == coupon.ValidFrom && m.ValidTo == coupon.ValidTo && (m.CouponId != coupon.CouponId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var couponEntity = ModelCrossMapper.Map<CouponObject, Coupon>(coupon);
                if (couponEntity == null || couponEntity.CouponId < 1)
                {
                    return -2;
                }
                _repository.Update(couponEntity);
                _uoWork.SaveChanges();
                return 5;
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
                var returnStatus = _repository.Remove(couponId);
                _uoWork.SaveChanges();
                return returnStatus.CouponId > 0;
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
                var myItem = _repository.GetById(couponId);
                if (myItem == null || myItem.CouponId < 1)
                {
                    return new CouponObject();
                }
                var couponObject = ModelCrossMapper.Map<Coupon, CouponObject>(myItem);
                if (couponObject == null || couponObject.CouponId < 1)
                {
                    return new CouponObject();
                }
                couponObject.ValidityPeriod = couponObject.ValidFrom.ToString("dd/MM/yyyy") + " - " + couponObject.ValidTo.ToString("dd/MM/yyyy");
                return couponObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new CouponObject();
            }
        }

        public List<CouponObject> GetCouponObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<Coupon> couponEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    couponEntityList = _repository.GetWithPaging(m => m.CouponId, tpageNumber, tsize).ToList();
                }

                else
                {
                    couponEntityList = _repository.GetAll().ToList();
                }

                if (!couponEntityList.Any())
                {
                    return new List<CouponObject>();
                }
                var couponObjList = new List<CouponObject>();
                couponEntityList.ForEach(m =>
                {
                    var couponObject = ModelCrossMapper.Map<Coupon, CouponObject>(m);
                    if (couponObject != null && couponObject.CouponId > 0)
                    {
                        couponObject.ValidityPeriod = couponObject.ValidFrom.ToString("dd/MM/yyyy") + " - " + couponObject.ValidTo.ToString("dd/MM/yyyy");
                        couponObjList.Add(couponObject);
                    }
                });

                return couponObjList;
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
                var couponEntityList = _repository.GetAll(m => m.Title.ToLower().Contains(searchCriteria.ToLower()) && m.Code.Contains(searchCriteria)).ToList();

                if (!couponEntityList.Any())
                {
                    return new List<CouponObject>();
                }
                var couponObjList = new List<CouponObject>();
                couponEntityList.ForEach(m =>
                {
                    var couponObject = ModelCrossMapper.Map<Coupon, CouponObject>(m);
                    if (couponObject != null && couponObject.CouponId > 0)
                    {
                        couponObject.ValidityPeriod = couponObject.ValidFrom.ToString("dd/MM/yyyy") + " - " + couponObject.ValidTo.ToString("dd/MM/yyyy");
                        couponObjList.Add(couponObject);
                    }
                });
                return couponObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<CouponObject>();
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

        public int GetObjectCount(Expression<Func<Coupon, bool>> predicate)
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

        public List<CouponObject> GetCountries()
        {
            try
            {
                var couponEntityList = _repository.GetAll().ToList();
                if (!couponEntityList.Any())
                {
                    return new List<CouponObject>();
                }
                var couponObjList = new List<CouponObject>();
                couponEntityList.ForEach(m =>
                {
                    var couponObject = ModelCrossMapper.Map<Coupon, CouponObject>(m);
                    if (couponObject != null && couponObject.CouponId > 0 )
                    {
                      couponObjList.Add(couponObject);
                    }
                });
                return couponObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
