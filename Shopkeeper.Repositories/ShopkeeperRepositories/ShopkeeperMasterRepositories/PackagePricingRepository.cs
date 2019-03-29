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
    public class PackagePricingRepository
    {
        private readonly IShopkeeperRepository<PackagePricing> _repository;
        private readonly UnitOfWork _uoWork;

        public PackagePricingRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder);
            _uoWork = new UnitOfWork(shopkeeperMasterContext);
            _repository = new ShopkeeperRepository<PackagePricing>(_uoWork);
        }

        public long AddPackagePricing(PackagePricingObject packagePricing)
        {
            try
            {
                if (packagePricing == null)
                {
                    return -2;
                }

                var packagePricingEntity = ModelCrossMapper.Map<PackagePricingObject, PackagePricing>(packagePricing);
                if (packagePricingEntity == null || packagePricingEntity.SubscriptionPackageId < 1)
                {
                    return -2;
                }
                _repository.Add(packagePricingEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdatePackagePricing(PackagePricingObject packagePricing)
        {
            try
            {
                if (packagePricing == null)
                {
                    return -2;
                }
                var packagePricingEntity = ModelCrossMapper.Map<PackagePricingObject, PackagePricing>(packagePricing);
                if (packagePricingEntity == null || packagePricingEntity.SubscriptionPackageId < 1)
                {
                    return -2;
                }
                _repository.Update(packagePricingEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeletePackagePricing(long packagePricingId)
        {
            try
            {
                _repository.Remove(packagePricingId);
                _uoWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public PackagePricingObject GetPackagePricing(long packagePricingId)
        {
            try
            {
                var myItem = _repository.Get(m => m.PackagePricingId == packagePricingId, "BillingCycle, SubscriptionPackage");
                if (myItem == null || myItem.BillingCycleId < 1)
                {
                    return new PackagePricingObject();
                }
                var packagePricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(myItem);
                if (packagePricingObject == null || packagePricingObject.SubscriptionPackageId < 1)
                {
                    return new PackagePricingObject();
                }
                packagePricingObject.SubscriptionPackageTitle = myItem.SubscriptionPackage.PackageTitle;
                packagePricingObject.BillingCycleName = myItem.BillingCycle.Name;
                return packagePricingObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PackagePricingObject();
            }
        }

        public PackagePricingObject GetPackagePricing(long packagePricingId, long billingCycleId)
        {
            try
            {
                var myItem = _repository.Get(m => m.SubscriptionPackageId == packagePricingId && m.BillingCycleId == billingCycleId).ToList();
                if (!myItem.Any())
                {
                    return new PackagePricingObject();
                }
                var packagePricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(myItem[0]);
                if (packagePricingObject == null || packagePricingObject.SubscriptionPackageId < 1)
                {
                    return new PackagePricingObject();
                }
               
                return packagePricingObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new PackagePricingObject();
            }
        }

        public List<PackagePricingObject> GetPackagePricingObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<PackagePricing> packagePricingEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    packagePricingEntityList = _repository.GetWithPaging(m => m.PackagePricingId, tpageNumber, tsize, "BillingCycle, SubscriptionPackage").ToList();
                }

                else
                {
                    packagePricingEntityList = _repository.GetAll("BillingCycle, SubscriptionPackage").ToList();
                }

                if (!packagePricingEntityList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                var packagePricingObjList = new List<PackagePricingObject>();
                packagePricingEntityList.ForEach(m =>
                {
                    var packagePricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(m);
                    if (packagePricingObject != null && packagePricingObject.BillingCycleId > 0)
                    {
                        packagePricingObject.SubscriptionPackageTitle = m.SubscriptionPackage.PackageTitle;
                        packagePricingObject.BillingCycleName = m.BillingCycle.Name;
                        packagePricingObjList.Add(packagePricingObject);
                    }
                });

                return packagePricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
            }
        }

        public List<PackagePricingObject> GetPackagePricingBillingCycle(int? itemsPerPage, int? pageNumber, long billingcycleId)
        {
            try
            {
                List<PackagePricing> packagePricingEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    packagePricingEntityList = _repository.GetWithPaging(m => m.BillingCycleId == billingcycleId, m => m.PackagePricingId, tpageNumber, tsize, "BillingCycle, SubscriptionPackage").ToList();
                }

                else
                {
                    packagePricingEntityList = _repository.GetAll("BillingCycle, SubscriptionPackage").ToList();
                }

                if (!packagePricingEntityList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                var packagePricingObjList = new List<PackagePricingObject>();
                packagePricingEntityList.ForEach(m =>
                {
                    var packagePricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(m);
                    if (packagePricingObject != null && packagePricingObject.BillingCycleId > 0)
                    {
                        packagePricingObject.SubscriptionPackageTitle = m.SubscriptionPackage.PackageTitle;
                        packagePricingObject.BillingCycleName = m.BillingCycle.Name;
                        packagePricingObjList.Add(packagePricingObject);
                    }
                });

                return packagePricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
            }
        }
        
        public List<PackagePricingObject> GetPackagePricingSubscriptionPackage(int? itemsPerPage, int? pageNumber, long subscriptionPackageId)
        {
            try
            {
                List<PackagePricing> packagePricingEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    packagePricingEntityList = _repository.GetWithPaging(m => m.SubscriptionPackageId == subscriptionPackageId, m => m.PackagePricingId, tpageNumber, tsize, "BillingCycle, SubscriptionPackage").ToList();
                }

                else
                {
                    packagePricingEntityList = _repository.GetAll("BillingCycle, SubscriptionPackage").ToList();
                }

                if (!packagePricingEntityList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                var packagePricingObjList = new List<PackagePricingObject>();
                packagePricingEntityList.ForEach(m =>
                {
                    var packagePricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(m);
                    if (packagePricingObject != null && packagePricingObject.BillingCycleId > 0)
                    {
                        packagePricingObject.SubscriptionPackageTitle = m.SubscriptionPackage.PackageTitle;
                        packagePricingObject.BillingCycleName = m.BillingCycle.Name;
                        packagePricingObjList.Add(packagePricingObject);
                    }
                });

                return packagePricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<PackagePricingObject>();
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

        public int GetObjectCount(Expression<Func<PackagePricing, bool>> predicate)
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

        public List<PackagePricingObject> GetPackagePricings()
        {
            try
            {
                var packagePricingEntityList = _repository.GetAll().ToList();
                if (!packagePricingEntityList.Any())
                {
                    return new List<PackagePricingObject>();
                }
                var packagePricingObjList = new List<PackagePricingObject>();
                packagePricingEntityList.ForEach(m =>
                {
                    var packagePricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(m);
                    if (packagePricingObject != null && packagePricingObject.PackagePricingId > 0)
                    {
                        packagePricingObjList.Add(packagePricingObject);
                    }
                });
                return packagePricingObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

    }
}
