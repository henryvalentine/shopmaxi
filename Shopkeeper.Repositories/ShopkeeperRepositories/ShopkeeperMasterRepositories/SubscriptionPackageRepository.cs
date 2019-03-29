using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class SubscriptionPackageRepository
    {
        private readonly IShopkeeperRepository<SubscriptionPackage> _repository;
       private readonly UnitOfWork _uoWork;
        private ShopKeeperMasterEntities _db = new ShopKeeperMasterEntities();

        public SubscriptionPackageRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            _db = new ShopKeeperMasterEntities(entityCnxStringBuilder);
            _uoWork = new UnitOfWork(_db);
            _repository = new ShopkeeperRepository<SubscriptionPackage>(_uoWork);
		}
        public long AddSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
        {
            try
            {
                if (subscriptionPackage == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.PackageTitle.Trim().ToLower() == subscriptionPackage.PackageTitle.Trim().ToLower() &&
                    subscriptionPackage.FileStorageSpace == m.FileStorageSpace && m.NumberOfOutlets == subscriptionPackage.NumberOfOutlets 
                    && m.Registers == subscriptionPackage.Registers && m.NumberOfUsers == subscriptionPackage.NumberOfUsers
                    && m.MaximumTransactions == subscriptionPackage.MaximumTransactions);
                
                if (duplicates > 0)
                {
                    return -3;
                }
                var subscriptionPackageEntity = ModelCrossMapper.Map<SubscriptionPackageObject, SubscriptionPackage>(subscriptionPackage);
                if (subscriptionPackageEntity == null || string.IsNullOrEmpty(subscriptionPackageEntity.PackageTitle))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(subscriptionPackageEntity);
                _uoWork.SaveChanges();
                return returnStatus.SubscriptionPackageId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSubscriptionPackage(SubscriptionPackageObject subscriptionPackage)
        {
            try
            {
                if (subscriptionPackage == null)
                {
                    return -2;
                }
                var duplicates =  _repository.Count(m => m.PackageTitle.Trim().ToLower() == subscriptionPackage.PackageTitle.Trim().ToLower() &&
                    subscriptionPackage.FileStorageSpace == m.FileStorageSpace && m.NumberOfOutlets == subscriptionPackage.NumberOfOutlets 
                    && m.Registers == subscriptionPackage.Registers && m.NumberOfUsers == subscriptionPackage.NumberOfUsers
                    && m.MaximumTransactions == subscriptionPackage.MaximumTransactions && (m.SubscriptionPackageId != subscriptionPackage.SubscriptionPackageId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var subscriptionPackageEntity = ModelCrossMapper.Map<SubscriptionPackageObject, SubscriptionPackage>(subscriptionPackage);
                if (subscriptionPackageEntity == null || subscriptionPackageEntity.SubscriptionPackageId < 1)
                {
                    return -2;
                }
                _repository.Update(subscriptionPackageEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteSubscriptionPackage(long subscriptionPackageId)
        {
            try
            {
                var returnStatus = _repository.Remove(subscriptionPackageId);
                _uoWork.SaveChanges();
                return returnStatus.SubscriptionPackageId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public SubscriptionPackageObject GetSubscriptionPackage(long subscriptionPackageId)
        {
            try
            {
                var myItem = _repository.Get(m => m.SubscriptionPackageId == subscriptionPackageId, "PackagePricings");
                if (myItem == null || myItem.SubscriptionPackageId < 1)
                {
                    return new SubscriptionPackageObject();
                }
                var subscriptionPackageObject = ModelCrossMapper.Map<SubscriptionPackage, SubscriptionPackageObject>(myItem);
                if (subscriptionPackageObject == null || subscriptionPackageObject.SubscriptionPackageId < 1)
                {
                    return new SubscriptionPackageObject();
                }
                if (!subscriptionPackageObject.PackagePricings.Any())
                {
                    return new SubscriptionPackageObject();
                }
                subscriptionPackageObject.Price = subscriptionPackageObject.PackagePricings.ToList()[0].Price;
                return subscriptionPackageObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new SubscriptionPackageObject();
            }
        }

        public List<SubscriptionPackageObject> GetSubscriptionPackageObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<SubscriptionPackage> subscriptionPackageEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    subscriptionPackageEntityList = _repository.GetWithPaging(m => m.SubscriptionPackageId, tpageNumber, tsize).ToList();
                }

                else
                {
                    subscriptionPackageEntityList = _repository.GetAll().ToList();
                }

                if (!subscriptionPackageEntityList.Any())
                {
                    return new List<SubscriptionPackageObject>();
                }
                var subscriptionPackageObjList = new List<SubscriptionPackageObject>();
                subscriptionPackageEntityList.ForEach(m =>
                {
                    var subscriptionPackageObject = ModelCrossMapper.Map<SubscriptionPackage, SubscriptionPackageObject>(m);
                    if (subscriptionPackageObject != null && subscriptionPackageObject.SubscriptionPackageId > 0)
                    {
                        subscriptionPackageObjList.Add(subscriptionPackageObject);
                    }
                });

                return subscriptionPackageObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPackageObject>();
            }
        }
        public List<SubscriptionPackageObject> Search(string searchCriteria)
        {
            try
            {
                List<SubscriptionPackage> subscriptionPackageEntityList;
                long outInt;
                var res = long.TryParse(searchCriteria, out outInt);
                if (res)
                {
                    subscriptionPackageEntityList = _repository.GetAll(m => m.PackageTitle.Contains(searchCriteria) ||
                     m.FileStorageSpace == outInt || m.NumberOfOutlets == outInt
                     || m.Registers == outInt && m.NumberOfUsers == outInt
                     || m.MaximumTransactions == outInt).ToList();
                }
                else
                {
                    subscriptionPackageEntityList = _repository.GetAll(m => m.PackageTitle.Contains(searchCriteria)).ToList();
                }
               
                if (!subscriptionPackageEntityList.Any())
                {
                    return new List<SubscriptionPackageObject>();
                }
                var subscriptionPackageObjList = new List<SubscriptionPackageObject>();
                subscriptionPackageEntityList.ForEach(m =>
                {
                    var subscriptionPackageObject = ModelCrossMapper.Map<SubscriptionPackage, SubscriptionPackageObject>(m);
                    if (subscriptionPackageObject != null && subscriptionPackageObject.SubscriptionPackageId > 0)
                    {
                       subscriptionPackageObjList.Add(subscriptionPackageObject);
                    }
                });
                return subscriptionPackageObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<SubscriptionPackageObject>();
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

        public int GetObjectCount(Expression<Func<SubscriptionPackage, bool>> predicate)
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

        public List<SubscriptionPackageObject> GetSubscriptionPackages()
        {
            try
            {
                using (var db = _db)
                {
                    var subscriptionPackages = (from sp in db.SubscriptionPackages.Include("PackagePricings") select sp).ToList();

                    if (!subscriptionPackages.Any())
                    {
                        return new List<SubscriptionPackageObject>();
                    }
                    
                    var subscriptionPackageObjList = new List<SubscriptionPackageObject>();
                    const string naira = "&#8358;";
                    var decimalEncoded = System.Net.WebUtility.HtmlDecode(naira);
                    subscriptionPackages.ForEach(m =>
                    {
                        var subscriptionPackageObject = ModelCrossMapper.Map<SubscriptionPackage, SubscriptionPackageObject>(m);
                        if (subscriptionPackageObject != null && subscriptionPackageObject.SubscriptionPackageId > 0)
                        {
                            subscriptionPackageObject.PackagePricings = new List<PackagePricingObject>();
                            if (m.PackagePricings.Any())
                            {
                                m.PackagePricings.ToList().ForEach(v =>
                                {
                                    var cycleInfo = db.BillingCycles.Where(k => k.BillingCycleId == v.BillingCycleId).ToList();
                                    if (cycleInfo.Any())
                                    {
                                        var pricingObject = ModelCrossMapper.Map<PackagePricing, PackagePricingObject>(v);
                                        if (pricingObject != null && pricingObject.PackagePricingId > 0)
                                        {
                                            pricingObject.BillingCycleName = decimalEncoded + v.Price + " " + cycleInfo[0].Name;
                                            pricingObject.BillingCycleCode = cycleInfo[0].Code;
                                            pricingObject.Duration = cycleInfo[0].Duration;
                                            subscriptionPackageObject.PackagePricings.Add(pricingObject);
                                        }
                                    }
                                });
                            }
                            subscriptionPackageObjList.Add(subscriptionPackageObject);
                        
                        }
                    });
                    return !subscriptionPackageObjList.Any()? new List<SubscriptionPackageObject>() : subscriptionPackageObjList.OrderBy(m => m.SubscriptionPackageId).ToList();

                    }
               
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
