using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class SubscriptionPricingRepository
    {
        private readonly IShopkeeperRepository<SubscriptionPricing> _repository;
       private readonly UnitOfWork _uoWork;

       public SubscriptionPricingRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<SubscriptionPricing>(_uoWork);
		}
       
        public long AddSubscriptionPricing(SubscriptionPricingObject subscriptionPricing)
        {
            try
            {
                if (subscriptionPricing == null)
                {
                    return -2;
                }
                
                var subscriptionPricingEntity = ModelCrossMapper.Map<SubscriptionPricingObject, SubscriptionPricing>(subscriptionPricing);
                if (subscriptionPricingEntity == null || subscriptionPricingEntity.SubscriptionPackageId < 1)
                {
                    return -2;
                }
                _repository.Add(subscriptionPricingEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSubscriptionPricing(SubscriptionPricingObject subscriptionPricing)
        {
            try
            {
                if (subscriptionPricing == null)
                {
                    return -2;
                }
                var subscriptionPricingEntity = ModelCrossMapper.Map<SubscriptionPricingObject, SubscriptionPricing>(subscriptionPricing);
                if (subscriptionPricingEntity == null || subscriptionPricingEntity.SubscriptionPackageId < 1)
                {
                    return -2;
                }
                _repository.Update(subscriptionPricingEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSubscriptionPricing(long subscriptionPricingId)
        {
            try
            {
                var returnStatus = _repository.Remove(subscriptionPricingId);
                _uoWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SubscriptionPricingObject GetSubscriptionPricing(long subscriptionPricingId)
        {
            try
            {
                var myItem = _repository.GetById(m => m.SubscriptionPricingId == subscriptionPricingId, "BillingCycle, SubscriptionPackage");
                if (myItem == null || myItem.BillingCycleId < 1)
                {
                    return new SubscriptionPricingObject();
                }
                var subscriptionPricingObject = ModelCrossMapper.Map<SubscriptionPricing, SubscriptionPricingObject>(myItem);
                if (subscriptionPricingObject == null || subscriptionPricingObject.SubscriptionPackageId < 1)
                {
                    return new SubscriptionPricingObject();
                }
                subscriptionPricingObject.SubscriptionPackagePackageTitle = myItem.SubscriptionPackage.PackageTitle;
                subscriptionPricingObject.BillingCycleName = myItem.BillingCycle.Name;
                return subscriptionPricingObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new SubscriptionPricingObject();
            }
        }

        public List<SubscriptionPricingObject> GetSubscriptionPricingObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<SubscriptionPricing> subscriptionPricingEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    subscriptionPricingEntityList = _repository.GetWithPaging(m => m.SubscriptionPricingId, tpageNumber, tsize, "BillingCycle, SubscriptionPackage").ToList();
                }

                else
                {
                    subscriptionPricingEntityList = _repository.GetAll("BillingCycle, SubscriptionPackage").ToList();
                }

                if (!subscriptionPricingEntityList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                var subscriptionPricingObjList = new List<SubscriptionPricingObject>();
                subscriptionPricingEntityList.ForEach(m =>
                {
                    var subscriptionPricingObject = ModelCrossMapper.Map<SubscriptionPricing, SubscriptionPricingObject>(m);
                    if (subscriptionPricingObject != null && subscriptionPricingObject.BillingCycleId > 0)
                    {
                        subscriptionPricingObject.SubscriptionPackagePackageTitle = m.SubscriptionPackage.PackageTitle;
                        subscriptionPricingObject.BillingCycleName = m.BillingCycle.Name;
                        subscriptionPricingObjList.Add(subscriptionPricingObject);
                    }
                });

                return subscriptionPricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
            }
        }

        public List<SubscriptionPricingObject> GetSubscriptionPricingBillingCycle(int? itemsPerPage, int? pageNumber, long billingcycleId)
        {
            try
            {
                List<SubscriptionPricing> subscriptionPricingEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    subscriptionPricingEntityList = _repository.GetWithPaging(m => m.BillingCycleId == billingcycleId, m => m.SubscriptionPricingId, tpageNumber, tsize, "BillingCycle, SubscriptionPackage").ToList();
                }

                else
                {
                    subscriptionPricingEntityList = _repository.GetAll("BillingCycle, SubscriptionPackage").ToList();
                }

                if (!subscriptionPricingEntityList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                var subscriptionPricingObjList = new List<SubscriptionPricingObject>();
                subscriptionPricingEntityList.ForEach(m =>
                {
                    var subscriptionPricingObject = ModelCrossMapper.Map<SubscriptionPricing, SubscriptionPricingObject>(m);
                    if (subscriptionPricingObject != null && subscriptionPricingObject.BillingCycleId > 0)
                    {
                        subscriptionPricingObject.SubscriptionPackagePackageTitle = m.SubscriptionPackage.PackageTitle;
                        subscriptionPricingObject.BillingCycleName = m.BillingCycle.Name;
                        subscriptionPricingObjList.Add(subscriptionPricingObject);
                    }
                });

                return subscriptionPricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
            }
        }

        public List<SubscriptionPricingObject> GetSubscriptionPricingSubscriptionPackage(int? itemsPerPage, int? pageNumber, long subscriptionPackageId)
        {
            try
            {
                List<SubscriptionPricing> subscriptionPricingEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    subscriptionPricingEntityList = _repository.GetWithPaging(m => m.SubscriptionPackageId == subscriptionPackageId, m => m.SubscriptionPricingId, tpageNumber, tsize, "BillingCycle, SubscriptionPackage").ToList();
                }

                else
                {
                    subscriptionPricingEntityList = _repository.GetAll("BillingCycle, SubscriptionPackage").ToList();
                }

                if (!subscriptionPricingEntityList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                var subscriptionPricingObjList = new List<SubscriptionPricingObject>();
                subscriptionPricingEntityList.ForEach(m =>
                {
                    var subscriptionPricingObject = ModelCrossMapper.Map<SubscriptionPricing, SubscriptionPricingObject>(m);
                    if (subscriptionPricingObject != null && subscriptionPricingObject.BillingCycleId > 0)
                    {
                        subscriptionPricingObject.SubscriptionPackagePackageTitle = m.SubscriptionPackage.PackageTitle;
                        subscriptionPricingObject.BillingCycleName = m.BillingCycle.Name;
                        subscriptionPricingObjList.Add(subscriptionPricingObject);
                    }
                });

                return subscriptionPricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPricingObject>();
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
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int GetObjectCount(Expression<Func<SubscriptionPricing, bool>> predicate)
        {
            try
            {
                return _repository.Count(predicate);
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<SubscriptionPricingObject> GetSubscriptionPricings()
        {
            try
            {
                var subscriptionPricingEntityList = _repository.GetAll().ToList();
                if (!subscriptionPricingEntityList.Any())
                {
                    return new List<SubscriptionPricingObject>();
                }
                var subscriptionPricingObjList = new List<SubscriptionPricingObject>();
                subscriptionPricingEntityList.ForEach(m =>
                {
                    var subscriptionPricingObject = ModelCrossMapper.Map<SubscriptionPricing, SubscriptionPricingObject>(m);
                    if (subscriptionPricingObject != null && subscriptionPricingObject.SubscriptionPricingId > 0)
                    {
                        subscriptionPricingObjList.Add(subscriptionPricingObject);
                    }
                });
                return subscriptionPricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LoggError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}


