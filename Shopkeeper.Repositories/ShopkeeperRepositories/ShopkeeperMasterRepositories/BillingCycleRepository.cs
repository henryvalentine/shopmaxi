using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using ShopKeeper.Master.EF.Models.Master;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories
{
    public class BillingCycleRepository
    {
       private readonly IShopkeeperRepository<BillingCycle> _repository;
       private readonly UnitOfWork _uoWork;

        public BillingCycleRepository()
        {
            var entityCnxStringBuilder = ConfigurationManager.ConnectionStrings["ShopKeeperMasterEntities"].ConnectionString;
            var shopkeeperMasterContext = new ShopKeeperMasterEntities(entityCnxStringBuilder); 
           _uoWork = new UnitOfWork(shopkeeperMasterContext);
           _repository = new ShopkeeperRepository<BillingCycle>(_uoWork);
		}
       
        public long AddBillingCycle(BillingCycleObject billingCycle)
        {
            try
            {
                if (billingCycle == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(billingCycle.Name.Trim().ToLower()) || m.Code.Equals(billingCycle.Code));
                if (duplicates > 0)
                {
                    return -3;
                }
                var billingCycleEntity = ModelCrossMapper.Map<BillingCycleObject, BillingCycle>(billingCycle);
                if (billingCycleEntity == null || string.IsNullOrEmpty(billingCycleEntity.Name))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(billingCycleEntity);
                _uoWork.SaveChanges();
                return returnStatus.BillingCycleId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateBillingCycle(BillingCycleObject billingCycle)
        {
            try
            {
                if (billingCycle == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.Name.Trim().ToLower().Equals(billingCycle.Name.Trim().ToLower()) || m.Code.Equals(billingCycle.Code) && (m.BillingCycleId != billingCycle.BillingCycleId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var billingCycleEntity = ModelCrossMapper.Map<BillingCycleObject, BillingCycle>(billingCycle);
                if (billingCycleEntity == null || billingCycleEntity.BillingCycleId < 1)
                {
                    return -2;
                }
                _repository.Update(billingCycleEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteBillingCycle(long billingCycleId)
        {
            try
            {
                var returnStatus = _repository.Remove(billingCycleId);
                _uoWork.SaveChanges();
                return returnStatus.BillingCycleId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public BillingCycleObject GetBillingCycle(long billingCycleId)
        {
            try
            {
                var myItem = _repository.GetById(billingCycleId);
                if (myItem == null || myItem.BillingCycleId < 1)
                {
                    return new BillingCycleObject();
                }
                var billingCycleObject = ModelCrossMapper.Map<BillingCycle, BillingCycleObject>(myItem);
                if (billingCycleObject == null || billingCycleObject.BillingCycleId < 1)
                {
                    return new BillingCycleObject();
                }
                return billingCycleObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new BillingCycleObject();
            }
        }

        public List<BillingCycleObject> GetBillingCycleObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<BillingCycle> billingCycleEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    billingCycleEntityList = _repository.GetWithPaging(m => m.BillingCycleId, tpageNumber, tsize).ToList();
                }

                else
                {
                    billingCycleEntityList = _repository.GetAll().ToList();
                }

                if (!billingCycleEntityList.Any())
                {
                    return new List<BillingCycleObject>();
                }
                var billingCycleObjList = new List<BillingCycleObject>();
                billingCycleEntityList.ForEach(m =>
                {
                    var billingCycleObject = ModelCrossMapper.Map<BillingCycle, BillingCycleObject>(m);
                    if (billingCycleObject != null && billingCycleObject.BillingCycleId > 0)
                    {
                        billingCycleObjList.Add(billingCycleObject);
                    }
                });

                return billingCycleObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BillingCycleObject>();
            }
        }

        public List<BillingCycleObject> Search(string searchCriteria)
        {
            try
            {
                var duration = 0;
                var dres = int.TryParse(searchCriteria, out duration);
                if (!dres)
                {
                    duration = 0;
                }

                var billingCycleEntityList = _repository.GetAll(m => m.Name.ToLower().Contains(searchCriteria.ToLower()) || m.Code.ToLower().Contains(searchCriteria.ToLower()) || m.Duration == duration).ToList();

                if (!billingCycleEntityList.Any())
                {
                    return new List<BillingCycleObject>();
                }
                var billingCycleObjList = new List<BillingCycleObject>();
                billingCycleEntityList.ForEach(m =>
                {
                    var billingCycleObject = ModelCrossMapper.Map<BillingCycle, BillingCycleObject>(m);
                    if (billingCycleObject != null && billingCycleObject.BillingCycleId > 0)
                    {
                        billingCycleObjList.Add(billingCycleObject);
                    }
                });
                return billingCycleObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<BillingCycleObject>();
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

        public List<BillingCycleObject> GetBillingCycles()
        {
            try
            {
                var billingCycleEntityList = _repository.GetAll().ToList();
                if (!billingCycleEntityList.Any())
                {
                    return new List<BillingCycleObject>();
                }
                var billingCycleObjList = new List<BillingCycleObject>();
                billingCycleEntityList.ForEach(m =>
                {
                    var billingCycleObject = ModelCrossMapper.Map<BillingCycle, BillingCycleObject>(m);
                    if (billingCycleObject != null && billingCycleObject.BillingCycleId > 0)
                    {
                        billingCycleObjList.Add(billingCycleObject);
                    }
                });
                return billingCycleObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
